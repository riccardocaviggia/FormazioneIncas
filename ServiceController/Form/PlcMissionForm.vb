Imports CommonSim

Public Class PlcMissionForm
    Private cs As String
    Private Sub PlcMissionForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cs = ConnectionStringProvider.GetConnectionString(Environment.GetCommandLineArgs)

        SetupPlcMissionGrid()

        Timer1.Interval = 2000
        Timer1.Start()
    End Sub

    Private Sub SetupPlcMissionGrid()
        dgvPlcMission.Dock = DockStyle.Fill
        dgvPlcMission.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvPlcMission.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
        dgvPlcMission.AllowUserToAddRows = False
        dgvPlcMission.AllowUserToResizeRows = False
        dgvPlcMission.RowHeadersVisible = False
        dgvPlcMission.ReadOnly = True
        dgvPlcMission.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvPlcMission.MultiSelect = False

        dgvPlcMission.DefaultCellStyle.SelectionBackColor = dgvPlcMission.DefaultCellStyle.BackColor
        dgvPlcMission.DefaultCellStyle.SelectionForeColor = dgvPlcMission.DefaultCellStyle.ForeColor

        dgvPlcMission.Columns.Clear()


    End Sub
End Class