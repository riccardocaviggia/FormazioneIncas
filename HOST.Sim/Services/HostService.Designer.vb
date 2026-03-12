Imports System.ServiceProcess
Imports CommonSim

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class HostService
    Inherits System.ServiceProcess.ServiceBase

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    ' Punto di ingresso principale — avvia WmsHostService
    <MTAThread()>
    <System.Diagnostics.DebuggerNonUserCode()>
    Shared Sub Main(ByVal args() As String)
        ServiceManager.HandleService(args, New HostService())
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New System.ComponentModel.Container()
        Me.ServiceName = "WmsHostService"
    End Sub

End Class