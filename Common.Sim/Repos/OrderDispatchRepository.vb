Imports System.Collections.Generic

Public Class OrderDispatchRepository
    Public Const DefaultLocation As String = "Z0000"
    Public Const StatusInProgress As String = "IN_PROGRESS"
    Public Const StatusPending As String = "PENDING"
    Public Const StatusFailed As String = "FAILED"
    Public Const StatusCompleted As String = "COMPLETED"
    Public Const StatusArchived As String = "ARCHIVED"

    Private ReadOnly _connectionString As String

    Public Sub New(connectionString As String)
        If String.IsNullOrWhiteSpace(connectionString) Then Throw New ArgumentNullException(NameOf(connectionString))
        _connectionString = connectionString
    End Sub

    '-------------------------------------------------------------------------------
    '- Inserisce nella tabella OrderDispatches un nuovo record con i dati dell'ordine da processare
    ' e restituisce l'ID del record inserito
    Public Function InsertDispatch(orderId As Integer,
                                   barcode As String,
                                   contextCode As String,
                                   priority As Integer,
                                   Optional location As String = DefaultLocation,
                                   Optional status As String = StatusPending) As Guid

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

    '-------------------------------------------------------------------------------
    ' Inserisce un nuovo record nella tabella RejectedOrders e restituisce l'ID del record inserito
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

    '-------------------------------------------------------------------------------
    ' Aggiorna lo stato e la locazione di un record della tabella OrderDispatches
    Public Sub UpdateDispatch(orderId As String, location As String, status As String)
        Const sql = "
            UPDATE dbo.OrderDispatches
            SET Location = @Location,
                Status = @Status,
                UpdatedAt = GETDATE()
            WHERE OrderId = @OrderId;"
        Dim parameters = New Dictionary(Of String, Object) From {
            {"OrderId", orderId},
            {"Location", location},
            {"Status", status}
        }
        DbHelper.ExecuteNonQuery(_connectionString, sql, parameters)
    End Sub

    '-------------------------------------------------------------------------------
    '- Interroga il db per trovare gli ordini che hanno un certo Status
    Public Function FetchBatchByStatus(batchSize As Integer, status As String) As IReadOnlyList(Of OrderRecord)
        Const sql = "SELECT TOP (@BatchSize) OrderID, Location, Status FROM dbo.OrderDispatches WHERE Status = @Status ORDER BY UpdatedAt ASC"

        Dim dictParams As New Dictionary(Of String, Object) From {
            {"BatchSize", batchSize},
            {"Status", status}
        }

        Return DbHelper.ExecuteReader(Of OrderRecord)(
            connectionString:=_connectionString,
            sql:=sql,
            parameters:=dictParams,
            map:=Function(reader)
                     Dim values As New Dictionary(Of String, Object) From {
                         {"OrderId", reader("OrderID")},
                         {"Location", reader("Location")},
                         {"Status", reader("Status")}
                     }
                     Return New OrderRecord(values)
                 End Function)
    End Function

    Public Function FetchBatch(batchSize As Integer) As IReadOnlyList(Of OrderRecord)
        Return FetchBatchByStatus(batchSize, StatusCompleted)
    End Function

    '-------------------------------------------------------------------------------
    '- Interroga il db per contare quanti ordini hanno un certo Status
    Public Function CountByStatus(status As String) As Integer
        Const sql = "SELECT COUNT(*) FROM dbo.OrderDispatches WHERE Status = @Status"

        Dim parameters = New Dictionary(Of String, Object) From {
            {"Status", status}
        }

        Try
            Dim res = DbHelper.ExecuteScalar(Of Object)(_connectionString, sql, parameters)
            If res Is Nothing Then
                Return 0
            End If
            Return Convert.ToInt32(res)
        Catch
            Return 0
        End Try

    End Function
End Class