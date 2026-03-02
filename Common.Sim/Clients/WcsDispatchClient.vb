Imports System.Net.Http
Imports System.Text
Imports System.Text.Json
Imports System.Threading
Imports System.Threading.Tasks

Public Class WcsDispatchClient
    Private Shared ReadOnly Http As New HttpClient()
    Private Shared ReadOnly JsonOptions As New JsonSerializerOptions With {
        .PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        .PropertyNameCaseInsensitive = True
    }

    Private ReadOnly _endpoint As Uri

    Public Sub New(endpoint As String)
        If String.IsNullOrWhiteSpace(endpoint) Then Throw New ArgumentNullException(NameOf(endpoint))
        _endpoint = New Uri(endpoint, UriKind.Absolute)
    End Sub

    Public Async Function SendAsync(orders As IReadOnlyList(Of DispatchOrderDto),
                                    ct As CancellationToken) As Task
        If orders Is Nothing OrElse orders.Count = 0 Then Return

        Dim payload = New DispatchBatchRequest With {.Orders = orders.ToList()}
        Dim json = JsonSerializer.Serialize(payload, JsonOptions)

        Using content As New StringContent(json, Encoding.UTF8, "application/json")
            Dim response = Await Http.PostAsync(_endpoint, content, ct).ConfigureAwait(False)
            response.EnsureSuccessStatusCode()
        End Using
    End Function
End Class