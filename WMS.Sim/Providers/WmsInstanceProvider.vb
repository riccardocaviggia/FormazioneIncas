Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Dispatcher

Public Class WmsInstanceProvider
    Implements IInstanceProvider

    Private ReadOnly _handler As IWmsBarcodeHandler

    Public Sub New(handler As IWmsBarcodeHandler)
        _handler = handler
    End Sub

    Public Function GetInstance(instanceContext As InstanceContext) As Object Implements IInstanceProvider.GetInstance
        Return New WmsService(_handler)
    End Function

    Public Function GetInstance(instanceContext As InstanceContext, message As Message) As Object Implements IInstanceProvider.GetInstance
        Return New WmsService(_handler)
    End Function

    Public Sub ReleaseInstance(instanceContext As InstanceContext, instance As Object) Implements IInstanceProvider.ReleaseInstance
    End Sub
End Class
