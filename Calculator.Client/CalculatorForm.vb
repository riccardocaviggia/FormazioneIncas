Imports System.Globalization
Imports System.Net
Imports System.Web.Services.Protocols

Public Class CalculatorForm
    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles btnCalculate.Click
        Dim val1 As Double
        Dim val2 As Double

        '- Sostituisco il punto con la virgola perchè nella tastiera italiana il punto non è considerato separatore decimale
        If Not Double.TryParse(txtVal1.Text.Replace(".", ","), val1) _
            OrElse Not Double.TryParse(txtVal2.Text.Replace(".", ","), val2) Then
            MsgBox("Inserisci numeri validi")
            txtVal1.Clear()
            txtVal2.Clear()
            Return
        End If

        Dim client As New ServiceCalculatorRef.CalculatorSoapClient()

        Try
            Dim res As Double = Await Task.Run(Function() client.Add(val1, val2))
            txtRes.Text = res.ToString()
            txtVal1.Clear()
            txtVal2.Clear()
        Catch ex As SoapException
            MsgBox("Errore SOAP del servizio: ", ex.Message)
        Catch ex As WebException
            MsgBox("Errore di rete: ", ex.Message)
        Catch ex As Exception
            MsgBox("Errore: ", ex.Message)
        Finally
            client = Nothing
        End Try
    End Sub
End Class
