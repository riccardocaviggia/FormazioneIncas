Imports System

Public Class DispatchOrderDto
    Public Property OrderId As Integer
    Public Property Barcode As String
    Public Property ContextCode As String
    Public Property Priority As Integer
    Public Property Location As String
    Public Property OrderDate As DateTime?
    Public Property RequiredDate As DateTime?
End Class