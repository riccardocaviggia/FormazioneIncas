Imports System.Configuration

Public Class WmsConfig

    Public Shared Function GetDispatchEndpoint() As String
        Return GetRequiredEndpoint("WmsDispatchEndpoint", True)
    End Function

    Public Shared Function GetServiceEndpoint() As String
        Return GetRequiredEndpoint("WmsServiceEndpoint", True)
    End Function

    Public Shared Function GetAuthUsername() As String
        Return GetRequiredSettings("WmsAuthUsername")
    End Function

    Public Shared Function GetAuthPassword() As String
        Return GetRequiredSettings("WmsAuthPassword")
    End Function
End Class