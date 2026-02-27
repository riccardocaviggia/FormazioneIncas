Imports System.Text.Json.Serialization

Public Class AckMessage
    <JsonPropertyName("type")>
    Public Property Type As String = "ack"

    <JsonPropertyName("ok")>
    Public Property Ok As Boolean = True

    <JsonPropertyName("id")>
    <JsonIgnore(Condition:=JsonIgnoreCondition.WhenWritingNull)>
    Public Property Id As String
End Class
