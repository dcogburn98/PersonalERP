using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using PERP_API;

namespace PERP_TaskScheduler
{
    public partial class MainForm : Form
    {
        private PERP_API_Contract api;
        private string sessionToken;
        private UserInfo currentUser;

        public MainForm(PERP_API_Contract proxy, string sessionToken, UserInfo currentUser)
        {
            InitializeComponent();
            api = proxy;
            this.sessionToken = sessionToken;
            this.currentUser = currentUser;

            FormClosing += MainForm_FormClosing;
            dgvTasks.SelectionChanged += DgvTasks_SelectionChanged;
            btnRefresh.Click += BtnRefresh_Click;
            btnEnable.Click += BtnEnable_Click;
            btnRunNow.Click += BtnRunNow_Click;
            btnDelete.Click += BtnDelete_Click;

            if (currentUser.Role != "Admin")
            {
                btnRunNow.Enabled = false;
                btnDelete.Enabled = false;
            }

            RefreshTasks();
        }

        private void RefreshTasks()
        {
            try
            {
                DataTable dt = api.ListScheduledTasks(sessionToken);
                dgvTasks.DataSource = dt;

                if (dgvTasks.Rows.Count > 0)
                {
                    dgvTasks.Rows[0].Selected = true;
                    LoadSelectedTaskDetails();
                }
                else
                {
                    ClearDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tasks: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvTasks_SelectionChanged(object sender, EventArgs e)
        {
            LoadSelectedTaskDetails();
        }

        private void LoadSelectedTaskDetails()
        {
            if (dgvTasks.SelectedRows.Count == 0)
            {
                ClearDetails();
                return;
            }

            DataGridViewRow row = dgvTasks.SelectedRows[0];
            DataRowView drv = row.DataBoundItem as DataRowView;
            if (drv == null)
            {
                ClearDetails();
                return;
            }

            DataRow dr = drv.Row;

            lblTaskNameValue.Text = dr.Table.Columns.Contains("TaskName")
                ? dr["TaskName"].ToString() : "";
            lblModuleValue.Text = dr.Table.Columns.Contains("ModuleName")
                ? dr["ModuleName"].ToString() : "";
            lblActionTypeValue.Text = dr.Table.Columns.Contains("ActionType")
                ? dr["ActionType"].ToString() : "";
            lblIntervalValue.Text = dr.Table.Columns.Contains("IntervalSeconds")
                ? dr["IntervalSeconds"].ToString() + "s" : "";
            txtActionData.Text = dr.Table.Columns.Contains("ActionData")
                ? dr["ActionData"].ToString() : "";

            bool enabled = dr.Table.Columns.Contains("Enabled")
                && Convert.ToBoolean(dr["Enabled"]);
            btnEnable.Text = enabled ? "Disable" : "Enable";

            LoadTaskHistory(dr);
        }

        private void LoadTaskHistory(DataRow dr)
        {
            try
            {
                if (!dr.Table.Columns.Contains("Id"))
                {
                    dgvHistory.DataSource = null;
                    return;
                }

                int taskId = Convert.ToInt32(dr["Id"]);
                DataTable history = api.GetTaskHistory(sessionToken, taskId, 50);
                dgvHistory.DataSource = history;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading task history: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearDetails()
        {
            lblTaskNameValue.Text = "";
            lblModuleValue.Text = "";
            lblActionTypeValue.Text = "";
            lblIntervalValue.Text = "";
            txtActionData.Text = "";
            btnEnable.Text = "Enable";
            dgvHistory.DataSource = null;
        }

        private int GetSelectedTaskId()
        {
            if (dgvTasks.SelectedRows.Count == 0) return -1;
            DataRowView drv = dgvTasks.SelectedRows[0].DataBoundItem as DataRowView;
            if (drv == null || !drv.Row.Table.Columns.Contains("Id")) return -1;
            return Convert.ToInt32(drv.Row["Id"]);
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshTasks();
        }

        private void BtnEnable_Click(object sender, EventArgs e)
        {
            int taskId = GetSelectedTaskId();
            if (taskId < 0) return;

            try
            {
                DataRowView drv = dgvTasks.SelectedRows[0].DataBoundItem as DataRowView;
                bool currentlyEnabled = drv.Row.Table.Columns.Contains("Enabled")
                    && Convert.ToBoolean(drv.Row["Enabled"]);

                api.SetTaskEnabled(sessionToken, taskId, !currentlyEnabled);
                RefreshTasks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error toggling task: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRunNow_Click(object sender, EventArgs e)
        {
            int taskId = GetSelectedTaskId();
            if (taskId < 0) return;

            try
            {
                api.RunTaskNow(sessionToken, taskId);
                RefreshTasks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error running task: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            int taskId = GetSelectedTaskId();
            if (taskId < 0) return;

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this scheduled task?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            try
            {
                api.UnregisterScheduledTask(sessionToken, taskId);
                RefreshTasks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting task: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}
