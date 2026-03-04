Imports System.Collections.Generic

Public Class OrderDispatchRepository
    Public Const DefaultLocation As String = "Z0000"
    Public Const StatusInProgress As String = "IN_PROGRESS"
    Public Const StatusPending As String = "PENDING"

    Private ReadOnly _connectionString As String

    Public Sub New(connectionString As String)
        If String.IsNullOrWhiteSpace(connectionString) Then Throw New ArgumentNullException(NameOf(connectionString))
        _connectionString = connectionString
    End Sub

    Public Function InsertDispatch(orderId As Integer,
                                   barcode As String,
                                   contextCode As String,
                                   priority As Integer,
                                   Optional location As String = DefaultLocation,
                                   Optional status As String = StatusInProgress) As Guid

        Const insertDispatchSql = "
            INSERT INTO dbo.OrderDispatches (OrderID, Barcode, ContextCode, Location, Priority, Status)
            OUTPUT INSERTED.Id
            VALUES (@OrderID, @Barcode, @ContextCode, @Location, @Priority, @Status);"

        Dim parameters = New Dictionary(Of String, Object) From {
            {"OrderID", orderId},
            {"Barcode", barcode},
            {"ContextCode", contextCode},
            {"Location", location},
            {"Priority", priority},
            {"Status", status}
        }

        Return DbHelper.ExecuteScalar(Of Guid)(_connectionString, insertDispatchSql, parameters)
    End Function

    Public Function InsertRejected(orderId As Integer,
                                   barcode As String,
                                   location As String,
                                   priority As Integer,
                                   reason As String) As Guid

        Const insertRejectedSql = "
            INSERT INTO dbo.RejectedOrders (OrderID, Barcode, Location, Priority, Reason)
            OUTPUT INSERTED.Id
            VALUES (@OrderID, @Barcode, @Location, @Priority, @Reason);"

        Dim parameters = New Dictionary(Of String, Object) From {
            {"OrderID", orderId},
            {"Barcode", barcode},
            {"Location", location},
            {"Priority", priority},
            {"Reason", reason}
        }

        Return DbHelper.ExecuteScalar(Of Guid)(_connectionString, insertRejectedSql, parameters)
    End Function
End Class