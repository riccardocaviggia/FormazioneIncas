Imports ClassLibrary

Public Class Form1
    Private _config As AppConfig
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _config = ConfigManager.LoadConfig()
        pgTable.SelectedObject = _config
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        ConfigManager.SaveConfig(_config)
        Dim db As New DataBaseTools(_config.ConnectionStrings.ConnDEV)
        MsgBox("Saved!" & vbCrLf & db.TestConnection())
    End Sub
End Class
