Imports System.ServiceModel
Imports System.ServiceProcess
Imports CommonSim

Public Class WmsHostService
    Inherits ServiceBase

    Private _host As ServiceHost
    Private _logger As ServiceLogger

    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim wmsEndpoint As New Uri(WmsConfig.GetWmsEndpoint())
        Dim hostEndpoint As String = HostConfig.GetHostEndpoint()
        Dim connectionString As String = ConnectionStringProvider.GetConnectionString(args)

        _logger = New ServiceLogger(connectionString, "WMS.Sim", Sub()
                                                                 End Sub)

        Dim hostGateway As IHostGateway = New HostHttpClient(hostEndpoint)
        Dim handler As IWmsBarcodeHandler = New WmsBarcodeHandler(hostGateway, _logger)

        _host = New ServiceHost(GetType(WmsService), wmsEndpoint)
        _host.Description.Behaviors.Add(New WmsServiceBehavior(handler))

        Dim binding As New BasicHttpBinding()
        _host.AddServiceEndpoint(GetType(IWmsService), binding, "")

        Dim smb As New Description.ServiceMetadataBehavior With {.HttpGetEnabled = True}
        _host.Description.Behaviors.Add(smb)

        _host.Open()
        _logger.Info("WMS.Started")
    End Sub

    Protected Overrides Sub OnStop()
        If _host IsNot Nothing Then
            _host.Close()
            _host = Nothing
        End If

        _logger?.Info("WMS.Stopped")
    End Sub
End Class