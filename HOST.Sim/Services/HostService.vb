Imports CommonSim

Public Class HostService
    Private _server As HostHttpServer
    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim cs As String = ConnectionStringProvider.GetConnectionString(args)
        Dim endpoint As String = HostConfig.GetHostEndpoint()
        Dim repository As New BarcodeRepository(cs)

        _server = New HostHttpServer(endpoint, repository, Sub(msg) Console.WriteLine("[HostService] " & msg))
        _server.Start()

        Console.WriteLine("[HostService] Started. Listening on " & endpoint)
    End Sub

    Protected Overrides Sub OnStop()
        If _server IsNot Nothing Then
            _server.Stop()
            _server = Nothing
        End If
    End Sub

End Class
