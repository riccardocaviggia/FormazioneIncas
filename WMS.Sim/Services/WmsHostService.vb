Imports System.ServiceModel
Imports System.ServiceProcess
Imports CommonSim

Public Class WmsHostService
    Inherits ServiceBase

    Private _host As ServiceHost
    Private _logger As ServiceLogger
    Private _dispatchServer As WmsDispatchServer
    Private _dispatchRepository As OrderDispatchRepository

    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim connectionString As String = ConnectionStringProvider.GetConnectionString(args)

        _logger = New ServiceLogger(connectionString, "WMS.Sim", Sub()
                                                                 End Sub)
        Try
            '-------------------------------------------------------------------------------
            'Client per chiamare il WCS (Warehouse Control System) e dirgli di eseguire le missioni di movimentazione merci
            Dim wcsClient = New WcsDispatchClient(WcsConfig.GetDispatchEndpoint())
            'Elenco ordini da mandare al WCS
            _dispatchRepository = New OrderDispatchRepository(connectionString)
            '-------------------------------------------------------------------------------
            'Genera delle locazioni (vani)
            Dim allocator = New LocationAllocator()
            'Processor che riceve le richieste di dispatch dal WmsDispatchServer (vedi sotto) e le trasforma in missioni da mandare al WCS tramite wcsClient.
            Dim processor = New WmsDispatchProcessor(allocator, wcsClient, _dispatchRepository, _logger)
            '-------------------------------------------------------------------------------
            '- HTTP dispatch server (riceve le chiamate dal WmsDispatchClient (HOST)) e le passa al processor per l'elaborazione. 
            'Il server è implementato con HttpListener, non con WCF, per semplicità e leggerezza (non serve tutta la complessità di WCF per questo scenario)
            _dispatchServer = New WmsDispatchServer(WmsConfig.GetDispatchEndpoint(), processor, _logger)
            _dispatchServer.Start()
            _logger.Info("WMS.DispatchServerStarted")
            '-------------------------------------------------------------------------------
        Catch ex As Exception
            _logger?.[Error]("WMS.DispatchServerStartError", ex)
            Throw '- il dispatch è critico: se non parte, il servizio non ha senso
        End Try

        _logger.Info("WMS.Started")
    End Sub

    Protected Overrides Sub OnStop()
        Try
            If _host IsNot Nothing Then
                Try
                    _host.Close()
                Catch ex As Exception
                    _logger?.[Error]("WMS.OnStop.HostCloseError", ex)
                End Try
                _host = Nothing
            End If

            _dispatchServer?.[Stop]()
            _dispatchServer = Nothing

            _logger?.Info("WMS.Stopped")
        Catch ex As Exception
            _logger?.[Error]("WMS.OnStop.Error", ex)
        End Try
    End Sub
End Class