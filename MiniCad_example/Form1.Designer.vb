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
        Me.btnDraw = New System.Windows.Forms.Button()
        Me.btnStaticMembers = New System.Windows.Forms.Button()
        Me.cdlgColorSelect = New System.Windows.Forms.ColorDialog()
        Me.SuspendLayout()
        '
        'btnDraw
        '
        Me.btnDraw.Location = New System.Drawing.Point(633, 392)
        Me.btnDraw.Name = "btnDraw"
        Me.btnDraw.Size = New System.Drawing.Size(155, 46)
        Me.btnDraw.TabIndex = 0
        Me.btnDraw.Text = "Points"
        Me.btnDraw.UseVisualStyleBackColor = True
        '
        'btnStaticMembers
        '
        Me.btnStaticMembers.Location = New System.Drawing.Point(12, 392)
        Me.btnStaticMembers.Name = "btnStaticMembers"
        Me.btnStaticMembers.Size = New System.Drawing.Size(155, 46)
        Me.btnStaticMembers.TabIndex = 1
        Me.btnStaticMembers.Text = "Static Members Demo"
        Me.btnStaticMembers.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.btnStaticMembers)
        Me.Controls.Add(Me.btnDraw)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnStaticMembers As Button
    Private WithEvents btnDraw As Button
    Friend WithEvents cdlgColorSelect As ColorDialog
End Class
