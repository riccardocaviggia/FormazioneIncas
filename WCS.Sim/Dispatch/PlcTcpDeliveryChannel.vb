Imports System.IO
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

    Private _client As TcpClient
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
                _client = New TcpClient() With {.NoDelay = True}
                _client.Connect(_host, _port)
                Dim stream = _client.GetStream()
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
    '- Costruisce l'ordine, lo invia al PLC e attende l'ACK con timeout
    Public Async Function SendAsync(order As DispatchOrderDto, ct As CancellationToken) As Task Implements IOrderDeliveryChannel.SendAsync
        Dim msg = New PlcOrderMessage With {
            .DispatchId = order.DispatchId,
            .OrderId = order.OrderId,
            .Barcode = order.Barcode,
            .Location = order.Location,
            .Priority = order.Priority
        }

        Try
            Await _writer.WriteLineAsync(JsonSerializer.Serialize(msg, JsonOptions)).ConfigureAwait(False)
            _logger?.Info($"WCS.OrderSentToPlc[OrderId={order.OrderId};Location={order.Location}]")

            '- Attesa ACK con timeout
            Dim readTask = _reader.ReadLineAsync()
            Dim timeoutTask = Task.Delay(_ackTimeout, ct)
            Dim completed = Await Task.WhenAny(readTask, timeoutTask).ConfigureAwait(False)

            If completed Is timeoutTask Then
                _dispatchRepository.UpdateDispatch(order.DispatchId, order.Location, OrderDispatchRepository.StatusFailed)
                _logger?.Log("WARN", "WCS.PlcAckTimeout", $"OrderId={order.OrderId}")
                Return
            End If

            Dim ackLine = Await readTask.ConfigureAwait(False)
            If ackLine Is Nothing Then Throw New IOException("PLC ha chiuso la connessione.")

            Dim ack = JsonSerializer.Deserialize(Of PlcAckMessage)(ackLine, JsonOptions)
            If ack?.Ok Then
                _dispatchRepository.UpdateDispatch(order.DispatchId, order.Location, OrderDispatchRepository.StatusCompleted)
                _logger?.Info($"WCS.OrderCompleted[OrderId={order.OrderId}]")
            Else
                _dispatchRepository.UpdateDispatch(order.DispatchId, order.Location, OrderDispatchRepository.StatusFailed)
                _logger?.Log("WARN", "WCS.OrderFailed", $"OrderId={order.OrderId};Reason={ack?.Reason}")
            End If

        Catch ex As Exception When Not TypeOf ex Is OperationCanceledException
            _dispatchRepository.UpdateDispatch(order.DispatchId, order.Location, OrderDispatchRepository.StatusFailed)
            _logger?.[Error]("WCS.PlcSendError", ex)
            Throw
        End Try
    End Function

    Public Sub Disconnect()
        Try
            _writer?.Dispose()
            _reader?.Dispose()
            _client?.Close()
        Catch
        End Try
        _logger?.Info("WCS.PlcDisconnected")
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Disconnect()
    End Sub
End Class