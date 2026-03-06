Imports System.Net
Imports System.Text
Imports System.Text.Json
Imports System.Threading
Imports System.Threading.Tasks
Imports System.IO
Imports CommonSim

Public Class WcsDispatchServer
    Implements IDisposable

    Private ReadOnly _prefix As String
    Private ReadOnly _queue As WcsOrderQueue
    Private ReadOnly _logger As ServiceLogger

    Private _listener As HttpListener
    Private _cts As CancellationTokenSource
    Private _listenerTask As Task

    Private Shared ReadOnly JsonOptions As New JsonSerializerOptions With {
        .PropertyNameCaseInsensitive = True
    }

    Public Sub New(prefix As String,
                   queue As WcsOrderQueue,
                   logger As ServiceLogger)
        If String.IsNullOrWhiteSpace(prefix) Then Throw New ArgumentNullException(NameOf(prefix))
        _prefix = prefix
        If queue Is Nothing Then Throw New ArgumentNullException(NameOf(queue))
        _queue = queue
        _logger = logger
    End Sub

    Public Sub Start()
        If _listener IsNot Nothing Then Throw New InvalidOperationException("WCS dispatch server already running.")

        _listener = New HttpListener()
        Dim listenerPrefix = _prefix.Replace("://localhost", "://+")
        _listener.Prefixes.Add(listenerPrefix)
        _listener.Start()

        _cts = New CancellationTokenSource()
        _listenerTask = Task.Run(Function() ListenAsync(_cts.Token))
        _logger?.Info($"WCS.DispatchServerStarted[{_prefix}]")
    End Sub

    '-------------------------------------------------------------------------------
    '- LOOP DI ASCOLTO: accetta le richieste in arrivo e le gestisce HandleSync, esce dal loop se è richiesto lo stop
    Private Async Function ListenAsync(ct As CancellationToken) As Task
        While Not ct.IsCancellationRequested
            Dim context As HttpListenerContext = Nothing

            Try
                context = Await _listener.GetContextAsync().ConfigureAwait(False)
            Catch ex As HttpListenerException When ct.IsCancellationRequested
                Exit While
            Catch ex As Exception
                _logger?.[Error]("WCS.DispatchAcceptError", ex)
                Continue While
            End Try

            Await Task.Run(Function() HandleAsync(context), ct)
        End While
    End Function


    '-------------------------------------------------------------------------------
    '- GESTIONE RICHIESTA: se è una POST a /dispatch, legge il body, deserializza e mette gli ordini in coda. Altrimenti risponde con 404
    Private Async Function HandleAsync(context As HttpListenerContext) As Task
        Using context.Response
            If context.Request.HttpMethod <> "POST" OrElse
               Not context.Request.Url.AbsolutePath.TrimEnd("/"c).EndsWith("/dispatch", StringComparison.OrdinalIgnoreCase) Then
                context.Response.StatusCode = CInt(HttpStatusCode.NotFound)
                Return
            End If

            Try
                Dim body As String
                Using reader As New StreamReader(context.Request.InputStream, Encoding.UTF8)
                    body = Await reader.ReadToEndAsync().ConfigureAwait(False)
                End Using

                Dim request = JsonSerializer.Deserialize(Of DispatchBatchRequest)(body, JsonOptions)
                If request IsNot Nothing AndAlso request.HasOrders() Then
                    _queue.EnqueueRange(request.Orders)
                End If

                context.Response.StatusCode = CInt(HttpStatusCode.Accepted)
            Catch ex As Exception
                _logger?.[Error]("WCS.DispatchHandleError", ex)
                context.Response.StatusCode = CInt(HttpStatusCode.InternalServerError)
            End Try
        End Using
    End Function

    Public Sub [Stop]()
        Try
            _cts?.Cancel()
            _listener?.Stop()
        Catch
        End Try

        Try
            _listenerTask?.Wait(TimeSpan.FromSeconds(5))
        Catch
        End Try

        _listener = Nothing
        _listenerTask = Nothing
        _cts = Nothing
        _logger?.Info("WCS.DispatchServerStopped")
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        [Stop]()
    End Sub
End Class