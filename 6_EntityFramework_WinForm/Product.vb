Public Class Product
    Public Property ProductID As Integer
    Public Property ProductName As String
    Public Property UnitPrice As Decimal?
    Public Property CategoryID As Integer?

    Public Overrides Function ToString() As String
        Return $"{ProductID}: {ProductName} - ${UnitPrice} (Category: {CategoryID})"
    End Function
End Class
