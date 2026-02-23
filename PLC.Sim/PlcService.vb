Imports System.Data.SqlClient
Imports CommonSim

Public Class PlcService

    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim cs As String = ConnectionStringProvider.GetConnectionString(args)
    End Sub

    Protected Overrides Sub OnStop()
        ' Inserire qui il codice delle procedure di chiusura necessarie per arrestare il proprio servizio.
    End Sub

End Class
