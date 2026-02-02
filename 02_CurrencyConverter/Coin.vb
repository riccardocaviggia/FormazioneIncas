Public Class Coin
    Private _value As Double
    Private _symbol As Char
    Private _euroToDollarRate As Double = 1.25


    Public Property value As Double
        Set(val As Double)
            _value = val
        End Set
        Get
            Return _value
        End Get
    End Property

    Public Property symbol As Char
        Set(sym As Char)
            _symbol = sym
        End Set
        Get
            Return _symbol
        End Get
    End Property
    Public Sub New(val As Double, sym As Char)
        _value = val
        _symbol = sym
    End Sub

    Public Function Convert() As Coin
        If Symbol = "€"c Then
            Return New Coin(value * _euroToDollarRate, "$"c)
        ElseIf Symbol = "$"c Then
            Return New Coin(value / _euroToDollarRate, "€"c)
        Else
            Throw New ArgumentException("Unsupported currency symbol.")
        End If
    End Function

    Public Overrides Function ToString() As String
        Return String.Format("{0:0.00} {1}", Value, Symbol)
    End Function

    'todo da fare
End Class
