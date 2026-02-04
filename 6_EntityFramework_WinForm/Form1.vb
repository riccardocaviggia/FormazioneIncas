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
        Dim id As Integer

        If Not TryGetSelectedId(id) Then Return

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
        Dim id As Integer

        If Not TryGetSelectedId(id) Then Return

        Try
            Dim deleteP = _db.Products.Find(id)
            If deleteP IsNot Nothing Then
                _db.Products.Remove(deleteP)
                _db.SaveChanges()
                AddData()
            End If
        Catch ex As Exception
            MsgBox("Error: " & ex.Message)
        End Try
    End Sub

    Private Function TryGetSelectedId(ByRef id As Integer) As Boolean
        If dgvProducts.CurrentRow Is Nothing Then
            MsgBox("Please select a row.")
            Return False
        End If

        id = CInt(dgvProducts.CurrentRow.Cells("ProductId").Value)
        Return True
    End Function
End Class
