Imports System.ServiceProcess
Imports CommonSim

Public Class WcsService
    Inherits ServiceBase

    Private _logger As ServiceLogger
    Private _dispatchServer As WcsDispatchServer
    Private _orderQueue As WcsOrderQueue
    Private _orderDispatcher As WcsOrderDispatcher
    Private _deliveryChannel As PlcTcpDeliveryChannel

    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim connectionString = ConnectionStringProvider.GetConnectionString(args)

        _logger = New ServiceLogger(connectionString, "WCS.Sim", Sub()
                                                                 End Sub)

        '-------------------------------------------------------------------------------
        '- Canale TCP verso il PLC (si connette con retry)
        Dim dispatchRepository = New OrderDispatchRepository(connectionString)
        _deliveryChannel = New PlcTcpDeliveryChannel(
            TcpConfig.GetPlcHost(),
            TcpConfig.GetPlcPort(),
            dispatchRepository,
            _logger)
        _deliveryChannel.Connect()

        '-------------------------------------------------------------------------------
        '- Consumer: svuota la coda e invia gli ordini al PLC
        _orderQueue = New WcsOrderQueue()
        _orderDispatcher = New WcsOrderDispatcher(_orderQueue, _deliveryChannel, _logger)
        _orderDispatcher.Start()

        '-------------------------------------------------------------------------------
        '- Avvia il server: riceve i batch dal WMS via HTTPS
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

        _deliveryChannel?.Disconnect()
        _deliveryChannel = Nothing

        _logger?.Info("WCS.Stopped")
    End Sub
End Class