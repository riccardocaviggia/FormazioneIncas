Imports System.Threading

Public Interface IHostGateway
    Function CheckBarcodeAsync(barcodeValue As String, contextCode As String, ct As CancellationToken) As Task(Of BarcodeResponse)
End Interface
