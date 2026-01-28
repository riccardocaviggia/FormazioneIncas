Public Class Points
    Dim x As Integer = 0
    Dim y As Integer = 0
    Dim color As Color = Color.Black

    Public Sub New(px As Integer, py As Integer, pcolor As Color)
        x = px
        y = py
        color = pcolor
    End Sub

    'Public Sub New(px As Integer, py As Integer, pcolor As Color, where As Graphics)
    'x = px
    'y = py
    'color = pcolor
    'pwhere = where
    'End Sub

    'Public Sub Draw()
    'pwhere.DrawRectangle(New Pen(color), x, y, 1, 1)
    'End Sub

    Public Sub Draw(where As Graphics)
        where.DrawRectangle(New Pen(color), x, y, 1, 1)
    End Sub
End Class
