Imports System.Net
Imports System.Net.Http
Imports System.Text
Imports System.Text.Json
Imports System.Threading
Imports System.Threading.Tasks

Public Class WmsDispatchClient
    Private Shared ReadOnly Http As New HttpClient(
        New HttpClientHandler() With {
            .UseProxy = False,      ' disabilita il proxy
            .ServerCertificateCustomValidationCallback =
                Function(msg, cert, chain, errors) True   ' accetta certificati autofirmati
        })

    Private Shared ReadOnly JsonOptions As New JsonSerializerOptions With {
        .PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        .PropertyNameCaseInsensitive = True
    }

    Private ReadOnly _endpoint As Uri
    Private ReadOnly _authHeaderValue As String

    Public Sub New(endpoint As String, username As String, password As String)
        If String.IsNullOrWhiteSpace(endpoint) Then Throw New ArgumentNullException(NameOf(endpoint))
        If String.IsNullOrWhiteSpace(username) Then Throw New ArgumentNullException(NameOf(username))
        If String.IsNullOrWhiteSpace(password) Then Throw New ArgumentNullException(NameOf(password))
        _endpoint = New Uri(endpoint, UriKind.Absolute)
        _authHeaderValue = BasicAuthenticator.BuildHeaderValue(username, password)
    End Sub

    '-------------------------------------------------------------------------------
    '- Invia il batch al WMS e attende la risposta
    Public Async Function SendBatchAsync(orders As IReadOnlyList(Of DispatchOrderDto),
                                         ct As CancellationToken) As Task(Of Boolean)
        If orders Is Nothing OrElse orders.Count = 0 Then
            Return False
        End If

        Dim payload = New DispatchBatchRequest With {.Orders = orders.ToList()}
        Dim json = JsonSerializer.Serialize(payload, JsonOptions)
        Using request As New HttpRequestMessage(HttpMethod.Post, _endpoint)
            request.Content = New StringContent(json, Encoding.UTF8, "application/json")
            request.Headers.TryAddWithoutValidation("Authorization", _authHeaderValue)
            Dim response = Await Http.SendAsync(request, ct).ConfigureAwait(False)
            response.EnsureSuccessStatusCode()
            Return True
        End Using
    End Function
End Class