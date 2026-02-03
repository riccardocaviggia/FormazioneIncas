Public Class Form1
    Private _connString As String
    Private _db As Context
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Dim args As String() = Environment.GetCommandLineArgs()
            If args.Length <> 2 Then
                Throw New Exception("Error: Provide only the configuration path as argument.")
            Else _connString = args(1)
            End If

            _db = New Context(_connString)
            AddData()
        Catch ex As Exception
            MsgBox("Initialisation error")
        End Try
    End Sub

    Private Sub AddData()
        Try
            Dim products = _db.Products.ToList()
            dgvProducts.DataSource = products
        Catch ex As Exception
            MsgBox("Error adding data from the database")
        End Try
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Try
            Dim newProduct As New Product With {
                .ProductName = "EF Product Test",
                .UnitPrice = 200.7,
                .CategoryID = 3
                }

            _db.Products.Add(newProduct)
            _db.SaveChanges()
            AddData()
        Catch ex As Exception
            MsgBox("Error: " & ex.Message)
        End Try
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        If dgvProducts.CurrentRow Is Nothing Then Return

        Dim id As Integer = CInt(dgvProducts.CurrentRow.Cells("ProductID").Value)

        Try
            Dim modifiedP = _db.Products.Find(id)
            If modifiedP IsNot Nothing Then
                modifiedP.ProductName = "Modificato"
                modifiedP.UnitPrice = 1200.3
                _db.SaveChanges()
                AddData()
            End If
        Catch ex As Exception
            MsgBox("Error: " & ex.Message)
        End Try
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If dgvProducts.CurrentRow Is Nothing Then Return

        Dim id As Integer
    End Sub
End Class
