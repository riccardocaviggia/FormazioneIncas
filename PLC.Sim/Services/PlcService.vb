Imports System.ServiceProcess
Imports System.Text.Json
Imports System.Threading
Imports CommonSim

Public Class PlcService
    Inherits ServiceBase

    Private _logger As ServiceLogger
    Private _tcpServer As PlcTcpServer

    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim connString As String = ConnectionStringProvider.GetConnectionString(args)

        _logger = New ServiceLogger(connString, "PLC.Sim", Sub()
                                                           End Sub)
        _logger.Info("PLC.Starting")
        _tcpServer = New PlcTcpServer(TcpConfig.GetPlcPort(), _logger)
        _tcpServer.Start()

        _logger.Info("PLC.Stopped")
    End Sub

    Protected Overrides Sub OnStop()
        Try
            _tcpServer?.[Stop]()
            _tcpServer = Nothing

            _logger?.Info("PLC.Stopped")
        Catch ex As Exception
            _logger?.[Error]("PLC.OnStop.Error", ex)
        End Try
    End Sub
End Class
