Imports System.Data.SqlClient
Imports System.Data.SqlTypes

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

    Public Function ExecuteScalar(Of T)(connectionString As String,
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
                Return ConvertScalarResult(Of T)(result)
            End Using
        End Using
    End Function

    Public Sub ExecuteNonQuery(connectionString As String,          ' Modifica il db senza restituire risultato
                                        sql As String,
                                        Optional parameters As Dictionary(Of String, Object) = Nothing)
        Using conn As New SqlConnection(connectionString)
            Using cmd As New SqlCommand(sql, conn)
                AddParameters(cmd, parameters)
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Function ConvertScalarResult(Of T)(value As Object) As T
        Dim targetType = GetType(T)
        Dim underlying = Nullable.GetUnderlyingType(targetType)
        Dim effectiveType = If(underlying, targetType)

        If effectiveType Is GetType(Guid) Then
            Dim guidValue As Guid

            If TypeOf value Is Guid Then
                guidValue = DirectCast(value, Guid)
            ElseIf TypeOf value Is SqlGuid Then
                guidValue = DirectCast(value, SqlGuid).Value
            ElseIf TypeOf value Is Byte() Then
                Dim bytes = DirectCast(value, Byte())
                If bytes.Length <> 16 Then
                    Throw New InvalidCastException("Array di byte non valido per Guid.")
                End If
                guidValue = New Guid(bytes)
            ElseIf TypeOf value Is String Then
                Dim s = DirectCast(value, String)
                If Not Guid.TryParse(s, guidValue) Then
                    Throw New InvalidCastException($"Impossibile convertire la stringa '{s}' in Guid.")
                End If
            Else
                Throw New InvalidCastException($"Impossibile convertire il valore di tipo {value.GetType().FullName} in Guid.")
            End If

            Dim boxed As Object = If(underlying Is Nothing,
                                     CType(guidValue, Object),
                                     CType(New Nullable(Of Guid)(guidValue), Object))
            Return CType(boxed, T)
        End If

        Dim converted = Convert.ChangeType(value, effectiveType)
        Dim boxedResult As Object = If(underlying Is Nothing,
                                       converted,
                                       Activator.CreateInstance(targetType, converted))
        Return CType(boxedResult, T)
    End Function

    Private Sub AddParameters(cmd As SqlCommand, parameters As Dictionary(Of String, Object))
        If parameters Is Nothing Then Return
        For Each param In parameters
            Dim paramName = If(param.Key.StartsWith("@"), param.Key, "@" & param.Key)
            cmd.Parameters.AddWithValue(paramName, If(param.Value, DBNull.Value))
        Next
    End Sub
End Module
