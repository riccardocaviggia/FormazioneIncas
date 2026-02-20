Imports System.ServiceProcess
Imports CommonSim

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class WcsService
    Inherits System.ServiceProcess.ServiceBase

    'UserService esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
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

    ' Il punto di ingresso principale del processo
    <MTAThread()> _
    <System.Diagnostics.DebuggerNonUserCode()> _
    Shared Sub Main()
        ServiceHelper.RunService(New WcsService())
    End Sub

    'Richiesto da Progettazione componenti
    Private components As System.ComponentModel.IContainer

    ' NOTA: la procedura che segue è richiesta da Progettazione componenti
    ' Può essere modificata in Progettazione componenti.  
    ' Non modificarla nell'editor del codice.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        components = New System.ComponentModel.Container()
        Me.ServiceName = "WcsService"
    End Sub

End Class
