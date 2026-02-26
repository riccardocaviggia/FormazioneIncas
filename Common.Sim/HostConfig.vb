Imports System.Configuration

Public Class HostConfig
    Public Shared Function GetHostEndpoint() As String
        Return ConfigurationManager.AppSettings("HostEndpoint")
    End Function
End Class
