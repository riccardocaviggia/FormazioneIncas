Public Class ClassStudent
    Public Property name As String
    Public Property surname As String
    Public Property BirthDate As ClassDate

    Public Sub New(n As String, s As String, b As ClassDate)
        name = n
        surname = s
        BirthDate = b
    End Sub

    Public Function StudentInfo() As String
        Return $"{name} {surname}{vbCrLf}Date of birth: {BirthDate.DateString}"
    End Function

End Class
