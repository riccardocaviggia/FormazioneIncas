Imports System.Net.Http
Imports System.Text.Json
Imports System.Threading
Imports System.Threading.Tasks
Imports CommonSim

Public Class HostHttpClient
    Implements IHostGateway

    Private ReadOnly _baseUrl As String
    Private Shared ReadOnly _httpClient As New HttpClient()

    Public Sub New(baseUrl As String)
        _baseUrl = baseUrl.TrimEnd("/"c)
    End Sub

    Public Async Function CheckBarcodeAsync(barcodeValue As String,
                                            contextCode As String,
                                            ct As CancellationToken) As Task(Of BarcodeResponse) _
                                            Implements IHostGateway.CheckBarcodeAsync
        Dim url = $"{_baseUrl}/api/barcode?barcodeValue={Uri.EscapeDataString(barcodeValue)}&contextCode={Uri.EscapeDataString(contextCode)}"

        Dim responseMessage = Await _httpClient.GetAsync(url, ct).ConfigureAwait(False)
        responseMessage.EnsureSuccessStatusCode()

        Dim json = Await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(False)
        Return JsonSerializer.Deserialize(Of BarcodeResponse)(json, _jsonOptions)
    End Function

    Private Shared ReadOnly _jsonOptions As New JsonSerializerOptions() With {
        .PropertyNameCaseInsensitive = True
    }
End Class