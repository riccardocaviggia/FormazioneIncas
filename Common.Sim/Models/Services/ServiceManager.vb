Imports System.ComponentModel
Imports System.Configuration.Install
Imports System.Diagnostics
Imports System.Reflection
Imports System.ServiceProcess

Public Class ServiceManager

    '--------------------------------------------------------------------------------------
    '- Gestisce installazione, disinstallazione, avvio e arresto del servizio in base agli argomenti passati
    Public Shared Sub HandleService(ByVal args As String(), ByVal serviceInstance As ServiceBase)
        If args IsNot Nothing AndAlso args.Length > 0 Then
            Try
                Dim exePath As String = Process.GetCurrentProcess().MainModule.FileName

                Select Case args(0).ToLower()
                    Case "/install", "-i", "/i"
                        ManagedInstallerClass.InstallHelper({exePath})
                        Console.WriteLine("Service installed successfully.")
                        Return
                    Case "/uninstall", "-u", "/u"
                        ManagedInstallerClass.InstallHelper({"/u", exePath})
                        Console.WriteLine("Service uninstalled successfully.")
                        Return
                    Case "/start", "-s", "/s"
                        Dim serviceName = GetInstalledServiceName()
                        Using sc As New ServiceController(serviceName)
                            If sc.Status = ServiceControllerStatus.Stopped Then
                                sc.Start()
                                sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30))
                            End If
                            Console.WriteLine("Service '" & serviceName & "' started successfully.")
                        End Using
                        Return
                    Case "/stop", "-t", "/t"
                        Dim serviceName = GetInstalledServiceName()
                        Using sc As New ServiceController(serviceName)
                            If sc.Status = ServiceControllerStatus.Running Then
                                sc.Stop()
                                sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30))
                            End If
                            Console.WriteLine("Service '" & serviceName & "' stopped successfully.")
                        End Using
                        Return
                End Select
            Catch ex As Exception
                Console.WriteLine("Error: " & ex.Message)
                Return
            End Try
        End If

        '- Se non vengono passati argomenti, avvia il servizio normalmente
        Try
            ServiceHelper.RunService(serviceInstance)
        Catch ex As Exception
            Console.WriteLine("Error: " & ex.Message)
        End Try
    End Sub

    '--------------------------------------------------------------------------------------
    '- Legge il ServiceName dal ProjectInstaller dell'assembly chiamante,
    ' cioè il nome reale con cui il servizio è registrato in Windows.
    Private Shared Function GetInstalledServiceName() As String
        Dim asm = Assembly.GetEntryAssembly()
        For Each t In asm.GetTypes()
            If Not GetType(Installer).IsAssignableFrom(t) Then
                Continue For
            End If

            Dim attrs = t.GetCustomAttributes(GetType(RunInstallerAttribute), True)
            If attrs.Length = 0 Then
                Continue For
            End If

            Dim attr = TryCast(attrs(0), RunInstallerAttribute)
            If attr Is Nothing OrElse Not attr.RunInstaller Then
                Continue For
            End If

            Using inst = CType(Activator.CreateInstance(t), Installer)
                For Each child In inst.Installers
                    Dim si = TryCast(child, ServiceInstaller)
                    If si IsNot Nothing Then
                        Return si.ServiceName
                    End If
                Next
            End Using
        Next

        Throw New InvalidOperationException("ProjectInstaller with ServiceInstaller not found in entry assembly.")
    End Function
End Class
