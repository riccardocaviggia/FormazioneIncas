Imports System.Configuration

Public Module TcpConfig
    Public Function GetTcpPort() As Integer
        Dim port As Integer
        If Integer.TryParse(ConfigurationManager.AppSettings("WcsTcpServerPort"), port) AndAlso port > 0 AndAlso port < 65536 Then
            Return port
        End If

        Throw New ConfigurationErrorsException("WcsTcpServerPort is not configured correctly. It must be a valid integer between 1 and 65535.")
    End Function

    Public Function GetTcpHost() As String
        Dim host = ConfigurationManager.AppSettings("WcsTcpServerHost")
        If String.IsNullOrEmpty(host) Then
            Throw New ConfigurationErrorsException("WcsTcpServerHost is not configured.")
        End If
        Return host.Trim()
    End Function
End Module
