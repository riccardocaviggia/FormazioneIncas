Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Dim choosenColor As Color = Color.Black

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnDraw.Click
        Dim p As Points
        Dim randomNum As New Random()
        Dim i As Integer = 1

        For i = 1 To 1000
            p = New Points(Math.Ceiling(randomNum.NextDouble() * btnDraw.Width), Math.Ceiling(randomNum.NextDouble() * btnDraw.Height), Color.Red, btnDraw.CreateGraphics)
            p.Draw()
        Next


    End Sub

    Private Sub btnStaticMembers_Click(sender As Object, e As EventArgs) Handles btnStaticMembers.Click
        Dim p1 As Points
        Dim p2 As Points
        Dim p3 As Points

        Points.PointsSituation()
        p1 = New Points(50, 50, Color.Blue)
        Points.PointsSituation()

        p2 = New Points(50, 50, Color.Blue)
        Points.PointsSituation()

        p3 = New Points(50, 50, Color.Blue)
        Points.PointsSituation()
    End Sub

    Private Sub Form1_MouseClick(sender As Object, ev As EventArgs) Handles MyBase.MouseClick
        If DirectCast(ev, MouseEventArgs).Button = MouseButtons.Right Then
            If cdlgColorSelect.ShowDialog() = DialogResult.OK Then
                choosenColor = cdlgColorSelect.Color
            End If
        Else
            Dim p As Points = New Points(DirectCast(ev, MouseEventArgs).X, DirectCast(ev, MouseEventArgs).Y, choosenColor, CreateGraphics())
            p.Draw()
        End If
    End Sub
End Class
