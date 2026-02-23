Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

Public Class WcsTcpServer
    Private ReadOnly _port As Integer
    Private ReadOnly _log As Action(Of String)
    Private _listener As TcpListener
    Private _acceptThread As Thread ' Thread per l'accettazione di nuovi client
    Private _cts As CancellationTokenSource ' Token per la cancellazione dei thread

    Public Sub New(port As Integer, Optional log As Action(Of String) = Nothing)
        _port = port
        _log = If(log, Sub(msg)
                       End Sub)
    End Sub

    Public Sub Start()
        If _listener IsNot Nothing Then
            Throw New InvalidOperationException("Server is already running.")
        End If

        _cts = New CancellationTokenSource()
        _listener = New TcpListener(IPAddress.Any, _port) ' TcpListener che ascolta su tutte le interfacce di rete
        _listener.Start()

        _log("WcsTcpServer started. Listening on port " & _port)

        _acceptThread = New Thread(Sub() AcceptLoop(_cts.Token)) With {.IsBackground = True, .Name = "WcsTcpServer.AcceptLoop"} ' Thread background per accettare connessioni
        _acceptThread.Start()
    End Sub

    Public Sub [Stop]()
        _log("WcsTcpServer stopping...")

        Try
            If _cts IsNot Nothing Then
                _cts.Cancel()
            End If
        Catch ex As Exception
            _log("Error cancelling token: " & ex.ToString())
        End Try

        Try
            If _listener IsNot Nothing Then
                _listener.Stop()
            End If
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
            If _cts IsNot Nothing Then
                _cts.Dispose()
            End If
        Catch ex As Exception
            _log("Error disposing token: " & ex.ToString())
        End Try

        _acceptThread = Nothing
        _listener = Nothing
        _cts = Nothing
        _log("WcsTcpServer stopped.")
    End Sub

    Private Sub AcceptLoop(ct As CancellationToken) 'ogni client viene gestito in un thread separato, il loop continua ad accettare nuovi client finché non viene richiesto di fermarsi
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
            client.NoDelay = True ' Invia subito i pacchetti senza aspettare di riempire il buffer

            Try
                Using ns = client.GetStream()
                    Using reader As New StreamReader(ns, Encoding.UTF8, detectEncodingFromByteOrderMarks:=False, bufferSize:=4096, leaveOpen:=True)
                        Using writer As New StreamWriter(ns, Encoding.UTF8, bufferSize:=4096, leaveOpen:=True) With {.AutoFlush = True} ' AutoFluch scrive immediatamente nello stream, LeaveOpen lascia lo stream aperto quando il writer viene distrutto
                            While Not ct.IsCancellationRequested
                                Dim line As String

                                Try
                                    line = reader.ReadLine()
                                Catch ex As IOException
                                    _log("Client read IO error: " & ex.Message())
                                    Exit While
                                Catch ex As Exception
                                    _log("Client read unexpected error: " & ex.ToString())
                                    Exit While
                                End Try

                                If line Is Nothing Then Exit While
                                If line.Length = 0 Then Continue While

                                _log("Received from client: " & line)
                                Dim ack As String = "{""type"":""ack"",""ok"":true}"

                                Try
                                    writer.WriteLine(ack)
                                    _log("Sent to client: " & ack)
                                Catch ex As IOException
                                    _log("Client write IO error: " & ex.Message())
                                    Exit While
                                Catch ex As Exception
                                    _log("Client write unexpected error: " & ex.ToString())
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
End Class
