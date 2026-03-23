Imports System.Configuration

Public Module AppSettingsReader
    Public Function GetRequiredSettings(key As String) As String
        Dim value = ConfigurationManager.AppSettings(key)
        If String.IsNullOrWhiteSpace(value) Then
            Throw New ConfigurationErrorsException($"Missing required app setting: {key}")
        End If

        Return value.Trim()
    End Function

    Public Function GetRequiredEndpoint(key As String, ensureTrailingSlash As Boolean) As String
        Dim endpoint = GetRequiredSettings(key)
        If ensureTrailingSlash AndAlso Not endpoint.EndsWith("/", StringComparison.Ordinal) Then
            Return endpoint & "/"
        End If

        Return endpoint
    End Function
End Module
