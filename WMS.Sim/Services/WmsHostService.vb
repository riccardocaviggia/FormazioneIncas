Imports System.ServiceModel
Imports System.ServiceProcess
Imports CommonSim

Public Class WmsHostService
    Inherits ServiceBase

    Private _host As ServiceHost

    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim wmsEndpoint As New Uri(WmsConfig.GetWmsEndpoint())
        Dim hostEndpoint As String = HostConfig.GetHostEndpoint()

        Dim hostGateway As IHostGateway = New HostHttpClient(hostEndpoint)
        Dim handler As IWmsBarcodeHandler = New WmsBarcodeHandler(hostGateway, Sub(msg) Console.WriteLine(msg))

        _host = New ServiceHost(GetType(WmsService), wmsEndpoint)
        _host.Description.Behaviors.Add(New WmsServiceBehavior(handler))

        Dim binding As New BasicHttpBinding()
        _host.AddServiceEndpoint(GetType(IWmsService), binding, "")

        Dim smb As New Description.ServiceMetadataBehavior With {.HttpGetEnabled = True}
        _host.Description.Behaviors.Add(smb)

        _host.Open()
        Console.WriteLine("[WMS] WCF service started at " & wmsEndpoint.ToString())
        Console.WriteLine("[WMS] HOST endpoint: " & hostEndpoint)
    End Sub

    Protected Overrides Sub OnStop()
        If _host IsNot Nothing Then
            _host.Close()
            _host = Nothing
        End If
        Console.WriteLine("[WMS] WCF service stopped.")
    End Sub
End Class