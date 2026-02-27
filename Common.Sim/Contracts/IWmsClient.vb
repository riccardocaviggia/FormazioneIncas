Imports System.Threading

Public Interface IWmsClient
    Function ProcessBarcodeAsync(barcodeValue As String, contextCode As String, ct As CancellationToken) As Task(Of WmsResponse)
End Interface
