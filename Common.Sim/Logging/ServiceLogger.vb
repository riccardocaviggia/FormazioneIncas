Public Class ServiceLogger
    Private ReadOnly _repository As ServiceLogRepository
    Private ReadOnly _serviceName As String
    Private ReadOnly _fallback As Action(Of String)

    Public Sub New(connectionString As String, serviceName As String, Optional fallback As Action(Of String) = Nothing)
        If String.IsNullOrWhiteSpace(serviceName) Then
            Throw New ArgumentException("Service name is required.", NameOf(serviceName))
        End If

        _repository = New ServiceLogRepository(connectionString)
        _serviceName = serviceName
        _fallback = If(fallback, Sub(message) Debug.WriteLine(message))
    End Sub

    Public Sub Info(messageType As String, Optional correlationId As String = Nothing, Optional barcode As String = Nothing, Optional contextCode As String = Nothing)
        Write("INFO", messageType, Nothing, correlationId, barcode, contextCode)
    End Sub

    Public Sub [Error](messageType As String, exception As Exception, Optional correlationId As String = Nothing, Optional barcode As String = Nothing, Optional contextCode As String = Nothing)
        Write("ERROR", messageType, exception?.ToString(), correlationId, barcode, contextCode)
    End Sub

    Public Sub Log(level As String, messageType As String, Optional details As String = Nothing, Optional correlationId As String = Nothing, Optional barcode As String = Nothing, Optional contextCode As String = Nothing)
        Write(level, messageType, details, correlationId, barcode, contextCode)
    End Sub

    Private Sub Write(level As String, messageType As String, details As String, correlationId As String, barcode As String, contextCode As String)
        Dim entry As New ServiceLogEntry With {
            .TimeStampUtc = Date.UtcNow,
            .ServiceName = _serviceName,
            .LevelType = level,
            .MessageType = messageType,
            .CorrelationId = correlationId,
            .Barcode = barcode,
            .ContextCode = contextCode,
            .Exception = details
        }

        Try
            _repository.Insert(entry)
        Catch ex As Exception
            _fallback?.Invoke("[ServiceLogger] " & ex.Message)
        End Try
    End Sub
End Class