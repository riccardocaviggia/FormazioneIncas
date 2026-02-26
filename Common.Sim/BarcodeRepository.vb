Imports System.Data.SqlClient

Public Class BarcodeRepository
    Private ReadOnly _connectionString As String

    Public Sub New(connectionString As String)
        _connectionString = connectionString
    End Sub

    Public Function IsAuthorized(barcodeValue As String, contextCode As String) As Boolean
        Dim query As String = "SELECT COUNT(1) FROM BarcodeRegistry WHERE BarcodeValue = @BarcodeValue AND contextCode = @ContextCode AND IsActive = 1"

        Using conn As New SqlConnection(_connectionString)
            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@BarcodeValue", barcodeValue)
                cmd.Parameters.AddWithValue("@ContextCode", contextCode)
                conn.Open()
                Dim count As Integer = CInt(cmd.ExecuteScalar())
                Return count > 0
            End Using
        End Using
    End Function
End Class
