Imports System.Data
Imports System.Data.SqlClient

Public Class ServiceLogRepository
    Private ReadOnly _connectionString As String

    Public Sub New(connectionString As String)
        If String.IsNullOrWhiteSpace(connectionString) Then
            Throw New ArgumentException("Connection string is required.", NameOf(connectionString))
        End If

        _connectionString = connectionString
    End Sub

    Public Sub Insert(entry As ServiceLogEntry)
        If entry Is Nothing Then Throw New ArgumentNullException(NameOf(entry))

        Const sql As String = "INSERT INTO ServiceLogs (TimeStampUtc, ServiceName, LevelType, MessageType, CorrelationId, Barcode, ContextCode, Exception) VALUES (@TimeStampUtc, @ServiceName, @LevelType, @MessageType, @CorrelationId, @Barcode, @ContextCode, @Exception)"

        Using conn As New SqlConnection(_connectionString)
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.Add("@TimeStampUtc", SqlDbType.DateTime2).Value = entry.TimeStampUtc
                cmd.Parameters.Add("@ServiceName", SqlDbType.NVarChar, 128).Value = entry.ServiceName
                cmd.Parameters.Add("@LevelType", SqlDbType.NVarChar, 16).Value = entry.LevelType
                cmd.Parameters.Add("@MessageType", SqlDbType.NVarChar, 128).Value = entry.MessageType
                cmd.Parameters.Add("@CorrelationId", SqlDbType.UniqueIdentifier).Value = GuidOrDbNull(entry.CorrelationId)
                cmd.Parameters.Add("@Barcode", SqlDbType.NVarChar, 64).Value = DbValue(entry.Barcode)
                cmd.Parameters.Add("@ContextCode", SqlDbType.NVarChar, 32).Value = DbValue(entry.ContextCode)
                cmd.Parameters.Add("@Exception", SqlDbType.NVarChar, -1).Value = DbValue(entry.Exception)

                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Shared Function DbValue(value As String) As Object
        Return If(String.IsNullOrWhiteSpace(value), CType(DBNull.Value, Object), value)
    End Function

    Private Shared Function GuidOrDbNull(value As String) As Object
        If String.IsNullOrWhiteSpace(value) Then Return DBNull.Value

        Dim parsed As Guid
        If Guid.TryParse(value, parsed) Then
            Return parsed
        End If

        Return DBNull.Value
    End Function
End Class