Imports System.Configuration

Public Class WmsConfig
    Public Shared Function GetWmsEndpoint() As String
        Return ConfigurationManager.AppSettings("WmsEndpoint")
    End Function
End Class
