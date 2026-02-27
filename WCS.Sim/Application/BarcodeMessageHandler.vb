Imports System.Threading
Imports CommonSim

Public Class BarcodeMessageHandler
    Implements IWcsMessageHandler

    Private ReadOnly _wmsClient As IWmsClient
    Private ReadOnly _log As Action(Of String)

    Public Sub New(wmsClient As IWmsClient, Optional log As Action(Of String) = Nothing)
        _wmsClient = wmsClient
        _log = If(log, Sub()
                       End Sub)
    End Sub
    Public Async Function HandleAsync(message As WcsInboundMessage, ct As CancellationToken) As Task(Of AckMessage) Implements IWcsMessageHandler.HandleAsync
        If message Is Nothing Then Throw New ArgumentNullException(NameOf(message))

        Dim response = Await _wmsClient.ProcessBarcodeAsync(message.Value, message.ContextCode, ct)
        _log($"[Handler] WMS allowed={response.Allowed} Reason={response.Reason}")

        Return New AckMessage With {
            .Id = message.Id,
            .Ok = response.Allowed
        }
    End Function
End Class
