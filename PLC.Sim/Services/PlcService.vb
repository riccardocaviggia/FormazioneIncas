Imports System.ServiceProcess
Imports System.Threading
Imports CommonSim

Public Class PlcService
    Inherits ServiceBase

    Private _channel As IPlcChannel
    Private _timer As Timer
    Private ReadOnly _sendLock As New Object()
    Private _barcodeProvider As IBarcodeProvider

    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim host As String = TcpConfig.GetTcpHost()
        Dim port As Integer = TcpConfig.GetTcpPort()
        Dim connString As String = ConnectionStringProvider.GetConnectionString(args)

        Dim contexts = LoadContexts(connString)
        Console.WriteLine("[PLC] loaded " & contexts.Length & " contexts from Db")

        _barcodeProvider = New SequentialBarcodeProvider(contexts)

        Dim tcpClient = New PlcTcpClient(host, port)
        _channel = New PlcTcpChannel(tcpClient)
        _channel.Connect()

        _timer = New Timer(AddressOf SendBarcode, state:=Nothing, dueTime:=1000, period:=5000)
        Console.WriteLine("[PLC] Connected to WCS server at " & host & ":" & port)
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
            Try
                Dim payload As String = _barcodeProvider.CreatePayload()
                Dim ack As String = _channel.Send(payload)

                Console.WriteLine("[PLC] Sent: " & payload)
                Console.WriteLine("[PLC] Received ACK: " & ack)
            Catch ex As Exception
                Console.WriteLine("[PLC] ERROR: " & ex.Message)
            Finally
                Monitor.Exit(_sendLock)
            End Try
    End Sub

    Protected Overrides Sub OnStop()
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
    End Sub
End Class
