Public Class Coin
    Public Property Value As Double
    Public Property Symbol As Char
    Private Const EuroToDollarRate As Double = 1.25

    Public Sub New(val As Double, sym As Char)
        Value = val
        Symbol = sym
    End Sub

    Public Function Convert() As Coin
        If Symbol = "€"c Then
            Return New Coin(Value * EuroToDollarRate, "$"c)
        ElseIf Symbol = "$"c Then
            Return New Coin(Value / EuroToDollarRate, "€"c)
        Else
            Throw New ArgumentException("Unsupported currency symbol.")
        End If
    End Function

    Public Overrides Function ToString() As String
        Return String.Format("{0:0.00} {1}", Value, Symbol)
    End Function
End Class
