Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports CommonSim

Public Class WmsDispatchProcessor
    Private ReadOnly _allocator As LocationAllocator
    Private ReadOnly _wcsClient As WcsDispatchClient
    Private ReadOnly _logger As ServiceLogger

    Public Sub New(allocator As LocationAllocator,
                   wcsClient As WcsDispatchClient,
                   logger As ServiceLogger)
        If allocator Is Nothing Then Throw New ArgumentNullException(NameOf(allocator))
        _allocator = allocator
        If wcsClient Is Nothing Then Throw New ArgumentNullException(NameOf(wcsClient))
        _wcsClient = wcsClient
        _logger = logger
    End Sub

    Public Async Function ProcessAsync(request As DispatchBatchRequest,
                                       ct As CancellationToken) As Task(Of DispatchBatchResponse)
        If request Is Nothing OrElse Not request.HasOrders() Then
            Return New DispatchBatchResponse With {
                .Succeeded = False,
                .Message = "Batch vuoto"
            }
        End If

        For Each order In request.Orders
            If String.IsNullOrWhiteSpace(order.Location) Then
                order.Location = _allocator.NextLocation()
            End If
        Next

        Dim ordered = request.Orders _
            .OrderBy(Function(o) o.Priority) _
            .ThenBy(Function(o) o.OrderDate.GetValueOrDefault(Date.MinValue)) _
            .ThenBy(Function(o) o.OrderId) _
            .ToList()

        Await _wcsClient.SendAsync(ordered, ct).ConfigureAwait(False)

        _logger?.Info($"WMS.DispatchBatchForwarded[count={ordered.Count}]")

        Return New DispatchBatchResponse With {
            .Succeeded = True,
            .Message = $"Ordini inoltrati al WCS: {ordered.Count}"
        }
    End Function
End Class