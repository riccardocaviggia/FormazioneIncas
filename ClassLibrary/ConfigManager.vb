Imports System.IO
Imports System.Xml.Serialization

Public Class ConfigManager
    Private Shared FilePath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml")

    Public Shared Function LoadConfig() As AppConfig
        If Not File.Exists(FilePath) Then Return New AppConfig()
        Try
            Dim serializer As New XmlSerializer(GetType(AppConfig))
            Using reader As New StreamReader(FilePath)
                Return DirectCast(serializer.Deserialize(reader), AppConfig)
            End Using
        Catch ex As Exception
            Return New AppConfig()
        End Try
    End Function

    Public Shared Sub SaveConfig(config As AppConfig)
        Dim serializer As New XmlSerializer(GetType(AppConfig))
        Using writer As New StreamWriter(FilePath)
            serializer.Serialize(writer, config)
        End Using
    End Sub
End Class
