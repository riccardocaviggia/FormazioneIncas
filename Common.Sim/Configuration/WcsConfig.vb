Imports System.Configuration

Public Class WcsConfig
    Private Const DefaultDispatchEndpoint As String = "https://localhost:9443/wcs/dispatch/"

    Public Shared Function GetDispatchEndpoint() As String
        Dim value = ConfigurationManager.AppSettings("WcsDispatchEndpoint")
        Return NormalizeEndpoint(If(String.IsNullOrWhiteSpace(value), DefaultDispatchEndpoint, value))
    End Function

    Private Shared Function NormalizeEndpoint(endpoint As String) As String
        If endpoint.EndsWith("/", StringComparison.Ordinal) Then Return endpoint
        Return endpoint & "/"
    End Function
End Class