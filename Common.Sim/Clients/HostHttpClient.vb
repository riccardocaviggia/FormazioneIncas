Imports System.Net.Http
Imports System.Text.Json
Imports CommonSim

Public Class HostHttpClient
    Private ReadOnly _baseUrl As String
    Private Shared ReadOnly _httpClient As New HttpClient()

    Public Sub New(baseUrl As String)
        _baseUrl = baseUrl.TrimEnd("/"c)
    End Sub

    Public Function CheckBarcode(barcodeValue As String, contextCode As String) As BarcodeResponse
        Dim url = $"{_baseUrl}/api/barcode?barcodeValue={Uri.EscapeDataString(barcodeValue)}&contextCode={Uri.EscapeDataString(contextCode)}"

        Dim responseMessage = _httpClient.GetAsync(url).Result
        responseMessage.EnsureSuccessStatusCode()

        Dim json = responseMessage.Content.ReadAsStringAsync().Result
        Return JsonSerializer.Deserialize(Of BarcodeResponse)(json, _jsonOptions)
    End Function

    Private Shared ReadOnly _jsonOptions As New JsonSerializerOptions() With {
        .PropertyNameCaseInsensitive = True
    }
End Class