Imports System.Text.Json.Serialization

Public Class BarcodeResponse
    <JsonPropertyName("allowed")>
    Public Property Allowed As Boolean

    <JsonPropertyName("barcodeValue")>
    Public Property BarcodeValue As String

    <JsonPropertyName("contextCode")>
    Public Property ContextCode As String
End Class
