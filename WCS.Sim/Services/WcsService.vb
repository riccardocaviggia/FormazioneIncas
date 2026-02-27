Imports System.ServiceProcess
Imports CommonSim

Public Class WcsService
    Inherits ServiceBase

    Private _server As WcsTcpServer
    Private _logger As ServiceLogger

    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim port = TcpConfig.GetTcpPort()
        Dim wmsEndpoint = WmsConfig.GetWmsEndpoint()
        Dim connectionString = ConnectionStringProvider.GetConnectionString(args)

        _logger = New ServiceLogger(connectionString, "WCS.Sim", Sub()
                                                                 End Sub)

        Dim wmsClient As IWmsClient = New WmsWcfClient(wmsEndpoint)
        Dim handler As IWcsMessageHandler = New BarcodeMessageHandler(wmsClient, _logger)

        _server = New WcsTcpServer(
            port,
            handler,
            logger:=_logger)

        _server.Start()

        _logger.Info("WCS.Started")
    End Sub

    Protected Overrides Sub OnStop()
        If _server IsNot Nothing Then
            _server.Stop()
            _server = Nothing
        End If

        _logger?.Info("WCS.Stopped")
    End Sub
End Class