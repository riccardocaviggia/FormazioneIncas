Imports System.Configuration

Public Class HostConfig
    Private Const DefaultOrdersBatchSize As Integer = 50
    Private Shared ReadOnly DefaultOrdersInterval As TimeSpan = TimeSpan.FromSeconds(10)

    Public Shared Function GetHostEndpoint() As String
        Dim endpoint = ConfigurationManager.AppSettings("HostEndpoint")
        If String.IsNullOrEmpty(endpoint) Then
            Throw New ConfigurationErrorsException("HostEndpoint is not configured in appSettings.")
        End If
        Return endpoint
    End Function

    Public Shared Function GetOrdersBatchSize() As Integer
        Return DefaultOrdersBatchSize
    End Function

    Public Shared Function GetOrdersPollingInterval() As TimeSpan
        Return DefaultOrdersInterval
    End Function
End Class
