Imports CommonSim
Imports System.ServiceProcess

Public Class WcsService
    Inherits ServiceBase

    Private _server As WcsTcpServer

    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim port As Integer = TcpConfig.GetTcpPort()

        _server = New WcsTcpServer(port, Sub(m) Console.WriteLine("[WcsService] " & m))

        _server.Start()
    End Sub

    Protected Overrides Sub OnStop()
        If _server IsNot Nothing Then
            _server.Stop()
            _server = Nothing
        End If
    End Sub

End Class
