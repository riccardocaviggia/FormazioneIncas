Public Class CustonExceptions
    Public Class NorthwindDatabaseException
        Inherits Exception
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
    End Class

    Public Class NorthwindTransactionException
        Inherits Exception
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
    End Class
End Class
