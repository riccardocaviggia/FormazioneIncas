Imports System.ServiceProcess
Imports CommonSim

Public Class WcsService
    Inherits ServiceBase

    Private _server As WcsTcpServer

    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim port = TcpConfig.GetTcpPort()
        Dim wmsEndpoint = WmsConfig.GetWmsEndpoint()

        Dim wmsClient As IWmsClient = New WmsWcfClient(wmsEndpoint)
        Dim handler As IWcsMessageHandler = New BarcodeMessageHandler(wmsClient, Sub(msg) Console.WriteLine("[Handler] " & msg))

        _server = New WcsTcpServer(port, handler, Sub(msg) Console.WriteLine("[WcsService] " & msg))
        _server.Start()

        Console.WriteLine("[WCS] Started on port " & port)
        Console.WriteLine("[WCS] WMS endpoint: " & wmsEndpoint)
    End Sub

    Protected Overrides Sub OnStop()
        If _server IsNot Nothing Then
            _server.Stop()
            _server = Nothing
        End If
    End Sub

End Class