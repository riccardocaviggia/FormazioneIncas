Imports CommonSim

Public Class WmsService
    Implements IWmsService

    Private ReadOnly _hostClient As HostHttpClient

    Public Sub New()
    End Sub

    Public Sub New(hostClient As HostHttpClient)
        _hostClient = hostClient
    End Sub

    Public Function ProcessBarcode(request As WmsRequest) As WmsResponse Implements IWmsService.ProcessBarcode
        Console.WriteLine("[WMS] Received barcode: " & request.BarcodeValue & " context: " & request.ContextCode)

        Try
            Dim result = _hostClient.CheckBarcode(request.BarcodeValue, request.ContextCode)
            Console.WriteLine("[WMS] HOST response: Allowed=" & result.Allowed)

            Return New WmsResponse With {
                .Allowed = result.Allowed,
                .Reason = If(result.Allowed, "OK", "Barcode not authorized for this context")
            }
        Catch ex As Exception
            Console.WriteLine("[WMS] ERROR calling HOST: " & ex.Message)
            Return New WmsResponse With {
                .Allowed = False,
                .Reason = "HOST unreachable: " & ex.Message
            }
        End Try
    End Function

End Class