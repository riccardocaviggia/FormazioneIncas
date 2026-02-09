Public Class User
    Public Property UserID As Integer
    Public Property UserName As String
    Public Property UserSurname As String
    Public Property UserEmail As String

    Public Overridable Property BookLoans As ICollection(Of BookLoan) = New HashSet(Of BookLoan)()
End Class
