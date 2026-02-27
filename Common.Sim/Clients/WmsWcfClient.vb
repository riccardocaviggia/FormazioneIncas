Imports System.ServiceModel
Imports System.Threading

Public Class WmsWcfClient
    Implements IWmsClient
    Private ReadOnly _endpointAddress As String

    Public Sub New(endpointAddress As String)
        _endpointAddress = endpointAddress
    End Sub

    Public Function ProcessBarcodeAsync(barcodeValue As String, contextCode As String, ct As CancellationToken) As Task(Of WmsResponse) Implements IWmsClient.ProcessBarcodeAsync
        Return Task.Run(Function()
                            ct.ThrowIfCancellationRequested()
                            Return ProcessBarcode(barcodeValue, contextCode)
                        End Function, ct)
    End Function

    Private Function ProcessBarcode(barcodeValue As String, contextCode As String) As WmsResponse
        Dim binding As New BasicHttpBinding()
        Dim endpoint As New EndpointAddress(_endpointAddress)
        Dim factory As New ChannelFactory(Of IWmsService)(binding, endpoint)
        Dim channel As IWmsService = factory.CreateChannel()

        Try
            Dim request As New WmsRequest With {
                .BarcodeValue = barcodeValue,
                .ContextCode = contextCode
            }
            Return channel.ProcessBarcode(request)
        Finally
            Try
                CType(channel, ICommunicationObject).Close()
            Catch
                CType(channel, ICommunicationObject).Abort()
            End Try
            factory.Close()
        End Try
    End Function
End Class
