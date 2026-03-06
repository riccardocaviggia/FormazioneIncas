Imports System.ServiceProcess
Imports System.Text.Json
Imports System.Threading
Imports CommonSim

Public Class PlcService
    Inherits ServiceBase

    Private _channel As IPlcChannel
    Private _timer As Timer
    Private ReadOnly _sendLock As New Object()
    Private _logger As ServiceLogger

    Private Shared ReadOnly _jsonOptions As New JsonSerializerOptions() With {
        .PropertyNameCaseInsensitive = True
    }

    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim host As String = TcpConfig.GetTcpHost()
        Dim port As Integer = TcpConfig.GetTcpPort()
        Dim connString As String = ConnectionStringProvider.GetConnectionString(args)

        _logger = New ServiceLogger(connString, "PLC.Sim", Sub()
                                                           End Sub)
        _logger.Info("PLC.Starting")

        Dim contexts = LoadContexts(connString)
        _logger.Info($"PLC.ContextsLoaded[items={contexts.Length}]")

        Dim tcpClient = New PlcTcpClient(host, port)
        _channel = New PlcTcpChannel(tcpClient)
        _channel.Connect()

        _logger.Info("PLC.ConnectedToWcs")
    End Sub

    Private Function LoadContexts(connectionString As String) As String()
        Dim rows = DbHelper.ExecuteReader(
            connectionString,
            "SELECT ContextCode FROM Contexts WHERE IsEnabled = 1",
            Function(dr) dr.GetString(0)
        )
        Return rows.ToArray()
    End Function


    Protected Overrides Sub OnStop()
        _logger?.Info("PLC.Stopping")

        Try
            If _timer IsNot Nothing Then
                _timer.Dispose()
                _timer = Nothing
            End If
        Catch
        End Try

        If _channel IsNot Nothing Then
            _channel.Disconnect()
            _channel = Nothing
        End If

        _logger?.Info("PLC.Stopped")
    End Sub
End Class
