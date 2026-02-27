Imports System.Threading
Imports CommonSim

Public Class WmsService
    Implements IWmsService

    Private ReadOnly _handler As IWmsBarcodeHandler

    Public Sub New()
    End Sub

    Public Sub New(handler As IWmsBarcodeHandler)
        _handler = handler
    End Sub

    Public Function ProcessBarcode(request As WmsRequest) As WmsResponse Implements IWmsService.ProcessBarcode
        If _handler Is Nothing Then
            Throw New InvalidOperationException("WmsService handler not configured. Check WmsInstanceProvider.")
        End If

        Return _handler.ProcessAsync(request, CancellationToken.None).GetAwaiter().GetResult()
    End Function
End Class