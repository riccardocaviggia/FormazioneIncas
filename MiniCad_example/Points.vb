Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices

Public Class Points
    Private _x As Integer = 0
    Private _y As Integer = 0
    Private _color As Color = Color.Black
    Private _where As Graphics = Nothing
    Private _pen As Pen = Nothing
    Public Shared pointsNum As Integer

    Public Property x As Integer
        Get
            Return _x
        End Get
        Set(value As Integer)
            _x = value
        End Set
    End Property

    Public Property y As Integer
        Get
            Return _y
        End Get
        Set(value As Integer)
            _y = value
        End Set
    End Property

    Public Property color As Color
        Get
            Return _color
        End Get
        Set(value As Color)
            _color = value
        End Set
    End Property

    Public Sub New(px As Integer, py As Integer, pcolor As Color)
        _x = px
        _y = py
        _color = pcolor
        _pen = New Pen(color)
        pointsNum += 1
    End Sub

    Public Sub New(px As Integer, py As Integer, pcolor As Color, pwhere As Graphics)
        _x = px
        _y = py
        _color = pcolor
        _pen = New Pen(color)
        _where = pwhere
    End Sub

    Public Shared Sub PointsSituation()
        MsgBox("Total Points: " & pointsNum)
    End Sub

    Public Sub Draw()
        _where.DrawRectangle(_pen, x, y, 1, 1)
    End Sub

    Public Sub Draw(pwhere As Graphics)
        pwhere.DrawRectangle(_pen, x, y, 1, 1)
    End Sub
End Class
