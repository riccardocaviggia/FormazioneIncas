Public Class Author
    Public Property AuthorID As Integer
    Public Property AuthorName As String
    Public Property AuthorSurname As String

    Public Overridable Property Books As ICollection(Of Book) = New HashSet(Of Book)()

End Class
