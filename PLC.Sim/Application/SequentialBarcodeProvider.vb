Imports System.Globalization
Imports System.Threading
Imports CommonSim

Public Class SequentialBarcodeProvider
    Implements IBarcodeProvider

    Private ReadOnly _contexts As String()
    Private _seq As Integer

    Public Sub New(contexts As IEnumerable(Of String))
        If contexts Is Nothing Then Throw New ArgumentNullException(NameOf(contexts))
        _contexts = contexts.Where(Function(c) Not String.IsNullOrWhiteSpace(c)).Select(Function(c) c.Trim()).ToArray()
        If _contexts.Length = 0 Then
            Throw New InvalidOperationException("No enabled contexts available for barcode generation.")
        End If
    End Sub

    Public Function CreatePayload() As String Implements IBarcodeProvider.CreatePayload
        Dim current = Interlocked.Increment(_seq)
        Dim context = _contexts((current - 1) Mod _contexts.Length)
        Dim barcodeValue = "BC" & current.ToString("00000000", CultureInfo.InvariantCulture)
        Dim id = Guid.NewGuid().ToString("N")
        Dim ts = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)

        Return String.Format(
            CultureInfo.InvariantCulture,
            "{{""type"":""barcode"",""id"":""{0}"",""value"":""{1}"",""contextCode"":""{2}"",""ts"":""{3}""}}",
            id, barcodeValue, context, ts)
    End Function
End Class