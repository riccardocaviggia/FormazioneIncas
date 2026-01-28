Public Class ClassDate
    Public Property day As Integer
    Public Property month As Integer
    Public Property year As Integer

    Public Sub New(d As Integer, m As Integer, y As Integer)
        If Not DateCheck(d, m, y) Then
            Throw New ArgumentException("Invalid date")
        End If

        day = d
        month = m
        year = y
    End Sub

    Public Sub New(d As Integer, m As Integer)
        Me.New(d, m, 2000)
    End Sub

    Public Function DateString() As String
        Return $"{day:D2}/{month:D2}/{year}"
    End Function

    Private Function DateCheck(d As Integer, m As Integer, y As Integer) As Boolean
        If d < 1 Or m < 1 Or m > 12 Then Return False

        Dim daysInMonth As Integer
        Select Case m
            Case 2
                daysInMonth = If(y Mod 4 = 0 And (y Mod 100 <> 0 Or y Mod 400 = 0), 29, 28)
            Case 4, 6, 9, 11
                daysInMonth = 30
            Case Else
                daysInMonth = 31
        End Select
        Return d <= daysInMonth
    End Function
End Class
