Imports System.IO

Public Class Logger
    Private Shared ReadOnly LogPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "activity.log")
    Public Shared Sub Log(msg As String)
        Dim rowLog As String = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - {msg}"
        TxtWriter.WriteToFile(LogPath, rowLog, True)
    End Sub
End Class
