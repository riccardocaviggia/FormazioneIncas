Public Class Book
    Public Property BookID As Integer
    Public Property BookTitle As String
    Public Property BookISBN As String

    Public Overridable Property Authors As ICollection(Of Author) = New HashSet(Of Author)()
    Public Overridable Property Loans As ICollection(Of BookLoan) = New HashSet(Of BookLoan)()
End Class
