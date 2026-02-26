Imports System.Data.SqlClient
Imports System.Runtime.InteropServices

Public Module DbHelper
    Public Function ExecuteReader(Of T)(
        connectionString As String,
        sql As String,
        map As Func(Of IDataRecord, T),     ' Trasforma ogni riga in un oggetto T
        Optional parameters As Dictionary(Of String, Object) = Nothing) As List(Of T)

        Dim results As New List(Of T)()

        Using conn As New SqlConnection(connectionString)
            Using cmd As New SqlCommand(sql, conn)
                AddParameters(cmd, parameters)
                conn.Open()
                Using dr = cmd.ExecuteReader()
                    While dr.Read()     ' Itera ogni riga
                        results.Add(map(dr))    ' Trasforma la riga in un oggetto T
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
        For Each param In parameters
            Dim paramName = If(param.Key.StartsWith("@"), param.Key, "@" & param.Key)
            cmd.Parameters.AddWithValue(paramName, If(param.Value, DBNull.Value))
        Next
    End Sub
End Module
