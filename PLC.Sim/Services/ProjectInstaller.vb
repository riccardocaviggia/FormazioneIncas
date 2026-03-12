Imports System.ComponentModel
Imports System.Configuration.Install
Imports System.ServiceProcess

<RunInstaller(True)>
Public Class ProjectInstaller
    Inherits Installer

    Private serviceInstaller As ServiceInstaller
    Private serviceProcessInstaller As ServiceProcessInstaller

    Public Sub New()
        serviceProcessInstaller = New ServiceProcessInstaller()
        serviceProcessInstaller.Account = ServiceAccount.LocalSystem

        serviceInstaller = New ServiceInstaller()
        serviceInstaller.ServiceName = "PLC.Sim"
        serviceInstaller.DisplayName = "Warehouse PLC Service"
        serviceInstaller.Description = "Simulates the PLC system"
        serviceInstaller.StartType = ServiceStartMode.Automatic
        serviceInstaller.DelayedAutoStart = True

        Installers.Add(serviceProcessInstaller)
        Installers.Add(serviceInstaller)
    End Sub
End Class