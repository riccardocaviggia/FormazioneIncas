Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Text

Public Class OrderRecord
    Private ReadOnly _values As IReadOnlyDictionary(Of String, Object)

    Public Sub New(values As IDictionary(Of String, Object))
        If values Is Nothing Then Throw New ArgumentNullException(NameOf(values))
        _values = New ReadOnlyDictionary(Of String, Object)(
            New Dictionary(Of String, Object)(values, StringComparer.OrdinalIgnoreCase))
    End Sub

    Public ReadOnly Property Values As IReadOnlyDictionary(Of String, Object)
        Get
            Return _values
        End Get
    End Property

    Public Function TryGetValue(Of T)(columnName As String, ByRef value As T) As Boolean
        If columnName Is Nothing Then Throw New ArgumentNullException(NameOf(columnName))

        Dim rawValue As Object = Nothing
        If Not _values.TryGetValue(columnName, rawValue) OrElse rawValue Is Nothing Then
            value = Nothing
            Return False
        End If

        value = CType(rawValue, T)
        Return True
    End Function

    Public Function GetValue(Of T)(columnName As String, Optional defaultValue As T = Nothing) As T
        Dim result As T = Nothing
        If TryGetValue(columnName, result) Then
            Return result
        End If
        Return defaultValue
    End Function

    Public Overrides Function ToString() As String
        Dim builder As New StringBuilder()
        For Each pair In _values
            If builder.Length > 0 Then builder.Append("; ")
            builder.Append(pair.Key).Append("="c).Append(If(pair.Value, "NULL"))
        Next
        Return builder.ToString()
    End Function
End Class
