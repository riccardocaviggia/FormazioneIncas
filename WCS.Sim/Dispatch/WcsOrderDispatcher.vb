Imports System.Threading
Imports System.Threading.Tasks
Imports CommonSim

Public Class WcsOrderDispatcher
    Implements IDisposable

    Private ReadOnly _queue As WcsOrderQueue
    Private ReadOnly _channel As IOrderDeliveryChannel
    Private ReadOnly _logger As ServiceLogger

    Private _cts As CancellationTokenSource
    Private _worker As Task

    Public Sub New(queue As WcsOrderQueue,
                   channel As IOrderDeliveryChannel,
                   logger As ServiceLogger)
        If queue Is Nothing Then Throw New ArgumentNullException(NameOf(queue))
        _queue = queue
        If channel Is Nothing Then Throw New ArgumentNullException(NameOf(channel))
        _channel = channel
        _logger = logger
    End Sub

    '-------------------------------------------------------------------------------
    '- Worker in background che consuma la coda e invia gli ordini al canale PLC
    Public Sub Start()
        If _worker IsNot Nothing Then Throw New InvalidOperationException("Dispatcher already started.")
        _cts = New CancellationTokenSource()
        _worker = Task.Run(Function() RunAsync(_cts.Token))
    End Sub

    Private Async Function RunAsync(ct As CancellationToken) As Task
        Try
            For Each order In _queue.GetConsumingEnumerable(ct)
                ct.ThrowIfCancellationRequested()
                Try
                    Await _channel.SendAsync(order, ct).ConfigureAwait(False)
                Catch ex As Exception
                    _logger?.[Error]("WCS.OrderDispatchError", ex)
                End Try
            Next
        Catch ex As OperationCanceledException
        End Try
    End Function

    Public Sub [Stop]()
        If _worker Is Nothing Then Return

        _cts.Cancel()
        _queue.Complete()

        Try
            _worker.Wait(TimeSpan.FromSeconds(5))
        Catch
        End Try

        _worker = Nothing
        _cts.Dispose()
        _cts = Nothing
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        [Stop]()
    End Sub
End Class