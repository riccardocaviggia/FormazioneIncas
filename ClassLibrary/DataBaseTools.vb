Imports System.Data.SqlClient

Public Class DataBaseTools
    Private _connString As String

    Public Sub New(connString As String)
        _connString = connString
    End Sub

    Public Function TestConnection() As String
        Using conn As New SqlConnection(_connString)
            Try
                conn.Open()
                Using cmd As New SqlCommand("SELECT TOP 1 CompanyName FROM Customers", conn)
                    Dim result = cmd.ExecuteScalar()
                    Dim name As String = Convert.ToString(result)
                    If String.IsNullOrEmpty(name) Then
                        Return "Connection established, but no data found."
                    End If
                    Return "Connection established! First customer: " & name
                End Using
            Catch ex As Exception
                Return "Error: " & ex.Message
            End Try
        End Using
    End Function
End Class
