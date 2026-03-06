Imports System.Configuration

Public Class HostConfig
    Private Const DefaultOrdersBatchSize As Integer = 50
    Private Shared ReadOnly DefaultOrdersInterval As TimeSpan = TimeSpan.FromSeconds(10)

    Public Shared Function GetOrdersBatchSize() As Integer
        Return DefaultOrdersBatchSize
    End Function

    Public Shared Function GetOrdersPollingInterval() As TimeSpan
        Return DefaultOrdersInterval
    End Function
End Class
