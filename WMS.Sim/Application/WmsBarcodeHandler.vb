Imports System.Threading
Imports CommonSim

Public Class WmsBarcodeHandler
    Implements IWmsBarcodeHandler

    Private ReadOnly _hostGateway As IHostGateway
    Private ReadOnly _logger As ServiceLogger

    Public Sub New(hostGateway As IHostGateway, Optional logger As ServiceLogger = Nothing)
        If hostGateway Is Nothing Then Throw New ArgumentNullException(NameOf(hostGateway))
        _hostGateway = hostGateway
        _logger = logger
    End Sub

    Public Async Function ProcessAsync(request As WmsRequest,
                                       ct As CancellationToken) As Task(Of WmsResponse) _
                                       Implements IWmsBarcodeHandler.ProcessAsync
        If request Is Nothing Then Throw New ArgumentNullException(NameOf(request))

        _logger?.Info("WMS.RequestReceived", barcode:=request.BarcodeValue, contextCode:=request.ContextCode)

        Try
            Dim result = Await _hostGateway.CheckBarcodeAsync(request.BarcodeValue, request.ContextCode, ct)
            _logger?.Info("WMS.HostResponse", barcode:=request.BarcodeValue, contextCode:=request.ContextCode)

            Return New WmsResponse With {
                .Allowed = result.Allowed,
                .Reason = If(result.Allowed, "OK", "Barcode not authorized for this context")
            }
        Catch ex As Exception
            _logger?.[Error]("WMS.HostError", ex, barcode:=request.BarcodeValue, contextCode:=request.ContextCode)

            Return New WmsResponse With {
                .Allowed = False,
                .Reason = "HOST unreachable: " & ex.Message
            }
        End Try
    End Function
End Class