Imports System.Configuration

Public Module TcpConfig
    Public Function GetPlcPort() As Integer
        Dim port As Integer
        If Integer.TryParse(ConfigurationManager.AppSettings("PlcTcpPort"), port) AndAlso port > 0 AndAlso port < 65536 Then
            Return port
        End If

        Throw New ConfigurationErrorsException("PlcTcpPort is not configured. It must be an integer between 1 and 65535.")
    End Function

    Public Function GetPlcHost() As String
        Dim host = ConfigurationManager.AppSettings("PlcTcpHost")
        If String.IsNullOrEmpty(host) Then
            Throw New ConfigurationErrorsException("PlcTcpHost is not configured.")
        End If
        Return host.Trim()
    End Function
End Module
