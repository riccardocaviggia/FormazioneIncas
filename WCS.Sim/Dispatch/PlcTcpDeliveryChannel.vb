Imports System.IO
Imports System.Net.Http
Imports System.Net.Sockets
Imports System.Text
Imports System.Text.Json
Imports System.Threading
Imports CommonSim

Public Class PlcTcpDeliveryChannel
    Implements IOrderDeliveryChannel
    Implements IDisposable

    Private ReadOnly _host As String
    Private ReadOnly _port As Integer
    Private ReadOnly _dispatchRepository As OrderDispatchRepository
    Private ReadOnly _logger As ServiceLogger
    Private ReadOnly _ackTimeout As TimeSpan

    Private Shared _httpClient As HttpClient

    Private _tcpClient As TcpClient
    Private _reader As StreamReader
    Private _writer As StreamWriter

    Private Shared ReadOnly JsonOptions As New JsonSerializerOptions With {
        .PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        .WriteIndented = False
    }

    Public Sub New(host As String,
                   port As Integer,
                   dispatchRepository As OrderDispatchRepository,
                   logger As ServiceLogger,
                   Optional ackTimeout As TimeSpan = Nothing)
        If String.IsNullOrWhiteSpace(host) Then Throw New ArgumentNullException(NameOf(host))
        If dispatchRepository Is Nothing Then Throw New ArgumentNullException(NameOf(dispatchRepository))
        _host = host
        _port = port
        _dispatchRepository = dispatchRepository
        _logger = logger
        _ackTimeout = If(ackTimeout = TimeSpan.Zero, TimeSpan.FromSeconds(10), ackTimeout)
    End Sub

    Public Sub Connect()
        Dim deadline = DateTime.UtcNow.AddSeconds(30)
        While True
            Try
                _tcpClient = New TcpClient() With {.NoDelay = True}
                _tcpClient.Connect(_host, _port)
                Dim stream = _tcpClient.GetStream()
                _reader = New StreamReader(stream, Encoding.UTF8, False, 4096, True)
                _writer = New StreamWriter(stream, Encoding.UTF8, 4096, True) With {.AutoFlush = True}
                _logger?.Info("WCS.PlcConnected")
                Return
            Catch ex As SocketException
                If DateTime.UtcNow > deadline Then
                    _logger?.Error("WCS.PlcConnectionFailed", ex)
                    Throw
                End If
                _logger?.Log("WARN", "WCS.PlcConnectionRetry", ex.Message)
                Thread.Sleep(1000)
            End Try
        End While
    End Sub

    '-------------------------------------------------------------------------------
    '- Invia l'ordine al PLC e attende l'ACK. Invia l'ACK al WMS
    Public Async Function SendAsync(order As DispatchOrderDto, ct As CancellationToken) As Task Implements IOrderDeliveryChannel.SendAsync
        Try
            Dim msg = BuildOrder(order)
            Await _writer.WriteLineAsync(JsonSerializer.Serialize(msg, JsonOptions)).ConfigureAwait(False)
            _logger?.Info($"WCS --- [OrderId={order.OrderId};Location={order.Location}] --> PLC")

            '- Attesa ACK con timeout
            Dim ackline = Await WaitForAckAsync(order, ct).ConfigureAwait(False)
            If ackline IsNot Nothing Then
                _logger?.Info("WCS received ACK from PLC")
                Await sendAckToWmsAsync(order.OrderId, order.Location).ConfigureAwait(False)
            End If

        Catch ex As Exception When Not TypeOf ex Is OperationCanceledException
            _dispatchRepository.UpdateDispatch(order.OrderId, order.Location, OrderDispatchRepository.StatusFailed)
            _logger?.[Error]("WCS.PlcSendError", ex)
            Throw
        End Try
    End Function

    '-------------------------------------------------------------------------------
    '- Costruisce l'ordine da inviare al PLC
    Private Shared Function BuildOrder(order As DispatchOrderDto)
        Return New PlcOrderMessage With {
            .DispatchId = order.DispatchId,
            .OrderId = order.OrderId,
            .Barcode = order.Barcode,
            .Location = order.Location,
            .Priority = order.Priority
        }
    End Function

    '-------------------------------------------------------------------------------
    '- Attende l'ACK dal PLC con timeout. Se scade, restituisce Nothing
    Private Async Function WaitForAckAsync(order As DispatchOrderDto, ct As CancellationToken) As Task(Of String)
        Dim readTask = _reader.ReadLineAsync()
        Dim timeoutTask = Task.Delay(_ackTimeout, ct)
        Dim completed = Await Task.WhenAny(readTask, timeoutTask).ConfigureAwait(False)

        If completed Is timeoutTask Then
            _dispatchRepository.UpdateDispatch(order.OrderId, order.Location, OrderDispatchRepository.StatusFailed)
            _logger?.Log("WARN", "WCS.PlcAckTimeout", $"OrderId={order.OrderId}")
            Return Nothing
        End If

        Dim ackLine = Await readTask.ConfigureAwait(False)
        If ackLine Is Nothing Then Throw New IOException("PLC connection has been closed.")

        Return ackLine
    End Function

    '-------------------------------------------------------------------------------
    '- Invia l'ACK al WMS tramite chiamata HTTP POST all'endpoint /dispatch/completed/{orderId}?location={location}
    Private Async Function sendAckToWmsAsync(orderId As String, location As String) As Task
        Try
            Dim endpoint = $"https://localhost:8443/wms/dispatch/completed/{orderId}?location={location}"
            _httpClient = New HttpClient()


            Using request As New HttpRequestMessage(HttpMethod.Post, endpoint)
                request.Content = New StringContent($"order {orderId} completed")
                request.Headers.TryAddWithoutValidation("Authorization", BasicAuthenticator.BuildHeaderValue(WmsConfig.GetAuthUsername, WmsConfig.GetAuthPassword))
                _logger?.Info("WCS --- ACK --> WMS")
                Dim response = Await _httpClient.SendAsync(request).ConfigureAwait(False)

                If Not response.IsSuccessStatusCode Then
                    _logger?.Log("WARN", "WCS.AckSendFailed", $"StatusCode={response.StatusCode}")
                End If
            End Using
        Catch ex As Exception
            _logger?.[Error]("WCSC.SendAckToWmsError", ex)
        End Try
    End Function

    Public Sub Disconnect()
        Try
            _writer?.Dispose()
            _reader?.Dispose()
            _tcpClient?.Close()
            _httpClient?.Dispose()
        Catch
        End Try
        _logger?.Info("WCS.PlcDisconnected")
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Disconnect()
    End Sub
End Class