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
        Try
            Dim sql As String = "INSERT INTO Products (ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued) VALUES ('Prodotto test', 1, 1, 15, 0, 10, 0, 0, 0)"
            Dim queries As New List(Of String)
            queries.Add(sql)
            Logger.Log("Attempting to add new product.")
            If _crud.ExecuteTransaction(queries) Then
                Logger.Log("New product added successfully.")
                MsgBox("Product added successfully.")
                LoadData()
            End If
        Catch ex As NorthwindTransactionException
            Logger.Log("Transaction error while adding product: " & ex.Message)
            MsgBox("Transaction error: " & ex.Message)
        Catch ex As Exception
            Logger.Log("An error occurred: " & ex.Message)
            MsgBox("Error: " & ex.Message)
        End Try
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        If dgvProducts.CurrentRow Is Nothing Then
            MsgBox("Please select a product to update.")
            Return
        End If

        Dim id As Integer = CInt(dgvProducts.CurrentRow.Cells("ProductID").Value)

        Try
            Logger.Log("Attempting to update product with ID: " & id)
            Dim sql As String = $"UPDATE Products SET ProductName = 'Product updated' WHERE ProductID = {id}"
            Dim queries As New List(Of String)
            queries.Add(sql)
            Logger.Log("Executing update transaction.")
            _crud.ExecuteTransaction(queries)
            Logger.Log("Product updated successfully.")
            LoadData()
            MsgBox("Product updated successfully.")
        Catch ex As NorthwindTransactionException
            Logger.Log("UPDATE failed: " & ex.Message)
            MsgBox("Transaction error: " & ex.Message)
        End Try
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If dgvProducts.CurrentRow Is Nothing Then
            MsgBox("Please select a product to delete.")
            Return
        End If

        Dim id As Integer = CInt(dgvProducts.CurrentRow.Cells("ProductID").Value)

        Try
            Logger.Log("Attempting to delete product with ID: " & id)
            Dim sql As String = $"DELETE FROM Products WHERE ProductID = {id}"
            Dim queries As New List(Of String)
            queries.Add(sql)
            Logger.Log("Executing delete transaction.")
            _crud.ExecuteTransaction(queries)
            Logger.Log("Product deleted successfully.")
            LoadData()
            MsgBox("Product deleted successfully.")
        Catch ex As NorthwindTransactionException
            Logger.Log("DELETE failed: " & ex.Message)
            MsgBox("Transaction error: " & ex.Message)
        End Try
    End Sub
End Class
