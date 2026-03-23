Imports System.Threading

Public Class LocationAllocator
    Private Const MaxZone As Integer = 10
    Private Const MaxSlot As Integer = 10

    Private _zone As Integer = 1
    Private _slot As Integer = 0

    Public Function NextLocation() As String
        SyncLock Me
            _slot += 1
            If _slot > MaxSlot Then
                _slot = 1
                _zone += 1
                If _zone > MaxZone Then
                    _zone = 1
                End If
            End If

            Return $"Z{_zone:00}{_slot:00}"
        End SyncLock
    End Function
End Class