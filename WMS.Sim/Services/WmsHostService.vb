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

        ' WCF barcode service — indipendente dal dispatch server


        ' HTTP dispatch server — indipendente dal WCF host
        Try
            Dim allocator = New LocationAllocator()
            Dim wcsClient = New WcsDispatchClient(WcsConfig.GetDispatchEndpoint())
            _dispatchRepository = New OrderDispatchRepository(connectionString)
            Dim processor = New WmsDispatchProcessor(allocator, wcsClient, _dispatchRepository, _logger)
            _dispatchServer = New WmsDispatchServer(WmsConfig.GetDispatchEndpoint(), processor, _logger)
            _dispatchServer.Start()
            _logger.Info("WMS.DispatchServerStarted")
        Catch ex As Exception
            _logger?.[Error]("WMS.DispatchServerStartError", ex)
            Throw ' il dispatch è critico: se non parte, il servizio non ha senso
        End Try

        _logger.Info("WMS.Started")
    End Sub

    Protected Overrides Sub OnStop()
        If _host IsNot Nothing Then
            Try
                _host.Close()
            Catch
            End Try
            _host = Nothing
        End If

        _dispatchServer?.[Stop]()
        _dispatchServer = Nothing

        _logger?.Info("WMS.Stopped")
    End Sub
End Class