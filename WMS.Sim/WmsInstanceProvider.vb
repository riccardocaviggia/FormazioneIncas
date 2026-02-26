Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Dispatcher
Imports CommonSim

Public Class WmsInstanceProvider
    Implements IInstanceProvider

    Private ReadOnly _hostClient As HostHttpClient

    Public Sub New(hostClient As HostHttpClient)
        _hostClient = hostClient
    End Sub

    Public Function GetInstance(instanceContext As InstanceContext) As Object Implements IInstanceProvider.GetInstance
        Return New WmsService(_hostClient)
    End Function

    Public Function GetInstance(instanceContext As InstanceContext, message As Message) As Object Implements IInstanceProvider.GetInstance
        Return New WmsService(_hostClient)
    End Function

    Public Sub ReleaseInstance(instanceContext As InstanceContext, instance As Object) Implements IInstanceProvider.ReleaseInstance
        ' nulla da fare
    End Sub

End Class
