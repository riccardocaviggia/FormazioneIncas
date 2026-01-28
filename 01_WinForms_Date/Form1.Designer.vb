<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
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
        Me.btnSend = New System.Windows.Forms.Button()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.txtSurname = New System.Windows.Forms.TextBox()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.txtDay = New System.Windows.Forms.TextBox()
        Me.txtYear = New System.Windows.Forms.TextBox()
        Me.txtMonth = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'btnSend
        '
        Me.btnSend.Location = New System.Drawing.Point(506, 112)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(133, 38)
        Me.btnSend.TabIndex = 0
        Me.btnSend.Text = "Invia"
        Me.btnSend.UseVisualStyleBackColor = True
        '
        'txtName
        '
        Me.txtName.Location = New System.Drawing.Point(116, 94)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(98, 22)
        Me.txtName.TabIndex = 1
        '
        'txtSurname
        '
        Me.txtSurname.Location = New System.Drawing.Point(229, 94)
        Me.txtSurname.Name = "txtSurname"
        Me.txtSurname.Size = New System.Drawing.Size(98, 22)
        Me.txtSurname.TabIndex = 2
        Me.txtSurname.Text = "Surname"
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Location = New System.Drawing.Point(113, 50)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(230, 16)
        Me.lblTitle.TabIndex = 3
        Me.lblTitle.Text = "Inserisci prima il nome poi il cognome"
        '
        'txtDay
        '
        Me.txtDay.Location = New System.Drawing.Point(116, 146)
        Me.txtDay.Name = "txtDay"
        Me.txtDay.Size = New System.Drawing.Size(90, 22)
        Me.txtDay.TabIndex = 4
        Me.txtDay.Text = "Day"
        '
        'txtYear
        '
        Me.txtYear.Location = New System.Drawing.Point(353, 146)
        Me.txtYear.Name = "txtYear"
        Me.txtYear.Size = New System.Drawing.Size(90, 22)
        Me.txtYear.TabIndex = 5
        Me.txtYear.Text = "Year"
        '
        'txtMonth
        '
        Me.txtMonth.Location = New System.Drawing.Point(229, 146)
        Me.txtMonth.Name = "txtMonth"
        Me.txtMonth.Size = New System.Drawing.Size(90, 22)
        Me.txtMonth.TabIndex = 6
        Me.txtMonth.Text = "Month"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.txtMonth)
        Me.Controls.Add(Me.txtYear)
        Me.Controls.Add(Me.txtDay)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.txtSurname)
        Me.Controls.Add(Me.txtName)
        Me.Controls.Add(Me.btnSend)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnSend As Button
    Friend WithEvents txtName As TextBox
    Friend WithEvents txtSurname As TextBox
    Friend WithEvents lblTitle As Label
    Friend WithEvents txtDay As TextBox
    Friend WithEvents txtYear As TextBox
    Friend WithEvents txtMonth As TextBox
End Class
