Imports System.ServiceProcess
Imports CommonSim

Public Class ServiceControllerForm
    Dim _dispatchOrderRepository As OrderDispatchRepository
    Private splitMain As SplitContainer

    Private ReadOnly servicesToMonitor As New List(Of ServiceInfo) From {
        New ServiceInfo("WMS.Sim", "Warehouse WMS Service"),
        New ServiceInfo("WCS.Sim", "Warehouse WCS Service"),
        New ServiceInfo("PLC.Sim", "PLC Simulator Service"),
        New ServiceInfo("HOST.Sim", "Host Polling Service")
    }

    Private ReadOnly orderStatusToMonitor As New List(Of OrderInfo) From {
        New OrderInfo("PENDING"),
        New OrderInfo("IN_PROGRESS"),
        New OrderInfo("COMPLETED"),
        New OrderInfo("ARCHIVED"),
        New OrderInfo("FAILED")}

    '-------------------------------------------------------------------------------
    '- Inizializzazione Form
    Private Sub ServiceControllerForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupServiceDataGridView()
        PopulateServiceGrid()

        SetupOrderCounterDataGridView()
        PopulateOrderCounterGrid()

        Dim cs As String = ConnectionStringProvider.GetConnectionString(Environment.GetCommandLineArgs)

        If cs Is Nothing Then
            _dispatchOrderRepository = Nothing
        Else
            _dispatchOrderRepository = New OrderDispatchRepository(cs)
        End If

        ' Configura il Timer (frequenza 2 secondi)
        TimerRefresh.Interval = 2000
        TimerRefresh.Start()

    End Sub

    '-------------------------------------------------------------------------------
    '- Setup griglia dei servizi
    Private Sub SetupServiceDataGridView()
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
        Dim actionCol As New DataGridViewButtonColumn With {
            .Name = "Action",
            .HeaderText = "Comando",
            .Width = 100,
            .FlatStyle = FlatStyle.Flat}
        dgvServices.Columns.Add(actionCol)
    End Sub

    '-------------------------------------------------------------------------------
    '- Setup griglia dei contatori
    Private Sub SetupOrderCounterDataGridView()
        dgvServices.Dock = DockStyle.Fill
        dgvOrdersCounter.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvOrdersCounter.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
        dgvOrdersCounter.AllowUserToAddRows = False
        dgvOrdersCounter.AllowUserToResizeRows = False
        dgvOrdersCounter.RowHeadersVisible = False
        dgvOrdersCounter.ReadOnly = True
        dgvOrdersCounter.Enabled = True
        dgvOrdersCounter.SelectionMode = DataGridViewSelectionMode.CellSelect
        dgvOrdersCounter.MultiSelect = False

        dgvOrdersCounter.DefaultCellStyle.SelectionBackColor = dgvOrdersCounter.DefaultCellStyle.BackColor
        dgvOrdersCounter.DefaultCellStyle.SelectionForeColor = dgvOrdersCounter.DefaultCellStyle.ForeColor

        dgvOrdersCounter.ScrollBars = ScrollBars.None

        dgvOrdersCounter.Columns.Add(New DataGridViewTextBoxColumn With {
            .Name = "OrderStatus", .HeaderText = "Stato Ordine", .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        })

        dgvOrdersCounter.Columns.Add(New DataGridViewTextBoxColumn With {
            .Name = "Count", .HeaderText = "Contatore", .Width = 100, .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        })
    End Sub

    Private Sub PopulateServiceGrid()
        For Each s In servicesToMonitor
            Dim rowIndex = dgvServices.Rows.Add(s.ServiceName, "In controllo...", "START")
            dgvServices.Rows(rowIndex).Tag = s.ServiceName
        Next
    End Sub

    Private Sub PopulateOrderCounterGrid()
        For Each o In orderStatusToMonitor
            Dim rowIndex = dgvOrdersCounter.Rows.Add(o.ServiceStatus, 0)
            dgvOrdersCounter.Rows(rowIndex).Tag = o.ServiceStatus
        Next
    End Sub

    '-------------------------------------------------------------------------------
    '- Refresh periodico dello stato dei servizi e dei contatori
    Private Sub TimerRefresh_Tick(sender As Object, e As EventArgs) Handles TimerRefresh.Tick
        For Each row As DataGridViewRow In dgvServices.Rows
            Dim serviceName = row.Tag.ToString()
            UpdateRowStatus(row, serviceName)
        Next

        UpdateOrderCounters()
    End Sub

    '-------------------------------------------------------------------------------
    '- Aggiorna lo stato di un servizio e la logica del bottone
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

    '-------------------------------------------------------------------------------
    '- Aggiorna i contatori degli ordini per ogni stato
    Private Sub UpdateOrderCounters()
        If _dispatchOrderRepository Is Nothing Then
            For Each row As DataGridViewRow In dgvOrdersCounter.Rows
                row.Cells("Count").Value = "-"
            Next
            Return
        End If

        For Each row As DataGridViewRow In dgvOrdersCounter.Rows
            Dim status = row.Tag.ToString()
            Try
                Dim count = _dispatchOrderRepository.CountByStatus(status)
                row.Cells("Count").Value = count
            Catch
                row.Cells("Count").Value = "ERR"
            End Try
        Next
    End Sub

    '-------------------------------------------------------------------------------
    '- Gestione Click bottone Start/Stop
    Private Sub dgvServices_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvServices.CellContentClick
        If e.RowIndex >= 0 AndAlso dgvServices.Columns(e.ColumnIndex).Name = "Action" Then
            Dim serviceName = dgvServices.Rows(e.RowIndex).Tag.ToString()
            ToggleService(serviceName)
        End If
    End Sub

    '-------------------------------------------------------------------------------
    '- Start/Stop del servizio selezionato
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