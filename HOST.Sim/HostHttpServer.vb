Imports System.Net
Imports System.Text
Imports System.Text.Json
Imports System.Threading
Imports CommonSim

Public Class HostHttpServer
    Private ReadOnly _endpoint As String
    Private ReadOnly _repository As BarcodeRepository
    Private ReadOnly _log As Action(Of String)
    Private _listener As HttpListener
    Private _listenerThread As Thread
    Private _cts As CancellationTokenSource

    Public Sub New(endpoint As String, repository As BarcodeRepository, Optional log As Action(Of String) = Nothing)
        _endpoint = endpoint
        _repository = repository
        _log = If(log, Sub(msg)
                       End Sub)
    End Sub

    Public Sub Start()
        If _listener IsNot Nothing Then
            Throw New InvalidOperationException("Server is already running.")
        End If

        _cts = New CancellationTokenSource()
        _listener = New HttpListener()
        _listener.Prefixes.Add(_endpoint)
        _listener.Start()

        _log("HostHttpServer started. Listening on " & _endpoint)

        _listenerThread = New Thread(Sub() ListenLoop(_cts.Token)) With {
            .IsBackground = True,
            .Name = "HostHttpServer.ListenLoop"
        }
        _listenerThread.Start()
    End Sub

    Public Sub [Stop]()
        _log("HostHttpServer stopping...")

        Try
            If _cts IsNot Nothing Then _cts.Cancel()
        Catch ex As Exception
            _log("Error cancelling token: " & ex.ToString())
        End Try

        Try
            If _listener IsNot Nothing Then _listener.Stop()
        Catch ex As Exception
            _log("Error stoppung listener: " & ex.ToString())
        End Try

        Try
            If _listenerThread IsNot Nothing AndAlso _listenerThread.IsAlive Then
                _listenerThread.Join(5000)
            End If
        Catch ex As Exception
            _log("Error joining listener thread: " & ex.ToString())
        End Try

        Try
            If _cts IsNot Nothing Then _cts.Dispose()
        Catch ex As Exception
            _log("Error disposing token: " & ex.ToString())
        End Try

        _listenerThread = Nothing
        _listener = Nothing
        _cts = Nothing
        _log("HostHttpServer stopped.")
    End Sub

    Private Sub ListenLoop(ct As CancellationToken)
        While Not ct.IsCancellationRequested
            Try
                Dim context = _listener.GetContext()
                ThreadPool.QueueUserWorkItem(Sub() HandleRequest(context))
            Catch ex As HttpListenerException
                If ct.IsCancellationRequested Then Exit While
                _log("HttpListenerException in ListenLoop: " & ex.ToString())
            Catch ex As ObjectDisposedException
                If ct.IsCancellationRequested Then Exit While
                _log("ObjectDisposedException in ListenLoop: " & ex.ToString())
            Catch ex As Exception
                _log("Unexpected exception in ListenLoop: " & ex.ToString())
            End Try
        End While
    End Sub

    Private Sub HandleRequest(context As HttpListenerContext)
        Dim request = context.Request
        Dim response = context.Response

        Try
            If request.HttpMethod <> "GET" OrElse Not request.Url.AbsolutePath.TrimEnd("/"c).Equals("/api/barcode", StringComparison.OrdinalIgnoreCase) Then
                SendResponse(response, HttpStatusCode.NotFound, "{""error"":""Not found""}")
                Return
            End If

            Dim BarcodeValue = request.QueryString("barcodeValue")
            Dim contextCode = request.QueryString("contextCode")

            If String.IsNullOrWhiteSpace(BarcodeValue) OrElse String.IsNullOrWhiteSpace(contextCode) Then
                SendResponse(response, HttpStatusCode.BadRequest, "{""error"":""barcodeValue and contextCode are required""}")
                Return
            End If

            _log("Request: barcodeValue = " & BarcodeValue & " contextCode = " & contextCode)

            Dim allowed As Boolean = _repository.IsAuthorized(BarcodeValue, contextCode)
            Dim json = JsonSerializer.Serialize(New BarcodeResponse With {
                                                .Allowed = allowed,
                                                .BarcodeValue = BarcodeValue,
                                                .ContextCode = contextCode})
            _log("Response: " & json)
            SendResponse(response, HttpStatusCode.OK, json)
        Catch ex As Exception
            _log("Error handling request: " & ex.ToString())
            SendResponse(response, HttpStatusCode.InternalServerError, "{""error"":""Internal server error""}")
        End Try
    End Sub

    Private Sub SendResponse(response As HttpListenerResponse, statusCode As HttpStatusCode, json As String)
        Try
            response.StatusCode = CInt(statusCode)
            response.ContentType = "application/json"
            Dim buffer = Encoding.UTF8.GetBytes(json)
            response.ContentLength64 = buffer.Length
            response.OutputStream.Write(buffer, 0, buffer.Length)
        Finally
            response.OutputStream.Close()
        End Try
    End Sub
End Class
