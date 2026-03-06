Imports System.Collections.Generic
Imports System.Threading
Imports CommonSim

Public Class OrderPollingService
    Implements IDisposable

    Private ReadOnly _repository As OrdersRepository
    Private ReadOnly _interval As TimeSpan
    Private ReadOnly _batchSize As Integer
    Private ReadOnly _onBatch As Action(Of IReadOnlyList(Of OrderRecord))
    Private ReadOnly _logger As ServiceLogger
    Private _timer As Timer
    Private _status As Integer

    Public Sub New(repository As OrdersRepository,
                   interval As TimeSpan,
                   batchSize As Integer,
                   onBatch As Action(Of IReadOnlyList(Of OrderRecord)),
                   Optional logger As ServiceLogger = Nothing)

        If repository Is Nothing Then Throw New ArgumentNullException(NameOf(repository))
        If interval <= TimeSpan.Zero Then Throw New ArgumentOutOfRangeException(NameOf(interval))
        If batchSize <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(batchSize))
        If onBatch Is Nothing Then Throw New ArgumentNullException(NameOf(onBatch))

        _repository = repository
        _interval = interval
        _batchSize = batchSize
        _onBatch = onBatch
        _logger = logger
    End Sub

    Public Sub Start()
        If _timer IsNot Nothing Then Throw New InvalidOperationException("OrderPollingService already running.")
        _timer = New Timer(AddressOf Poll, Nothing, TimeSpan.Zero, _interval)
    End Sub

    Public Sub StopPolling()
        _timer?.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan)
        _timer?.Dispose()
        _timer = Nothing
    End Sub

    Private Sub Poll(state As Object)
        If Interlocked.Exchange(_status, 1) = 1 Then Return

        Try
            Dim orders = _repository.FetchBatch(_batchSize)
            If orders.Count > 0 Then
                _logger?.Log("INFO", "HOST.OrdersFetched", $"Batch={orders.Count}")
                _onBatch(orders) 'ForwardOrders
            End If
        Catch ex As Exception
            _logger?.[Error]("HOST.OrderPollingError", ex)
        Finally
            Interlocked.Exchange(_status, 0)
        End Try
    End Sub

    Public Overloads Sub Dispose() Implements IDisposable.Dispose
        StopPolling()
    End Sub
End Class
