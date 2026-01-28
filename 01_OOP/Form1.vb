Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnSend.Click
        Try
            Dim d As New ClassDate(
                Integer.Parse(txtDay.Text),
                Integer.Parse(txtMonth.Text),
                Integer.Parse(txtYear.Text)
                )
            Dim s As New ClassStudent(txtName.Text, txtSurname.Text, d)
            MsgBox(s.StudentInfo())
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles lblDay.Click

    End Sub
End Class
