Public Interface IPlcChannel
    Sub Connect()
    Function Send(message As String) As String
    Sub Disconnect()
End Interface