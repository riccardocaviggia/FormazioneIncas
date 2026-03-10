Imports System.Collections.Generic
Imports System.Net
Imports System.Net.Http
Imports System.Threading
Imports System.Threading.Tasks
Imports CommonSim

Public Class HostService
    Private Const DefaultContextCode As String = "INBOUND"

    Private _logger As ServiceLogger
    Private _ordersPoller As OrderPollingService
    Private _ordersRepository As OrdersRepository
    Private _dispatchRepository As OrderDispatchRepository
    Private _wmsDispatchClient As WmsDispatchClient

    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim cs As String = ConnectionStringProvider.GetConnectionString(args)

        _logger = New ServiceLogger(cs, "HOST.Sim", Sub()
                                                    End Sub)

        _logger.Info("HOST.OnStart.START")

        '-------------------------------------------------------------------------------
        '- Webservice per passaggio ordini da HOST a Warehouse (WMS)
        _wmsDispatchClient = New WmsDispatchClient(WmsConfig.GetDispatchEndpoint())

        '-------------------------------------------------------------------------------
        '- Verifica che l'endpoint di dispatch sia raggiungibile prima di partire con il polling degli ordini,
        'altrimenti si rischia una race condition: l'HOST inizia a mandare i dati quando il WMS non è ancora pronto
        Dim deadline = DateTime.UtcNow.AddSeconds(30)
        Do
            Try
                Using req As New HttpRequestMessage(
                        HttpMethod.Head,
                        WmsConfig.GetDispatchEndpoint())
                    Dim handler As New HttpClientHandler() With {
                        .ServerCertificateCustomValidationCallback = Function(m, c, ch, e) True
                        }
                    Using client As New HttpClient(handler)
                        Dim res = client.SendAsync(req).GetAwaiter().GetResult()
                        If res.StatusCode = HttpStatusCode.MethodNotAllowed OrElse
                           res.StatusCode = HttpStatusCode.NotFound OrElse
                           res.IsSuccessStatusCode Then
                            Exit Do
                        End If
                    End Using
                End Using
            Catch
            End Try
            If DateTime.UtcNow > deadline Then Exit Do
            Threading.Thread.Sleep(500)
        Loop

        '-------------------------------------------------------------------------------
        '- TASK RICEZIONE ORDINI (prende gli ordini da Nortwind e li porta su Warehouse)
        'e poi li manda (ForwardOrders) al WMS tramite _wmsDispatchClient.
        _ordersRepository = New OrdersRepository(cs) 'cnn db Northwind
        _dispatchRepository = New OrderDispatchRepository(cs) 'cnn db Warehouse

        _ordersPoller = New OrderPollingService(
            _ordersRepository,
            HostConfig.GetOrdersPollingInterval(),
            HostConfig.GetOrdersBatchSize(),
            AddressOf ForwardOrders,
            _logger)
        _ordersPoller.Start()

        _logger.Info("HOST.OnStart.END")
    End Sub

    Protected Overrides Sub OnStop()
        _ordersPoller?.StopPolling()
        _ordersPoller = Nothing

        _logger?.Info("HOST.Stopped")
    End Sub

    '-------------------------------------------------------------------------------
    '- Crea dispatch per ogni ordine su WareHouseDb e costruisce i DTO
    Private Sub ForwardOrders(orders As IReadOnlyList(Of OrderRecord))
        If orders Is Nothing OrElse orders.Count = 0 Then Return

        Dim dispatchList As New List(Of DispatchOrderDto)(orders.Count)

        For Each order In orders
            Dim orderId = order.GetValue(Of Integer)("OrderID")
            Dim barcode = GenerateBarcode(orderId)
            Dim priority = CalculatePriority(order)
            Dim orderDate = order.GetValue(Of DateTime?)("OrderDate")
            Dim requiredDate = order.GetValue(Of DateTime?)("RequiredDate")

            'Inserimento su WareHouseDb (tabella OrderDispatches) con stato "Pending" (da processare) e recupero del DispatchId generato
            Dim dispatchId = _dispatchRepository.InsertDispatch(orderId, barcode, DefaultContextCode, priority, status:=OrderDispatchRepository.StatusPending)

            _logger?.Log("INFO", "HOST.DispatchCreated",
                         $"DispatchId={dispatchId};OrderID={orderId};Priority={priority}")

            dispatchList.Add(New DispatchOrderDto With {
                .DispatchId = dispatchId,
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

    '-------------------------------------------------------------------------------
    '- Retry backoff quando c'è l'invio di un batch: se dopo 3 tentativi il WMS non risponde, il batch viene marcato 'FAILED'
    Private Async Function SendBatchWithRetryAsync(batch As IReadOnlyList(Of DispatchOrderDto), attempts As Integer) As Task
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

        ' tutti i tentativi esauriti senza successo
        _logger?.Log("WARN", "HOST.DispatchBatchAbandoned", $"count={batch.Count}")
        MarkBatchFailed(batch)
    End Function

    '-------------------------------------------------------------------------------
    ' il batch viene marcato come 'FAILED'
    Private Sub MarkBatchFailed(batch As IReadOnlyList(Of DispatchOrderDto))
        For Each dto In batch
            Try
                _dispatchRepository.UpdateDispatch(dto.DispatchId, dto.Location, OrderDispatchRepository.StatusFailed)
            Catch ex As Exception
                _logger?.[Error]("HOST.DispatchMarkFailedError", ex)
            End Try
        Next
    End Sub
End Class
