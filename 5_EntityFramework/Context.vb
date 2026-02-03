Imports System.Data.Entity

Public Class Context
    Inherits DbContext

    Public Sub New(connString As String)
        MyBase.New(connString)
    End Sub

    Public Property Products As DbSet(Of Product)
End Class
