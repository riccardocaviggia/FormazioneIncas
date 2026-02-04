Imports System.Data.SqlClient
Imports ClassLibrary
Imports ClassLibrary.CustonExceptions
Imports ClassLibrary.TxtWriter
Public Class Form1
    Private _config As AppConfig
    Private _crud As SqlCrudManager
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            _config = ConfigManager.LoadConfig()
            Dim strFromFile As String = _config.ConnectionStrings.ConnDEV
            If String.IsNullOrEmpty(strFromFile) Then
                Throw New NorthwindDatabaseException("Connection string is empty in configuration.")
            End If
            _crud = New SqlCrudManager(strFromFile)
            Logger.Log("Application started. Parameters read from configuration file.")
            LoadData()
        Catch ex As Exception
            Logger.Log("Error during application startup: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadData()
        Try
            Dim sql As String = "SELECT * FROM Products"
            Dim dt As DataTable = _crud.ReadMethod(sql)
            dgvProducts.DataSource = dt
            Logger.Log("Data loaded successfully from Products table.")
        Catch ex As Exception
            MsgBox("Error loading data: " & ex.Message)
        End Try
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Dim sql As String = "INSERT INTO Products (ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued) VALUES ('Prodotto test', 1, 1, 15, 0, 10, 0, 0, 0)"
        ExecuteSqlTransaction(sql, "Attempting to add new product.", "Product added successfully.")
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Dim id As Integer
        If Not TryGetSelectedProductId(id) Then Return

        Dim sql As String = $"UPDATE Products SET ProductName = 'Product updated' WHERE ProductID = {id}"
        ExecuteSqlTransaction(sql, "Attempting to update a product", "Product updated successfully")
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim id As Integer
        If Not TryGetSelectedProductId(id) Then Return

        Dim sql = $"DELETE FROM Products WHERE ProductID = {id}"
        ExecuteSqlTransaction(sql, "Attempting to delete a product", "Product deleted successfully")
    End Sub

    Private Function TryGetSelectedProductId(ByRef id As Integer) As Boolean
        If dgvProducts.CurrentRow Is Nothing Then
            MsgBox("Please select a product to continue.")
            Return False
        End If

        id = CInt(dgvProducts.CurrentRow.Cells("ProductId").Value)
        Return True
    End Function

    Private Sub ExecuteSqlTransaction(sql As String, actionLog As String, successMsg As String)
        Try
            Dim queries As New List(Of String) From {sql}
            Logger.Log(actionLog)
            Dim executed As Boolean = _crud.ExecuteTransaction(queries)

            If executed Then
                Logger.Log(successMsg)
                LoadData()
            Else
                Logger.Log("Transaction executed but returned False for SQL: " & sql)
            End If
        Catch ex As NorthwindTransactionException
            Logger.Log("Transaction error: " & ex.Message)
            MsgBox("Transaction error: " & ex.Message)
        Catch ex As Exception
            Logger.Log("An error occurred: " & ex.Message)
            MsgBox("An error occurred: " & ex.Message)
        End Try
    End Sub
End Class
