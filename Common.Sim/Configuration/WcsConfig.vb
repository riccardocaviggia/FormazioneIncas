Imports System.Configuration.Install

Public Class WcsConfig

    Public Shared Function GetDispatchEndpoint() As String
        Return GetRequiredEndpoint("WcsDispatchEndpoint", True)
    End Function

    Public Shared Function GetServiceEndpoint() As String
        Return GetRequiredEndpoint("WcsServiceEndpoint", True)
    End Function

    Public Shared Function GetAuthUsername() As String
        Return GetRequiredSettings("WcsAuthUsername")
    End Function

    Public Shared Function GetAuthPassword() As String
        Return GetRequiredSettings("WcsAuthPassword")
    End Function
End Class