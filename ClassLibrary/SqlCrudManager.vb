Imports System.Data.SqlClient
Imports ClassLibrary.CustonExceptions

Public Class SqlCrudManager
    Private _connectionString As String

    Public Sub New(connString As String)
        _connectionString = connString
    End Sub

    Public Function ExecuteTransaction(queries As List(Of String)) As Boolean
        Using conn As New SqlConnection(_connectionString)
            conn.Open()
            Dim transaction As SqlTransaction = conn.BeginTransaction()
            Try
                For Each query In queries
                    Dim cmd As New SqlCommand(query, conn, transaction)
                    cmd.ExecuteNonQuery()
                Next
                transaction.Commit()
                Return True
            Catch ex As Exception
                transaction.Rollback()
                Throw New NorthwindTransactionException("Transaction failed and was rolled back.", ex)
            End Try
        End Using
    End Function

    Public Function ReadMethod(sql As String) As DataTable
        Dim dt As New DataTable()
        Try
            Using conn As New SqlConnection(_connectionString)
                Dim adapter As New SqlDataAdapter(sql, conn)
                adapter.Fill(dt)
            End Using
        Catch ex As Exception
            Throw New NorthwindDatabaseException("Error executing read method: " & ex.Message)
        End Try
        Return dt
    End Function
End Class
