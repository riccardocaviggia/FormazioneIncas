Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Text.Json
Imports System.Threading
Imports CommonSim

Public Class WcsTcpServer
    Private ReadOnly _port As Integer
    Private ReadOnly _log As Action(Of String)
    Private ReadOnly _handler As IWcsMessageHandler
    Private _listener As TcpListener
    Private _acceptThread As Thread
    Private _cts As CancellationTokenSource

    Public Sub New(port As Integer, handler As IWcsMessageHandler, Optional log As Action(Of String) = Nothing)
        If handler Is Nothing Then Throw New ArgumentNullException(NameOf(handler))
        _port = port
        _handler = handler
        _log = If(log, Sub(msg)
                       End Sub)
    End Sub

    Public Sub Start()
        If _listener IsNot Nothing Then
            Throw New InvalidOperationException("Server is already running.")
        End If

        _cts = New CancellationTokenSource()
        _listener = New TcpListener(IPAddress.Any, _port)
        _listener.Start()

        _log("WcsTcpServer started. Listening on port " & _port)

        _acceptThread = New Thread(Sub() AcceptLoop(_cts.Token)) With {.IsBackground = True, .Name = "WcsTcpServer.AcceptLoop"}
        _acceptThread.Start()
    End Sub

    Public Sub [Stop]()
        _log("WcsTcpServer stopping...")

        Try
            If _cts IsNot Nothing Then _cts.Cancel()
        Catch ex As Exception
            _log("Error cancelling token: " & ex.ToString())
        End Try

        Try
            If _listener IsNot Nothing Then _listener.Stop()
        Catch ex As Exception
            _log("Error stopping listener: " & ex.ToString())
        End Try

        Try
            If _acceptThread IsNot Nothing AndAlso _acceptThread.IsAlive Then
                _acceptThread.Join(millisecondsTimeout:=5000)
            End If
        Catch ex As Exception
            _log("Error joining accept thread: " & ex.ToString())
        End Try

        Try
            If _cts IsNot Nothing Then _cts.Dispose()
        Catch ex As Exception
            _log("Error disposing token: " & ex.ToString())
        End Try

        _acceptThread = Nothing
        _listener = Nothing
        _cts = Nothing
        _log("WcsTcpServer stopped.")
    End Sub

    Private Sub AcceptLoop(ct As CancellationToken)
        While Not ct.IsCancellationRequested
            Try
                Dim client As TcpClient = _listener.AcceptTcpClient()
                _log("Client connected")

                Dim t As New Thread(Sub() HandleClient(client, ct)) With {.IsBackground = True, .Name = "WcsTcpServer.HandleClient"}
                t.Start()
            Catch ex As SocketException
                If ct.IsCancellationRequested Then Exit While
                _log("SocketException in AcceptLoop: " & ex.ToString())
            Catch ex As ObjectDisposedException
                If ct.IsCancellationRequested Then Exit While
                _log("ObjectDisposedException in AcceptLoop: " & ex.ToString())
            Catch ex As Exception
                _log("Unexpected exception in AcceptLoop: " & ex.ToString())
            End Try
        End While
    End Sub

    Private Sub HandleClient(client As TcpClient, ct As CancellationToken)
        Using client
            client.NoDelay = True

            Try
                Using ns = client.GetStream()
                    Using reader As New StreamReader(ns, Encoding.UTF8, detectEncodingFromByteOrderMarks:=False, bufferSize:=4096, leaveOpen:=True)
                        Using writer As New StreamWriter(ns, Encoding.UTF8, bufferSize:=4096, leaveOpen:=True) With {.AutoFlush = True}
                            While Not ct.IsCancellationRequested
                                Dim line As String

                                Try
                                    line = reader.ReadLine()
                                Catch ex As IOException
                                    _log("Client read IO error: " & ex.Message)
                                    Exit While
                                Catch ex As Exception
                                    _log("Client read unexpected error: " & ex.ToString())
                                    Exit While
                                End Try

                                If line Is Nothing Then Exit While
                                If line.Length = 0 Then Continue While

                                _log("Received from PLC: " & line)

                                Try
                                    Dim msg = JsonSerializer.Deserialize(Of WcsInboundMessage)(line, _jsonOptions)

                                    Dim ackMessage As AckMessage = Nothing
                                    Try
                                        ackMessage = _handler.HandleAsync(msg, ct).GetAwaiter().GetResult()
                                    Catch ex As OperationCanceledException When ct.IsCancellationRequested
                                        Exit While
                                    Catch ex As Exception
                                        _log("ERROR handling inbound message: " & ex.ToString())
                                        ackMessage = New AckMessage With {
                                            .Id = msg?.Id,
                                            .Ok = False
                                        }
                                    End Try

                                    ' Manda ACK al PLC
                                    Dim ackJson = JsonSerializer.Serialize(AckMessage, _jsonOptions)
                                    writer.WriteLine(ackJson)
                                    _log("Sent ACK to PLC: " & ackJson)

                                Catch ex As JsonException
                                    _log("Invalid JSON from PLC: " & ex.Message)
                                    Exit While
                                Catch ex As IOException
                                    _log("Client write IO error: " & ex.Message)
                                    Exit While
                                End Try
                            End While
                        End Using
                    End Using
                End Using
            Finally
                _log("Client disconnected")
            End Try
        End Using
    End Sub

    Private Shared ReadOnly _jsonOptions As New JsonSerializerOptions() With {
        .PropertyNameCaseInsensitive = True
    }

End Class