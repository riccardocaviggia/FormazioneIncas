Imports System.Threading
Imports System.Threading.Tasks
Imports CommonSim

Public Class WmsBarcodeHandler
    Implements IWmsBarcodeHandler

    Private ReadOnly _hostGateway As IHostGateway
    Private ReadOnly _log As Action(Of String)

    Public Sub New(hostGateway As IHostGateway, Optional log As Action(Of String) = Nothing)
        If hostGateway Is Nothing Then Throw New ArgumentNullException(NameOf(hostGateway))
        _hostGateway = hostGateway
        _log = If(log, Sub()
                       End Sub)
    End Sub

    Public Async Function ProcessAsync(request As WmsRequest,
                                       ct As CancellationToken) As Task(Of WmsResponse) _
                                       Implements IWmsBarcodeHandler.ProcessAsync
        If request Is Nothing Then Throw New ArgumentNullException(NameOf(request))

        _log($"[WMS] Received barcode: {request.BarcodeValue} context: {request.ContextCode}")

        Try
            Dim result = Await _hostGateway.CheckBarcodeAsync(request.BarcodeValue, request.ContextCode, ct)
            _log($"[WMS] HOST response: Allowed={result.Allowed}")

            Return New WmsResponse With {
                .Allowed = result.Allowed,
                .Reason = If(result.Allowed, "OK", "Barcode not authorized for this context")
            }
        Catch ex As Exception
            _log($"[WMS] ERROR calling HOST: {ex.Message}")
            Return New WmsResponse With {
                .Allowed = False,
                .Reason = "HOST unreachable: " & ex.Message
            }
        End Try
    End Function
End Class