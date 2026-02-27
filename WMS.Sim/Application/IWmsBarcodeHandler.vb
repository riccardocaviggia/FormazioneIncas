Imports System.Threading
Imports CommonSim

Public Interface IWmsBarcodeHandler
    Function ProcessAsync(request As WmsRequest,
                          ct As CancellationToken) As Task(Of WmsResponse)
End Interface
