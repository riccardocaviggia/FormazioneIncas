Imports System.ServiceProcess
Imports System.Text.Json
Imports System.Threading
Imports CommonSim

Public Class PlcService
    Inherits ServiceBase

    Private _channel As IPlcChannel
    Private _timer As Timer
    Private ReadOnly _sendLock As New Object()
    Private _barcodeProvider As IBarcodeProvider
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

        _barcodeProvider = New SequentialBarcodeProvider(contexts)

        Dim tcpClient = New PlcTcpClient(host, port)
        _channel = New PlcTcpChannel(tcpClient)
        _channel.Connect()

        _timer = New Timer(AddressOf SendBarcode, state:=Nothing, dueTime:=1000, period:=5000)
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

    Private Sub SendBarcode(state As Object)
        If _barcodeProvider Is Nothing OrElse _channel Is Nothing Then Return
        If Not Monitor.TryEnter(_sendLock) Then Return

        Dim message As WcsInboundMessage = Nothing

        Try
            Dim payload As String = _barcodeProvider.CreatePayload()
            message = JsonSerializer.Deserialize(Of WcsInboundMessage)(payload, _jsonOptions)

            Dim ack As String = _channel.Send(payload)

            _logger?.Info("PLC.BarcodeSent", correlationId:=message?.Id, barcode:=message?.Value, contextCode:=message?.ContextCode)
            _logger?.Info("PLC.AckReceived", correlationId:=message?.Id, barcode:=message?.Value, contextCode:=message?.ContextCode)
        Catch ex As Exception
            _logger?.[Error]("PLC.SendFailed", ex, correlationId:=message?.Id, barcode:=message?.Value, contextCode:=message?.ContextCode)
        Finally
            Monitor.Exit(_sendLock)
        End Try
    End Sub

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

        _barcodeProvider = Nothing
        _logger?.Info("PLC.Stopped")
    End Sub
End Class
