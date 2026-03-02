Imports System.Threading
Imports System.Threading.Tasks
Imports CommonSim

Public Class LoggingOrderDeliveryChannel
    Implements IOrderDeliveryChannel

    Private ReadOnly _logger As ServiceLogger

    Public Sub New(logger As ServiceLogger)
        _logger = logger
    End Sub

    Public Function SendAsync(order As DispatchOrderDto, ct As CancellationToken) As Task Implements IOrderDeliveryChannel.SendAsync
        _logger?.Info($"WCS.OrderQueuedForPlc[OrderID={order?.OrderId};Barcode={order?.Barcode};Location={order?.Location};Priority={order?.Priority}]")
        Return Task.CompletedTask
    End Function
End Class