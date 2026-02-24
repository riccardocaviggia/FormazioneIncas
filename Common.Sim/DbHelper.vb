Imports System.Data.SqlClient
Imports System.Runtime.InteropServices

Public Module DbHelper
    Public Function ExecuteReader(Of T)(
        connectionString As String,
        sql As String,
        map As Func(Of IDataRecord, T),
        Optional parameters As Dictionary(Of String, Object) = Nothing) As List(Of T)

        Dim results As New List(Of T)()

        Using conn As New SqlConnection(connectionString)
            Using cmd As New SqlCommand(sql, conn)
                AddParameters(cmd, parameters)
                conn.Open()
                Using dr = cmd.ExecuteReader()
                    While dr.Read()
                        results.Add(map(dr))
                    End While
                End Using
            End Using
        End Using
        Return results
    End Function

    Public Function ExecuteScalar(Of T)(
        connectionString As String,
        sql As String,
        Optional parameters As Dictionary(Of String, Object) = Nothing) As T

        Using conn As New SqlConnection(connectionString)
            Using cmd As New SqlCommand(sql, conn)
                AddParameters(cmd, parameters)
                conn.Open()
                Dim result = cmd.ExecuteScalar()
                If result Is Nothing OrElse result Is DBNull.Value Then
                    Return Nothing
                End If
                Return CType(result, T)
            End Using
        End Using

    End Function

    Private Sub AddParameters(cmd As SqlCommand, parameters As Dictionary(Of String, Object))
        If parameters Is Nothing Then Return
        For Each kvp In parameters
            Dim paramName = If(kvp.Key.StartsWith("@"), kvp.Key, "@" & kvp.Key)
            cmd.Parameters.AddWithValue(paramName, If(kvp.Value, DBNull.Value))
        Next
    End Sub
End Module
