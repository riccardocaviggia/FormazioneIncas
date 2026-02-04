Public Class ClassStudent
    Private _name As String
    Private _surname As String
    Private _birthDate As ClassDate


    Public Property name As String
        Set(value As String)
            _name = value
        End Set
        Get
            Return _name
        End Get
    End Property

    Public Property surname As String
        Set(value As String)
            _surname = value
        End Set
        Get
            Return _surname
        End Get
    End Property

    Public Property BirthDate As ClassDate
        Set(value As ClassDate)
            _birthDate = value
        End Set
        Get
            Return _birthDate
        End Get
    End Property
    Public Sub New(n As String, s As String, b As ClassDate)
        _name = n
        _surname = s
        _birthDate = b
    End Sub

    Public Function StudentInfo() As String
        Return $"{name} {surname}{vbCrLf}Date of birth: {BirthDate.DateString}"
    End Function

End Class
