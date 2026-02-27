Public Class PlcTcpChannel
    Implements IPlcChannel

    Private ReadOnly _client As PlcTcpClient

    Public Sub New(client As PlcTcpClient)
        If client Is Nothing Then Throw New ArgumentNullException(NameOf(client))
        _client = client
    End Sub

    Public Sub Connect() Implements IPlcChannel.Connect
        _client.ConnectOrThrow()
    End Sub

    Public Function Send(message As String) As String Implements IPlcChannel.Send
        Return _client.SendAndWaitAck(message)
    End Function

    Public Sub Disconnect() Implements IPlcChannel.Disconnect
        _client.Disconnect()
    End Sub
End Class