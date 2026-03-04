Imports System.Collections.Generic
Imports System.Net.Http
Imports System.Threading
Imports System.Threading.Tasks
Imports CommonSim

Public Class HostService
    Private Const DefaultContextCode As String = "INBOUND"

    Private _server As HostHttpServer
    Private _logger As ServiceLogger
    Private _ordersPoller As OrderPollingService
    Private _dispatchRepository As OrderDispatchRepository
    Private _wmsDispatchClient As WmsDispatchClient

    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim cs As String = ConnectionStringProvider.GetConnectionString(args)
        Dim endpoint As String = HostConfig.GetHostEndpoint()
        Dim repository As New BarcodeRepository(cs)
        Dim authorizationService As IBarcodeAuthorizationService = New BarcodeAuthorizationService(repository)

        _logger = New ServiceLogger(cs, "HOST.Sim", Sub()
                                                    End Sub)

        _server = New HostHttpServer(endpoint, authorizationService, logger:=_logger)
        _server.Start()

        Dim ordersRepository As New OrdersRepository(cs)
        _dispatchRepository = New OrderDispatchRepository(cs)
        _wmsDispatchClient = New WmsDispatchClient(WmsConfig.GetDispatchEndpoint())

        _ordersPoller = New OrderPollingService(
            ordersRepository,
            HostConfig.GetOrdersPollingInterval(),
            HostConfig.GetOrdersBatchSize(),
            AddressOf ForwardOrders,
            _logger)
        _ordersPoller.Start()

        _logger.Info("HOST.Started")
    End Sub

    Protected Overrides Sub OnStop()
        _ordersPoller?.StopPolling()
        _ordersPoller = Nothing

        If _server IsNot Nothing Then
            _server.Stop()
            _server = Nothing
        End If

        _logger?.Info("HOST.Stopped")
    End Sub

    Private Sub ForwardOrders(orders As IReadOnlyList(Of OrderRecord))
        If orders Is Nothing OrElse orders.Count = 0 Then Return

        Dim dispatchList As New List(Of DispatchOrderDto)(orders.Count)

        For Each order In orders
            Dim orderId = order.GetValue(Of Integer)("OrderID")
            Dim barcode = GenerateBarcode(orderId)
            Dim priority = CalculatePriority(order)
            Dim orderDate = order.GetValue(Of DateTime?)("OrderDate")
            Dim requiredDate = order.GetValue(Of DateTime?)("RequiredDate")

            Dim dispatchId = _dispatchRepository.InsertDispatch(orderId, barcode, DefaultContextCode, priority)

            _logger?.Log("INFO", "HOST.DispatchCreated",
                         $"DispatchId={dispatchId};OrderID={orderId};Priority={priority}")

            dispatchList.Add(New DispatchOrderDto With {
                .OrderId = orderId,
                .Barcode = barcode,
                .ContextCode = DefaultContextCode,
                .Priority = priority,
                .Location = OrderDispatchRepository.DefaultLocation,
                .OrderDate = orderDate,
                .RequiredDate = requiredDate
            })
        Next

        If dispatchList.Count = 0 Then Return

        Task.Run(Async Function()
                     Try
                         Await SendBatchWithRetryAsync(dispatchList, 3)
                     Catch ex As Exception
                         _logger?.[Error]("HOST.DispatchBatchFailed", ex)
                     End Try
                 End Function)
    End Sub

    Private Shared Function GenerateBarcode(orderId As Integer) As String
        Return $"ORD{orderId:D8}"
    End Function

    Private Shared Function CalculatePriority(order As OrderRecord) As Integer
        Dim requiredDate As DateTime?
        If order.TryGetValue("RequiredDate", requiredDate) AndAlso requiredDate.HasValue Then
            Dim delta = requiredDate.Value.ToUniversalTime() - Date.UtcNow
            Return Math.Max(1, Math.Min(9999, CInt(Math.Ceiling(Math.Max(1, delta.TotalMinutes)))))
        End If

        Return 9999
    End Function

    Private Async Function SendBatchWithRetryAsync(batch As IReadOnlyList(Of DispatchOrderDto), attempts As Integer) As Task        ' retry backoff quando c'è l'invio di un batch
        For attempt = 1 To attempts
            Dim shouldDelay As Boolean = False

            Try
                Await _wmsDispatchClient.SendBatchAsync(batch, CancellationToken.None).ConfigureAwait(False)
                _logger?.Log("INFO", "HOST.DispatchBatchSent", $"count={batch.Count};attempt={attempt}")
                Return
            Catch ex As HttpRequestException When attempt < attempts
                shouldDelay = True
                _logger?.Log("INFO", "HOST.DispatchBatchRetry", $"attempt={attempt};error={ex.Message}")
            Catch ex As Exception
                _logger?.[Error]("HOST.DispatchBatchFailed", ex)
                Return
            End Try

            If shouldDelay Then
                Await Task.Delay(TimeSpan.FromSeconds(2 * attempt)).ConfigureAwait(False)
            End If
        Next
    End Function
End Class
