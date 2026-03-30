<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CalculatorForm
    Inherits System.Windows.Forms.Form

    'Form esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Richiesto da Progettazione Windows Form
    Private components As System.ComponentModel.IContainer

    'NOTA: la procedura che segue è richiesta da Progettazione Windows Form
    'Può essere modificata in Progettazione Windows Form.  
    'Non modificarla mediante l'editor del codice.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.txtVal1 = New System.Windows.Forms.TextBox()
        Me.txtVal2 = New System.Windows.Forms.TextBox()
        Me.txtRes = New System.Windows.Forms.TextBox()
        Me.btnCalculate = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'txtVal1
        '
        Me.txtVal1.Location = New System.Drawing.Point(39, 53)
        Me.txtVal1.Name = "txtVal1"
        Me.txtVal1.Size = New System.Drawing.Size(59, 20)
        Me.txtVal1.TabIndex = 0
        '
        'txtVal2
        '
        Me.txtVal2.Location = New System.Drawing.Point(119, 53)
        Me.txtVal2.Name = "txtVal2"
        Me.txtVal2.Size = New System.Drawing.Size(59, 20)
        Me.txtVal2.TabIndex = 1
        '
        'txtRes
        '
        Me.txtRes.Location = New System.Drawing.Point(315, 54)
        Me.txtRes.Name = "txtRes"
        Me.txtRes.Size = New System.Drawing.Size(68, 20)
        Me.txtRes.TabIndex = 2
        '
        'btnCalculate
        '
        Me.btnCalculate.Location = New System.Drawing.Point(202, 51)
        Me.btnCalculate.Name = "btnCalculate"
        Me.btnCalculate.Size = New System.Drawing.Size(75, 23)
        Me.btnCalculate.TabIndex = 3
        Me.btnCalculate.Text = "Calculate"
        Me.btnCalculate.UseVisualStyleBackColor = True
        '
        'CalculatorForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(457, 137)
        Me.Controls.Add(Me.btnCalculate)
        Me.Controls.Add(Me.txtRes)
        Me.Controls.Add(Me.txtVal2)
        Me.Controls.Add(Me.txtVal1)
        Me.Name = "CalculatorForm"
        Me.Text = "CalculatorForm"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents txtVal1 As TextBox
    Friend WithEvents txtVal2 As TextBox
    Friend WithEvents txtRes As TextBox
    Friend WithEvents btnCalculate As Button
End Class
