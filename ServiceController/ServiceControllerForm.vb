Imports System.ServiceProcess

Public Class ServiceControllerForm
    Private ReadOnly servicesToMonitor As New List(Of ServiceInfo) From {
        New ServiceInfo("WMS.Sim", "Warehouse WMS Service"),
        New ServiceInfo("WCS.Sim", "Warehouse WCS Service"),
        New ServiceInfo("PLC.Sim", "PLC Simulator Service"),
        New ServiceInfo("HOST.Sim", "Host Polling Service")
    }

    Private Sub ServiceControllerForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupDataGridView()
        PopulateGrid()

        ' Configura il Timer (frequenza 2 secondi)
        TimerRefresh.Interval = 2000
        TimerRefresh.Start()
    End Sub

    Private Sub SetupDataGridView()
        dgvServices.Dock = DockStyle.Fill
        dgvServices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvServices.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells
        dgvServices.AllowUserToAddRows = False
        dgvServices.AllowUserToResizeRows = False
        dgvServices.RowHeadersVisible = False
        dgvServices.ReadOnly = True
        dgvServices.Enabled = True
        dgvServices.SelectionMode = DataGridViewSelectionMode.CellSelect
        dgvServices.MultiSelect = False

        dgvServices.DefaultCellStyle.SelectionBackColor = dgvServices.DefaultCellStyle.BackColor
        dgvServices.DefaultCellStyle.SelectionForeColor = dgvServices.DefaultCellStyle.ForeColor

        ' Colonna Nome Servizio
        dgvServices.Columns.Add(New DataGridViewTextBoxColumn With {
            .Name = "Service", .HeaderText = "Servizio", .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        })

        ' Colonna Stato
        dgvServices.Columns.Add(New DataGridViewTextBoxColumn With {
            .Name = "Status", .HeaderText = "Stato", .Width = 120, .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        })

        ' Colonna Azione (Bottone Toggle)
        Dim actionCol As New DataGridViewButtonColumn()
        actionCol.Name = "Action"
        actionCol.HeaderText = "Comando"
        actionCol.Width = 100
        actionCol.FlatStyle = FlatStyle.Flat
        dgvServices.Columns.Add(actionCol)
    End Sub

    Private Sub PopulateGrid()
        For Each s In servicesToMonitor
            Dim rowIndex = dgvServices.Rows.Add(s.ServiceName, "In controllo...", "START")
            dgvServices.Rows(rowIndex).Tag = s.ServiceName
        Next
    End Sub

    Private Sub TimerRefresh_Tick(sender As Object, e As EventArgs) Handles TimerRefresh.Tick
        For Each row As DataGridViewRow In dgvServices.Rows
            Dim serviceName = row.Tag.ToString()
            UpdateRowStatus(row, serviceName)
        Next
    End Sub

    Private Sub UpdateRowStatus(row As DataGridViewRow, serviceName As String)
        Try
            Using sc As New ServiceProcess.ServiceController(serviceName)
                Dim status = sc.Status
                row.Cells("Status").Value = status.ToString().ToUpper()

                ' Logica Colori e Testo Bottone
                Select Case status
                    Case ServiceControllerStatus.Running
                        row.Cells("Status").Style.ForeColor = Color.Green
                        row.Cells("Action").Value = "STOP"
                        row.Cells("Action").Style.BackColor = Color.LightCoral
                    Case ServiceControllerStatus.Stopped
                        row.Cells("Status").Style.ForeColor = Color.Red
                        row.Cells("Action").Value = "START"
                        row.Cells("Action").Style.BackColor = Color.LightGreen
                    Case Else
                        row.Cells("Status").Style.ForeColor = Color.Orange
                        row.Cells("Action").Value = "WAIT..."
                        row.Cells("Action").Style.BackColor = Color.LightGray
                End Select
            End Using
        Catch
            row.Cells("Status").Value = "NON INSTALLATO"
            row.Cells("Status").Style.ForeColor = Color.Gray
            row.Cells("Action").Value = "---"
        End Try
    End Sub

    Private Sub dgvServices_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvServices.CellContentClick
        ' Verifica se è stato cliccato il bottone nella colonna "Action"
        If e.RowIndex >= 0 AndAlso dgvServices.Columns(e.ColumnIndex).Name = "Action" Then
            Dim serviceName = dgvServices.Rows(e.RowIndex).Tag.ToString()
            ToggleService(serviceName)
        End If
    End Sub

    Private Sub ToggleService(serviceName As String)
        Try
            Using sc As New ServiceProcess.ServiceController(serviceName)
                If sc.Status = ServiceControllerStatus.Running Then
                    sc.Stop()
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10))
                ElseIf sc.Status = ServiceControllerStatus.Stopped Then
                    sc.Start()
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10))
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show($"Errore: {ex.Message}", "Controllo Servizio", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class