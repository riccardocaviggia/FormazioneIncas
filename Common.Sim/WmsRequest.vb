Imports System.Runtime.Serialization

<DataContract>
Public Class WmsRequest
    <DataMember>
    Public Property BarcodeValue As String

    <DataMember>
    Public Property ContextCode As String
End Class
