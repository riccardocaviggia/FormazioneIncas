Imports System.IO
Imports System.ServiceProcess
Imports System.Threading
Imports CommonSim

Public Class PlcService
    Inherits ServiceBase

    Private _client As PlcTcpClient
    Private _timer As Timer
    Private _seq As Integer = 0 ' contatore per generare codici a barre univoci
    Private ReadOnly _sendLock As New Object()
    Private ReadOnly _contexts As String() = {"INBOUND", "OUTBOUND", "INVENTORY", "ERRORSIM"}

    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim host As String = TcpConfig.GetTcpHost()
        Dim port As Integer = TcpConfig.GetTcpPort()

        _client = New PlcTcpClient(host, port)
        _client.ConnectOrThrow()

        _timer = New Timer(AddressOf SendBarcode, state:=Nothing, dueTime:=1000, period:=5000) ' invia un barcode ogni 5 secondi dopo un ritardo iniziale di 1 secondo
        Console.WriteLine("[PLC] Connected to WCS server at " & host & ":" & port)

    End Sub

    Private Sub SendBarcode(state As Object)
        If Not Monitor.TryEnter(_sendLock) Then Return ' se il lock è già occupato (invio precedente) esce subito
        Try
            _seq += 1
            Dim barcodeValue As String = "BC" & _seq.ToString("00000000")
            Dim id As String = Guid.NewGuid().ToString("N")
            Dim contextCode As String = _contexts((_seq - 1) Mod _contexts.Length)
            Dim msq As String = "{""type"":""barcode"",""id"":""" & id & """,""value"":""" & barcodeValue & """, " & """contextCode"":""" & contextCode & """," & """ts"":""" & DateTime.UtcNow.ToString("o") & """}"
            Dim ack As String = _client.SendAndWaitAck(msq)

            Console.WriteLine("[PLC] Sent: " & msq)
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
        Catch ex As Exception
        End Try

        If _client IsNot Nothing Then
            _client.Disconnect()
            _client = Nothing
        End If
    End Sub

End Class
