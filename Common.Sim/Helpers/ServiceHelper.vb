Imports System.ServiceProcess
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Threading

Public Class ServiceHelper
    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Shared Function AllocConsole() As Boolean
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Shared Function AttachConsole(dwProcessId As Integer) As Boolean ' si collega alla console del processo padre
    End Function

    Private Const ATTACH_PARENT_PROCESS As Integer = -1

    Public Shared Sub RunService(service As ServiceBase)
        If Environment.UserInteractive Then
            ' Alloca una console se non esiste
            If Not AttachConsole(ATTACH_PARENT_PROCESS) Then
                AllocConsole()
            End If

            Dim onStart = service.GetType().GetMethod("OnStart", BindingFlags.Instance Or BindingFlags.NonPublic)
            Dim realArgs = Environment.GetCommandLineArgs().Skip(1).ToArray()
            onStart.Invoke(service, New Object() {realArgs})

            Dim stopEvent = New ManualResetEvent(False)
            AddHandler Console.CancelKeyPress, Sub(sender, e)
                                                   e.Cancel = True
                                                   stopEvent.Set()
                                               End Sub
            Console.WriteLine("Service is running... Press Ctrl+C to stop.")
            stopEvent.WaitOne()

            Dim onStop = service.GetType().GetMethod("OnStop", BindingFlags.Instance Or BindingFlags.NonPublic)
            onStop.Invoke(service, Nothing)
        Else
            ServiceBase.Run(service)
        End If
    End Sub
End Class
