Imports System.Configuration

Public Class WmsConfig

    Public Shared Function GetDispatchEndpoint() As String
        Return GetRequiredEndpoint("WmsDispatchEndpoint", True)
    End Function
End Class