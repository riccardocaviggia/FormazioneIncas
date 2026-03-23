Imports System.Collections.Generic
Imports System.Data

Public Class OrdersRepository
    Private ReadOnly _connectionString As String

    Public Sub New(connectionString As String)
        If String.IsNullOrWhiteSpace(connectionString) Then Throw New ArgumentNullException(NameOf(connectionString))
        _connectionString = connectionString
    End Sub

    Public Function FetchBatch(batchSize As Integer) As IReadOnlyList(Of OrderRecord)
        If batchSize <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(batchSize))

        Const sql = "
            SELECT TOP (@BatchSize) o.*
            FROM Northwind.dbo.Orders AS o WITH (READPAST)
            LEFT JOIN dbo.OrderDispatches AS od ON od.OrderID = o.OrderID
            WHERE o.ShippedDate IS NULL
              AND od.Id IS NULL
            ORDER BY o.OrderDate ASC, o.OrderID ASC;"

        Dim parameters = New Dictionary(Of String, Object) From {
            {"BatchSize", batchSize}
        }

        Return DbHelper.ExecuteReader(_connectionString, sql, AddressOf MapOrder, parameters)
    End Function

    Private Shared Function MapOrder(record As IDataRecord) As OrderRecord
        Dim values As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)

        For i = 0 To record.FieldCount - 1
            values(record.GetName(i)) = If(record.IsDBNull(i), Nothing, record.GetValue(i))
        Next

        Return New OrderRecord(values)
    End Function
End Class
