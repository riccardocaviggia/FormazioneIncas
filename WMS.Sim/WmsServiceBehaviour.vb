Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Description
Imports System.ServiceModel.Dispatcher
Imports CommonSim

Public Class WmsServiceBehavior
    Implements IServiceBehavior

    Private ReadOnly _hostClient As HostHttpClient

    Public Sub New(hostClient As HostHttpClient)
        _hostClient = hostClient
    End Sub

    Public Sub AddBindingParameters(serviceDescription As ServiceDescription, serviceHostBase As ServiceHostBase, endpoints As ObjectModel.Collection(Of ServiceEndpoint), bindingParameters As BindingParameterCollection) Implements IServiceBehavior.AddBindingParameters
        ' nulla da fare
    End Sub

    Public Sub ApplyDispatchBehavior(serviceDescription As ServiceDescription, serviceHostBase As ServiceHostBase) Implements IServiceBehavior.ApplyDispatchBehavior
        For Each channelDispatcher As ChannelDispatcher In serviceHostBase.ChannelDispatchers
            For Each endpointDispatcher As EndpointDispatcher In channelDispatcher.Endpoints
                endpointDispatcher.DispatchRuntime.InstanceProvider = New WmsInstanceProvider(_hostClient)
            Next
        Next
    End Sub

    Public Sub Validate(serviceDescription As ServiceDescription, serviceHostBase As ServiceHostBase) Implements IServiceBehavior.Validate
        ' nulla da fare
    End Sub

End Class
