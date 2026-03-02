Imports System.Configuration

Public Class WmsConfig
    Private Const DefaultServiceEndpoint As String = "https://localhost:8081/WmsService"
    Private Const DefaultDispatchEndpoint As String = "https://localhost:8443/wms/dispatch/"

    Public Shared Function GetWmsEndpoint() As String
        Dim value = ConfigurationManager.AppSettings("WmsEndpoint")
        Return If(String.IsNullOrWhiteSpace(value), DefaultServiceEndpoint, value)
    End Function

    Public Shared Function GetDispatchEndpoint() As String
        Dim value = ConfigurationManager.AppSettings("WmsDispatchEndpoint")
        Return NormalizeEndpoint(If(String.IsNullOrWhiteSpace(value), DefaultDispatchEndpoint, value))
    End Function

    Private Shared Function NormalizeEndpoint(endpoint As String) As String
        If endpoint.EndsWith("/", StringComparison.Ordinal) Then Return endpoint
        Return endpoint & "/"
    End Function
End Class
