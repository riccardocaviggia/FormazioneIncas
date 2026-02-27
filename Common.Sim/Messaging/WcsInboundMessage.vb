Imports System.Text.Json.Serialization

Public Class WcsInboundMessage
    <JsonPropertyName("id")>
    Public Property Id As String

    <JsonPropertyName("type")>
    Public Property Type As String

    <JsonPropertyName("value")>
    Public Property Value As String

    <JsonPropertyName("contextCode")>
    Public Property ContextCode As String

    <JsonPropertyName("ts")>
    Public Property Ts As String
End Class
