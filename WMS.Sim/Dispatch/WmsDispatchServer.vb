Imports System.Net
Imports System.Text
Imports System.Text.Json
Imports System.Threading
Imports System.Threading.Tasks
Imports CommonSim

Public Class WmsDispatchServer
    Implements IDisposable

    Private ReadOnly _prefix As String
    Private ReadOnly _processor As WmsDispatchProcessor
    Private ReadOnly _logger As ServiceLogger
    Private _listener As HttpListener
    Private _cts As CancellationTokenSource
    Private _listenerTask As Task

    Private Shared ReadOnly JsonOptions As New JsonSerializerOptions With {
        .PropertyNameCaseInsensitive = True
    }

    Public Sub New(prefix As String,
                   processor As WmsDispatchProcessor,
                   logger As ServiceLogger)
        If String.IsNullOrWhiteSpace(prefix) Then Throw New ArgumentNullException(NameOf(prefix))
        _prefix = prefix
        _processor = processor ?? Throw New ArgumentNullException(NameOf(processor))
        _logger = logger
    End Sub

    Public Sub Start()
        If _listener IsNot Nothing Then Throw New InvalidOperationException("Dispatch server already running.")

        _listener = New HttpListener()
        _listener.Prefixes.Add(_prefix)
        _listener.Start()

        _cts = New CancellationTokenSource()
        _listenerTask = Task.Run(Function() ListenAsync(_cts.Token))
        _logger?.Info($"WMS.DispatchServerStarted[{_prefix}]")
    End Sub

    Private Async Function ListenAsync(ct As CancellationToken) As Task
        While Not ct.IsCancellationRequested
            Dim context As HttpListenerContext = Nothing

            Try
                context = Await _listener.GetContextAsync().ConfigureAwait(False)
            Catch ex As HttpListenerException When ct.IsCancellationRequested
                Exit While
            Catch ex As Exception
                _logger?.[Error]("WMS.DispatchAcceptError", ex)
                Continue While
            End Try

            _ = Task.Run(Function() HandleAsync(context), ct)
        End While
    End Function

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
                Dim response = Await _processor.ProcessAsync(request, CancellationToken.None).ConfigureAwait(False)

                Dim json = JsonSerializer.Serialize(response, JsonOptions)
                Dim buffer = Encoding.UTF8.GetBytes(json)
                context.Response.ContentType = "application/json"
                context.Response.ContentLength64 = buffer.Length
                Await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(False)
            Catch ex As Exception
                _logger?.[Error]("WMS.DispatchHandleError", ex)
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
        _logger?.Info("WMS.DispatchServerStopped")
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        [Stop]()
    End Sub
End Class