Imports CommonSim
Imports System.ServiceProcess

Public Class WcsService
    Inherits ServiceBase

    Private _server As WcsTcpServer

    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim port As Integer = TcpConfig.GetTcpPort()
        Dim cs As String = ConnectionStringProvider.GetConnectionString(args)


        _server = New WcsTcpServer(port, Sub(msg) Console.WriteLine("[WcsService] " & msg))
        _server.Start()
    End Sub

    Protected Overrides Sub OnStop()
        If _server IsNot Nothing Then
            _server.Stop()
            _server = Nothing
        End If
    End Sub

End Class
