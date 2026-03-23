Public Class ServiceInfo
    Public Property ServiceName As String
    Public Property DisplayName As String

    Sub New(name As String, display As String)
        ServiceName = name
        DisplayName = display
    End Sub
End Class
