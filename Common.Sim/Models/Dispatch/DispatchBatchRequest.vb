Imports System.Collections.Generic

Public Class DispatchBatchRequest
    Public Property Orders As List(Of DispatchOrderDto)

    Public Function HasOrders() As Boolean
        Return Orders IsNot Nothing AndAlso Orders.Count > 0
    End Function
End Class