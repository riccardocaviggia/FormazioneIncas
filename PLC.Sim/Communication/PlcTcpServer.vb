Imports System.Diagnostics.Eventing
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Net.Sockets
Imports System.Text
Imports System.Text.Json
Imports System.Threading
Imports CommonSim

Public Class PlcTcpServer
    Private ReadOnly _port As Integer
    Private ReadOnly _logger As ServiceLogger
    Private _listener As TcpListener
    Private _acceptThread As Thread
    Private _cts As CancellationTokenSource

    Private Shared ReadOnly JsonOptions As New JsonSerializerOptions With {
        .PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        .PropertyNameCaseInsensitive = True
    }

    Public Sub New(port As Integer, logger As ServiceLogger)
        _port = port
        _logger = logger
    End Sub

    Public Sub Start()
        _cts = New CancellationTokenSource()
        _listener = New TcpListener(IPAddress.Any, _port)
        _listener.Start()
        _logger?.Info($"PLC.TcpServerStarted[port={_port}]")

        _acceptThread = New Thread(Sub() AcceptLoop(_cts.Token)) With {
            .IsBackground = True,
            .Name = "PlcTcpServer.AcceptLoop"
        }
        _acceptThread.Start()
    End Sub

    Private Sub AcceptLoop(ct As CancellationToken)
        While Not ct.IsCancellationRequested
            Try
                Dim client = _listener.AcceptTcpClient()
                _logger?.Info("PLC.WcsConnected")

                Dim t As New Thread(Sub() HandleClient(client, ct)) With {
                    .IsBackground = True,
                    .Name = "PlcTcpServer.HandleClient"
                }
                t.Start()
            Catch ex As SocketException
                If ct.IsCancellationRequested Then Exit While
                _logger?.[Error]("PLC.AcceptError", ex)
            Catch ex As ObjectDisposedException
                Exit While
            End Try
        End While
    End Sub

    '-------------------------------------------------------------------------------
    '- Gestisce la comunicazione con il WCS: riceve i messaggi
    Private Sub HandleClient(client As TcpClient, ct As CancellationToken)
        Using client
            client.NoDelay = True
            Try
                Using stream = client.GetStream()
                    Using reader As New StreamReader(stream, Encoding.UTF8, False, 4096, True)
                        Using writer As New StreamWriter(stream, Encoding.UTF8, 4096, True) With {.AutoFlush = True}
                            ProcessMessages(reader, writer, ct)
                        End Using
                    End Using
                End Using
            Finally
                _logger?.Info("PLC.WcsDisconnected")
            End Try
        End Using
    End Sub

    '-------------------------------------------------------------------------------
    '- Legge righe dallo stream
    Private Sub ProcessMessages(reader As StreamReader, writer As StreamWriter, ct As CancellationToken)
        While Not ct.IsCancellationRequested
            Dim line As String
            Try
                line = reader.ReadLine()
            Catch ex As IOException
                _logger?.[Error]("PLC.ReadError", ex)
                Exit While
            End Try

            If line Is Nothing Then Exit While

            '- Controllo sullo stato del PLC: risponde OK se riceve "STATUS"
            If line.Trim().ToUpper() = "STATUS" Then
                writer.WriteLine("OK")
                Continue While
            End If

            HandleOrderMessage(line, writer)
        End While
    End Sub

    '-------------------------------------------------------------------------------
    '- Deserializza il messaggio, simula l'esecuzione dell'ordine e invia l'ack
    Private Sub HandleOrderMessage(line As String, writer As StreamWriter)
        Try
            Dim order = JsonSerializer.Deserialize(Of PlcOrderMessage)(line, JsonOptions)
            _logger?.Info($"PLC has received  [OrderId={order?.OrderId};Location={order?.Location}]")

            SimulateMovement(order)

            SendAck(writer, order)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub SimulateMovement(order As PlcOrderMessage)
        Thread.Sleep(1000)
        _logger?.Info($"PLC has executed [OrderId={order?.OrderId};Location={order?.Location}]")
    End Sub

    Private Sub SendAck(writer As StreamWriter, order As PlcOrderMessage)
        Dim ack = New PlcAckMessage With {
                                        .DispatchId = order.DispatchId,
                                        .Ok = True
                                    }
        writer.WriteLine(JsonSerializer.Serialize(ack, JsonOptions))
        _logger?.Info("PLC --- ACK --> WCS")
    End Sub

    Public Sub [Stop]()
        Try
            _cts?.Cancel()
            _listener?.Stop()
        Catch
        End Try
        Try
            _acceptThread?.Join(5000)
        Catch
        End Try
        _listener = Nothing
        _acceptThread = Nothing
        _cts?.Dispose()
        _cts = Nothing
        _logger?.Info("PLC.TcpServerStopped")
    End Sub
End Class
