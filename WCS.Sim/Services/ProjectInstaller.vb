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
        serviceInstaller.ServiceName = "WCS.Sim"
        serviceInstaller.DisplayName = "Warehouse WCS Service"
        serviceInstaller.Description = "Simulates the WCS system."
        serviceInstaller.StartType = ServiceStartMode.Automatic
        serviceInstaller.DelayedAutoStart = True

        Installers.Add(serviceProcessInstaller)
        Installers.Add(serviceInstaller)
    End Sub
End Class
