Public Class WcsConfig

    Public Shared Function GetDispatchEndpoint() As String
        Return GetRequiredEndpoint("WcsDispatchEndpoint", True)
    End Function
End Class