Imports CommonSim

Public Class BarcodeAuthorizationService
    Implements IBarcodeAuthorizationService

    Private ReadOnly _repository As BarcodeRepository

    Public Sub New(repository As BarcodeRepository)
        If repository Is Nothing Then Throw New ArgumentNullException(NameOf(repository))
        _repository = repository
    End Sub

    Public Function IsAuthorized(barcodeValue As String, contextCode As String) As Boolean _
        Implements IBarcodeAuthorizationService.IsAuthorized
        Return _repository.IsAuthorized(barcodeValue, contextCode)
    End Function
End Class