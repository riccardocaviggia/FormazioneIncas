Imports System.ServiceProcess
Imports CommonSim

Public Class WcsService
    Inherits ServiceBase

    Private _logger As ServiceLogger
    Private _dispatchServer As WcsDispatchServer
    Private _orderQueue As WcsOrderQueue
    Private _orderDispatcher As WcsOrderDispatcher

    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim connectionString = ConnectionStringProvider.GetConnectionString(args)

        _logger = New ServiceLogger(connectionString, "WCS.Sim", Sub()
                                                                 End Sub)
        _orderQueue = New WcsOrderQueue()

        '-------------------------------------------------------------------------------
        '- Placeholder per il canale di consegna al PLC
        Dim deliveryChannel As IOrderDeliveryChannel = New LoggingOrderDeliveryChannel(_logger)

        '-------------------------------------------------------------------------------
        '- Avvia il consumer: pronto ad elaborare prima che arrivino ordini
        _orderDispatcher = New WcsOrderDispatcher(_orderQueue, deliveryChannel, _logger)
        _orderDispatcher.Start()

        '-------------------------------------------------------------------------------
        '- Avvia il server: accetta richieste solo dopo che il consumer è attivo
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

        _logger?.Info("WCS.Stopped")
    End Sub
End Class