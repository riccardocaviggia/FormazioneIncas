Imports System.IO
Public Class TxtWriter
    Public Shared Sub WriteToFile(path As String, content As String, append As Boolean)
        Using writer As New StreamWriter(path, append)
            writer.WriteLine(content)
        End Using
    End Sub
End Class
