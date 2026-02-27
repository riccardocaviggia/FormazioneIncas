Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Description
Imports System.ServiceModel.Dispatcher

Public Class WmsServiceBehavior
    Implements IServiceBehavior

    Private ReadOnly _handler As IWmsBarcodeHandler

    Public Sub New(handler As IWmsBarcodeHandler)
        _handler = handler
    End Sub

    Public Sub AddBindingParameters(serviceDescription As ServiceDescription,
                                    serviceHostBase As ServiceHostBase,
                                    endpoints As ObjectModel.Collection(Of ServiceEndpoint),
                                    bindingParameters As BindingParameterCollection) Implements IServiceBehavior.AddBindingParameters
    End Sub

    Public Sub ApplyDispatchBehavior(serviceDescription As ServiceDescription,
                                     serviceHostBase As ServiceHostBase) Implements IServiceBehavior.ApplyDispatchBehavior
        For Each channelDispatcher As ChannelDispatcher In serviceHostBase.ChannelDispatchers
            For Each endpointDispatcher As EndpointDispatcher In channelDispatcher.Endpoints
                endpointDispatcher.DispatchRuntime.InstanceProvider = New WmsInstanceProvider(_handler)
            Next
        Next
    End Sub

    Public Sub Validate(serviceDescription As ServiceDescription,
                        serviceHostBase As ServiceHostBase) Implements IServiceBehavior.Validate
    End Sub
End Class
