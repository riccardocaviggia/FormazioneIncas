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
        serviceInstaller.ServiceName = "WMS.Sim"
        serviceInstaller.DisplayName = "WMS Simulator Service"
        serviceInstaller.Description = "Simulates the WMS system"
        serviceInstaller.StartType = ServiceStartMode.Automatic

        Installers.Add(serviceProcessInstaller)
        Installers.Add(serviceInstaller)
    End Sub
End Class