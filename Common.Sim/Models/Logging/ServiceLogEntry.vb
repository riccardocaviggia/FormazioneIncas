Public Class ServiceLogEntry
    Public Property TimeStampUtc As Date = Date.UtcNow
    Public Property ServiceName As String
    Public Property LevelType As String
    Public Property MessageType As String
    Public Property CorrelationId As String
    Public Property Barcode As String
    Public Property ContextCode As String
    Public Property Exception As String
End Class