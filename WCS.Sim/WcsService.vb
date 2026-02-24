Imports CommonSim
Imports System.ServiceProcess

Public Class WcsService
    Inherits ServiceBase

    Private _server As WcsTcpServer

    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim port As Integer = TcpConfig.GetTcpPort()
        Dim cs As String = ConnectionStringProvider.GetConnectionString(args)
        Dim contexts = LoadContexts(cs)

        Console.WriteLine("[WcsService] Loaded " & contexts.Count & " contexts from DB")

        _server = New WcsTcpServer(port, contexts, Sub(msg) Console.WriteLine("[WcsService] " & msg))
        _server.Start()
    End Sub

    Private Function LoadContexts(connectionString As String) As Dictionary(Of String, Boolean)
        Dim rows = DbHelper.ExecuteReader(
            connectionString,
            "SELECT ContextCode, IsEnabled FROM Contexts",
            Function(dr)
                Return New ContextRecord With {
                    .ContextCode = dr.GetString(0),
                    .IsEnabled = Convert.ToBoolean(dr.GetValue(1))
                }
            End Function)

        Dim dict As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        For Each row In rows
            dict(row.ContextCode.ToUpperInvariant()) = row.IsEnabled
            Console.WriteLine("[WcsService] Context loaded: " & row.ContextCode & " -> IsEnabled=" & row.IsEnabled)
        Next

        Return dict
    End Function

    Protected Overrides Sub OnStop()
        If _server IsNot Nothing Then
            _server.Stop()
            _server = Nothing
        End If
    End Sub

End Class
