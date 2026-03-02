Imports System.ServiceProcess
Imports CommonSim

Public Class WcsService
    Inherits ServiceBase

    Private _server As WcsTcpServer
    Private _logger As ServiceLogger
    Private _dispatchServer As WcsDispatchServer
    Private _orderQueue As WcsOrderQueue
    Private _orderDispatcher As WcsOrderDispatcher

    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim port = TcpConfig.GetTcpPort()
        Dim wmsEndpoint = WmsConfig.GetWmsEndpoint()
        Dim connectionString = ConnectionStringProvider.GetConnectionString(args)

        _logger = New ServiceLogger(connectionString, "WCS.Sim", Sub()
                                                                 End Sub)

        Dim wmsClient As IWmsClient = New WmsWcfClient(wmsEndpoint)
        Dim handler As IWcsMessageHandler = New BarcodeMessageHandler(wmsClient, _logger)

        _server = New WcsTcpServer(port, handler, logger:=_logger)
        _server.Start()

        _orderQueue = New WcsOrderQueue()
        Dim deliveryChannel As IOrderDeliveryChannel = New LoggingOrderDeliveryChannel(_logger)
        _orderDispatcher = New WcsOrderDispatcher(_orderQueue, deliveryChannel, _logger)
        _orderDispatcher.Start()

        _dispatchServer = New WcsDispatchServer(WcsConfig.GetDispatchEndpoint(), _orderQueue, _logger)
        _dispatchServer.Start()

        _logger.Info("WCS.Started")
    End Sub

    Protected Overrides Sub OnStop()
        _dispatchServer?.[Stop]()
        _dispatchServer = Nothing

        _orderDispatcher?.[Stop]()
        _orderDispatcher = Nothing

        _orderQueue?.Dispose()
        _orderQueue = Nothing

        If _server IsNot Nothing Then
            _server.Stop()
            _server = Nothing
        End If

        _logger?.Info("WCS.Stopped")
    End Sub
End Class