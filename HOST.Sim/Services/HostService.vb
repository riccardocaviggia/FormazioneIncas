Imports CommonSim

Public Class HostService
    Private _server As HostHttpServer
    Private _logger As ServiceLogger

    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim cs As String = ConnectionStringProvider.GetConnectionString(args)
        Dim endpoint As String = HostConfig.GetHostEndpoint()
        Dim repository As New BarcodeRepository(cs)
        Dim authorizationService As IBarcodeAuthorizationService = New BarcodeAuthorizationService(repository)

        _logger = New ServiceLogger(cs, "HOST.Sim", Sub()
                                                    End Sub)

        _server = New HostHttpServer(endpoint, authorizationService, logger:=_logger)
        _server.Start()

        _logger.Info("HOST.Started")
    End Sub

    Protected Overrides Sub OnStop()
        If _server IsNot Nothing Then
            _server.Stop()
            _server = Nothing
        End If

        _logger?.Info("HOST.Stopped")
    End Sub
End Class
