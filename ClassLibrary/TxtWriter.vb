Imports System.IO
Public Class TxtWriter
    Public Class FileHelper
        Public Shared Sub WriteToFile(path As String, content As String, append As Boolean)
            Using writer As New StreamWriter(path, append)
                writer.WriteLine(content)
            End Using
        End Sub
    End Class

    Public Class Logger
        Private Shared ReadOnly LogPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "activity.log")
        Public Shared Sub Log(msg As String)
            Dim rowLog As String = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - {msg}"
            FileHelper.WriteToFile(LogPath, rowLog, True)
        End Sub
    End Class
End Class
