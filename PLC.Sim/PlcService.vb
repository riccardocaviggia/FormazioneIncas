Imports System.ServiceProcess
Imports CommonSim

Public Class PlcService
    Inherits ServiceBase

    Private _client As PlcTcpClient

    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim host As String = TcpConfig.GetTcpHost()
        Dim port As Integer = TcpConfig.GetTcpPort()

        _client = New PlcTcpClient(host, port)
        _client.ConnectOrThrow()

        Dim msg As String = "{""type"":""hello"",""from"":""PLC""}"
        Dim ack As String = _client.SendAndWaitAck(msg)

        Console.WriteLine("[PLC] sent: " & msg)
        Console.WriteLine("[PLC] received ack: " & ack)
    End Sub

    Protected Overrides Sub OnStop()
        If _client IsNot Nothing Then
            _client.Disconnect()
            _client = Nothing
        End If
    End Sub

End Class
