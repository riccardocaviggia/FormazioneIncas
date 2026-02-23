Imports System.IO
Imports System.Net.Sockets
Imports System.Text

Public Class PlcTcpClient
    Private ReadOnly _host As String
    Private ReadOnly _port As Integer
    Private _client As TcpClient
    Private _reader As StreamReader
    Private _writer As StreamWriter

    Public Sub New(host As String, port As Integer)
        _host = host
        _port = port
    End Sub

    Public Sub ConnectOrThrow(Optional timeoutMs As Integer = 5000)
        Disconnect()

        _client = New TcpClient()
        _client.NoDelay = True
        _client.ReceiveTimeout = timeoutMs
        _client.SendTimeout = timeoutMs
        _client.Connect(_host, _port)

        Dim ns = _client.GetStream()
        _reader = New StreamReader(ns, Encoding.UTF8, detectEncodingFromByteOrderMarks:=False, bufferSize:=4096, leaveOpen:=True)
        _writer = New StreamWriter(ns, Encoding.UTF8, bufferSize:=4096, leaveOpen:=True) With {.AutoFlush = True}
    End Sub

    Public Function SendAndWaitAck(line As String) As String
        If _client Is Nothing OrElse Not _client.Connected Then
            Throw New InvalidOperationException("Not connected to WCS server.")
        End If

        _writer.WriteLine(line)
        Dim ack = _reader.ReadLine()
        If ack Is Nothing Then Throw New IOException("Disconnnected while waiting for ack.")
        Return ack
    End Function

    Public Sub Disconnect()
        Try
            If _reader IsNot Nothing Then _reader.Dispose()
        Catch ex As Exception
        End Try
        Try
            If _writer IsNot Nothing Then _writer.Dispose()
        Catch ex As Exception
        End Try
        Try
            If _client IsNot Nothing Then _client.Close()
        Catch ex As Exception
        End Try
        _reader = Nothing
        _writer = Nothing
        _client = Nothing
    End Sub
End Class
