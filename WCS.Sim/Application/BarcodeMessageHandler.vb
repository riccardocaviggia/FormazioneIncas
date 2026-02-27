Imports System.Threading
Imports CommonSim

Public Class BarcodeMessageHandler
    Implements IWcsMessageHandler

    Private ReadOnly _wmsClient As IWmsClient
    Private ReadOnly _logger As ServiceLogger

    Public Sub New(wmsClient As IWmsClient, Optional logger As ServiceLogger = Nothing)
        _wmsClient = wmsClient
        _logger = logger
    End Sub

    Public Async Function HandleAsync(message As WcsInboundMessage, ct As CancellationToken) As Task(Of AckMessage) Implements IWcsMessageHandler.HandleAsync
        If message Is Nothing Then Throw New ArgumentNullException(NameOf(message))

        _logger?.Info("WCS.CallWms", correlationId:=message.Id, barcode:=message.Value, contextCode:=message.ContextCode)

        Try
            Dim response = Await _wmsClient.ProcessBarcodeAsync(message.Value, message.ContextCode, ct)
            _logger?.Info("WCS.WmsResponse", correlationId:=message.Id, barcode:=message.Value, contextCode:=message.ContextCode)

            Return New AckMessage With {
                .Id = message.Id,
                .Ok = response.Allowed
            }
        Catch ex As Exception
            _logger?.[Error]("WCS.WmsCallFailed", ex, correlationId:=message.Id, barcode:=message.Value, contextCode:=message.ContextCode)
            Throw
        End Try
    End Function
End Class
