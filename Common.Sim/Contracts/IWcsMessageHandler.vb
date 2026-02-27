Imports System.Threading

Public Interface IWcsMessageHandler
    Function HandleAsync(message As WcsInboundMessage, ct As CancellationToken) As Task(Of AckMessage)
End Interface
