Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices

Public Class Points
    Public Property x As Integer = 0
    Public Property y As Integer = 0
    Public Property color As Color = Color.Black
    Public Property where As Graphics = Nothing
    Public Property pen As Pen = Nothing
    Public Shared pointsNum As Integer

    Public Sub New(px As Integer, py As Integer, pcolor As Color)
        x = px
        y = py
        color = pcolor
        pen = New Pen(color)
        pointsNum += 1
    End Sub

    Public Sub New(px As Integer, py As Integer, pcolor As Color, pwhere As Graphics)
        x = px
        y = py
        color = pcolor
        pen = New Pen(color)
        where = pwhere
    End Sub

    Public Shared Sub PointsSituation()
        MsgBox("Total Points: " & pointsNum)
    End Sub

    Public Sub Draw()
        where.DrawRectangle(pen, x, y, 1, 1)
    End Sub

    Public Sub Draw(pwhere As Graphics)
        pwhere.DrawRectangle(pen, x, y, 1, 1)
    End Sub
End Class
