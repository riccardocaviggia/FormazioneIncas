Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports CommonSim

Public Class WcsOrderQueue
    Implements IDisposable

    Private ReadOnly _collection As BlockingCollection(Of DispatchOrderDto)

    Public Sub New()
        _collection = New BlockingCollection(Of DispatchOrderDto)(New ConcurrentQueue(Of DispatchOrderDto)())
    End Sub

    Public Sub EnqueueRange(orders As IEnumerable(Of DispatchOrderDto))
        If orders Is Nothing Then Return
        For Each order In orders
            _collection.Add(order)
        Next
    End Sub

    Public Function GetConsumingEnumerable(ct As CancellationToken) As IEnumerable(Of DispatchOrderDto)
        Return _collection.GetConsumingEnumerable(ct)
    End Function

    Public Sub Complete()
        _collection.CompleteAdding()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        _collection.Dispose()
    End Sub
End Class