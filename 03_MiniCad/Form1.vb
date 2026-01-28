Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button1_Click(sender As Object, e As MouseEventArgs) Handles btnDraw.Click
        Dim p As New Points(e.X, e.Y, Color.Red)
        p.Draw(btnDraw.CreateGraphics())
    End Sub
End Class
