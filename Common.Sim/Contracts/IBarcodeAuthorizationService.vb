Public Interface IBarcodeAuthorizationService
    Function IsAuthorized(barcodeValue As String, contextCode As String) As Boolean
End Interface