Public Class Form1

    Private Sub btnConvert_Click(sender As Object, e As EventArgs) Handles btnConvert.Click
        Try
            Dim input As String = txtValue.Text.Trim()

            If input.Length < 2 Then
                Throw New Exception("Invalid input")
            End If

            Dim symbol As Char = input.Last()
            Dim number As String = input.Substring(0, input.Length - 1).Trim()
            Dim amount As Double

            If Not Double.TryParse(number, amount) Then
                Throw New Exception("Invalid number")
            End If

            Dim c As New Coin(amount, symbol)
            Dim result As Coin = c.Convert()

            txtConverted.Text = result.ToString()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub
End Class
