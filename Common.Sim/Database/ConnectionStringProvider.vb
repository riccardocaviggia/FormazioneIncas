Imports System.Configuration

Public Module ConnectionStringProvider
    Public Function GetConnectionString(args As String(), Optional defaultName As String = "WareHouseDB") As String
        Dim csName = defaultName
        If args IsNot Nothing Then
            For Each arg In args
                If arg IsNot Nothing AndAlso arg.StartsWith("--csName=", StringComparison.OrdinalIgnoreCase) Then
                    csName = arg.Substring("--csName=".Length).Trim()
                End If
            Next
        End If
        Dim cs = ConfigurationManager.ConnectionStrings(csName)
        If cs Is Nothing OrElse String.IsNullOrWhiteSpace(cs.ConnectionString) Then
            Throw New InvalidOperationException("Connection string '" & csName & "' not found in configuration.")
        End If

        Return cs.ConnectionString
    End Function
End Module
