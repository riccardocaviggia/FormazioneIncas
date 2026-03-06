Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports CommonSim

Public Class WmsDispatchProcessor
    Private ReadOnly _allocator As LocationAllocator
    Private ReadOnly _wcsClient As WcsDispatchClient
    Private ReadOnly _logger As ServiceLogger
    Private ReadOnly _dispatchRepository As OrderDispatchRepository

    Public Sub New(allocator As LocationAllocator,
                   wcsClient As WcsDispatchClient,
                   dispatchRepository As OrderDispatchRepository,
                   logger As ServiceLogger)
        _allocator = allocator
        _wcsClient = wcsClient
        _dispatchRepository = dispatchRepository
        _logger = logger
    End Sub

    '-------------------------------------------------------------------------------
    '- Riceve il batch dall'HOST, assegna location e status e ordina per priorità. Invia il batch ordinato al WCS
    Public Async Function ProcessAsync(request As DispatchBatchRequest,
                                       ct As CancellationToken) As Task(Of DispatchBatchResponse)
        If request Is Nothing OrElse Not request.HasOrders() Then
            Return New DispatchBatchResponse With {
                .Succeeded = False,
                .Message = "Batch empty"
            }
        End If

        For Each order In request.Orders
            Dim needsAllocation =
                String.IsNullOrWhiteSpace(order.Location) OrElse
                String.Equals(order.Location,   ' così il placeholder Z0000 viene considerato come "non allocato"
                              OrderDispatchRepository.DefaultLocation,
                              StringComparison.OrdinalIgnoreCase)

            If needsAllocation Then
                order.Location = _allocator.NextLocation()
            End If

            If order.DispatchId <> Guid.Empty Then
                _dispatchRepository.UpdateDispatch(order.DispatchId,
                                                   order.Location,
                                                   OrderDispatchRepository.StatusInProgress)
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