Imports System.Collections.Generic
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Net.Sockets
Imports System.Threading
Imports System.Threading.Tasks
Imports CommonSim

Public Class HostService
    Private Const DefaultContextCode As String = "INBOUND"

    Private _logger As ServiceLogger
    Private _ordersPoller As OrderPollingService
    Private _completePoller As OrderPollingService
    Private _ordersRepository As OrdersRepository
    Private _dispatchRepository As OrderDispatchRepository
    Private _wmsDispatchClient As WmsDispatchClient

    Protected Overrides Async Sub OnStart(ByVal args() As String)
        Dim cs As String = ConnectionStringProvider.GetConnectionString(args)

        _logger = New ServiceLogger(cs, "HOST.Sim", Sub()
                                                    End Sub)
        _logger.Info("OnStart")

        '-------------------------------------------------------------------------------
        '- Webservice per passaggio ordini da HOST a Warehouse (WMS)
        _wmsDispatchClient = New WmsDispatchClient(
            WmsConfig.GetDispatchEndpoint(),
            WmsConfig.GetAuthUsername(),
            WmsConfig.GetAuthPassword(),
            _logger)

        '-------------------------------------------------------------------------------
        '- Attende che WMS, WCS e PLC siano avviati prima di iniziare a prendere gli ordini da Northwind e mandarli al WMS
        Await WaitForAllServicesReadyAsync()

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

        '-------------------------------------------------------------------------------
        '- POLLER che prende gli ordini COMPLETED da OrderDispatches e li marca come ARCHIVED
        ' Uso questa modalità di polling perchè l'HOST non ha un server per ricevere l'ACK
        ' di completamento ordine da WMS.
        _completePoller = New OrderPollingService(
            _dispatchRepository,
            TimeSpan.FromSeconds(5),
            50,
            AddressOf OnOrdersCompleted,
            _logger)
        _completePoller.Start()

        _logger.Info("HOST.OnStart.END")
    End Sub

    Protected Overrides Sub OnStop()
        Try
            _logger.Info("OnStop")

            _ordersPoller?.StopPolling()
            _ordersPoller = Nothing

            _logger?.Info("HOST.Stopped")
        Catch ex As Exception
            _logger?.[Error]("HOST.StopError", ex)
        End Try

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
                _dispatchRepository.UpdateDispatch(dto.OrderId, dto.Location, OrderDispatchRepository.StatusFailed)
            Catch ex As Exception
                _logger?.[Error]("HOST.DispatchMarkFailedError", ex)
            End Try
        Next
    End Sub

    '-------------------------------------------------------------------------------
    '- Funzione che controlla che WMS, WCS e PLC siano avviati
    Private Async Function WaitForAllServicesReadyAsync() As Task
        Dim endpoints = {WmsConfig.GetServiceEndpoint(), WcsConfig.GetServiceEndpoint()}
        Dim servicesReady As Boolean = False

        While Not servicesReady
            servicesReady = True

            '- Controlla WMS e WCS tramite chiamata HTTP GET all'endpoint /ready (devono rispondere con 200 OK)
            For Each url In endpoints

                Try
                    Using client As New HttpClient()
                        Dim res = Await client.GetAsync(url)
                        If res.StatusCode = HttpStatusCode.OK Then
                            If url.Contains("wms") Then
                                _logger?.Log("INFO", "WMS.StartResponse", $"WMS.ready = {res.StatusCode}")
                            Else _logger?.Log("INFO", "WCS.StartResponse", $"WCS.ready = {res.StatusCode}")
                            End If
                        End If
                        If Not res.IsSuccessStatusCode Then
                            servicesReady = False
                            Exit For
                        End If
                    End Using
                Catch
                    servicesReady = False
                    Exit For
                End Try
            Next

            '- Controlla PLC aprendo una connessione TCP, inviando "STATUS" e aspettando "OK"
            If servicesReady Then
                Try
                    Using tcp = New TcpClient()
                        Await tcp.ConnectAsync(TcpConfig.GetPlcHost, TcpConfig.GetPlcPort)
                        Using stream = tcp.GetStream()
                            Dim writer As New StreamWriter(stream) With {.AutoFlush = True}
                            Dim reader As New StreamReader(stream)
                            Await writer.WriteLineAsync("STATUS")
                            Dim response = Await reader.ReadLineAsync()
                            If response Is Nothing OrElse response.Trim().ToUpper() <> "OK" Then
                                servicesReady = False
                            Else
                                _logger?.Log("INFO", "PLC.StartResponse", $"PLC.ready={response}")
                            End If
                        End Using
                    End Using
                Catch
                    servicesReady = False
                End Try
            End If

            If Not servicesReady Then Await Task.Delay(1000)
        End While
    End Function

    Private Sub OnOrdersCompleted(dispatches As IReadOnlyList(Of OrderRecord))
        For Each dispatch In dispatches
            Dim orderId = dispatch.GetValue(Of Integer)("OrderId")
            Dim location = dispatch.GetValue(Of String)("Location")

            Dim orderIdStr = orderId.ToString()
            Try
                _dispatchRepository.UpdateDispatch(orderId.ToString(), location, OrderDispatchRepository.StatusArchived)
                _logger?.Log("INFO", "HOST.OrderCompleted", $"OrderID={orderId};Status={OrderDispatchRepository.StatusArchived}")
            Catch ex As Exception
                _logger?.[Error]("HOST.MarkCompleteError", ex)
            End Try
        Next
    End Sub
End Class
