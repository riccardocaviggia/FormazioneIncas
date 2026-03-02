Imports System.Threading
Imports System.Threading.Tasks

Public Interface IOrderDeliveryChannel
    Function SendAsync(order As DispatchOrderDto, ct As CancellationToken) As Task
End Interface