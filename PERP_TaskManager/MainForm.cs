using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using PERP_API;

namespace PERP_TaskManager
{
    public partial class MainForm : Form
    {
        private PERP_API_Contract api;
        private const string TABLE_NAME = "perp_tasks";

        public MainForm(PERP_API_Contract proxy)
        {
            InitializeComponent();
            api = proxy;
            FormClosing += MainForm_FormClosing;

            EnsureTableExists();
            cmbFilter.SelectedIndex = 0;
            RefreshTasks();
        }

        private void EnsureTableExists()
        {
            string createSql =
                $"CREATE TABLE IF NOT EXISTS {TABLE_NAME} (" +
                "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "title TEXT NOT NULL, " +
                "description TEXT, " +
                "priority TEXT NOT NULL DEFAULT 'Medium', " +
                "status TEXT NOT NULL DEFAULT 'Open', " +
                "created_date TEXT NOT NULL, " +
                "due_date TEXT)";
            api.DB_ExecuteNonQuery(createSql);
        }

        private void RefreshTasks()
        {
            string filter = cmbFilter.SelectedItem?.ToString() ?? "All";
            string sql;

            if (filter == "All")
                sql = $"SELECT id AS ID, title AS Title, description AS Description, " +
                      $"priority AS Priority, status AS Status, created_date AS Created, " +
                      $"due_date AS Due FROM {TABLE_NAME} ORDER BY " +
                      "CASE priority WHEN 'High' THEN 1 WHEN 'Medium' THEN 2 WHEN 'Low' THEN 3 END, id DESC";
            else
                sql = $"SELECT id AS ID, title AS Title, description AS Description, " +
                      $"priority AS Priority, status AS Status, created_date AS Created, " +
                      $"due_date AS Due FROM {TABLE_NAME} WHERE status = '{filter.Replace("'", "''")}' ORDER BY " +
                      "CASE priority WHEN 'High' THEN 1 WHEN 'Medium' THEN 2 WHEN 'Low' THEN 3 END, id DESC";

            try
            {
                DataTable dt = api.DB_ExecuteQuery(sql);
                dgvTasks.DataSource = dt;

                if (dgvTasks.Columns.Contains("ID"))
                    dgvTasks.Columns["ID"].Width = 40;

                lblCount.Text = $"{dt.Rows.Count} task(s)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tasks: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string title = txtTitle.Text.Trim();
            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Please enter a task title.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string desc = txtDescription.Text.Trim().Replace("'", "''");
            string priority = cmbPriority.SelectedItem?.ToString() ?? "Medium";
            string dueDate = dtpDue.Checked ? dtpDue.Value.ToString("yyyy-MM-dd") : "";
            string createdDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string sql = $"INSERT INTO {TABLE_NAME} (title, description, priority, status, created_date, due_date) " +
                         $"VALUES ('{title.Replace("'", "''")}', '{desc}', '{priority}', 'Open', '{createdDate}', '{dueDate}')";

            try
            {
                api.DB_ExecuteNonQuery(sql);
                api.Log($"[TaskManager] Created task: {title}");
                ClearInputFields();
                RefreshTasks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding task: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnMarkDone_Click(object sender, EventArgs e)
        {
            UpdateSelectedTaskStatus("Completed");
        }

        private void btnMarkInProgress_Click(object sender, EventArgs e)
        {
            UpdateSelectedTaskStatus("In Progress");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvTasks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a task to delete.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show("Delete the selected task?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                string id = dgvTasks.SelectedRows[0].Cells["ID"].Value.ToString();
                try
                {
                    api.DB_ExecuteNonQuery($"DELETE FROM {TABLE_NAME} WHERE id = {id}");
                    api.Log($"[TaskManager] Deleted task ID: {id}");
                    RefreshTasks();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting task: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateSelectedTaskStatus(string newStatus)
        {
            if (dgvTasks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a task.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string id = dgvTasks.SelectedRows[0].Cells["ID"].Value.ToString();
            try
            {
                api.DB_ExecuteNonQuery(
                    $"UPDATE {TABLE_NAME} SET status = '{newStatus}' WHERE id = {id}");
                api.Log($"[TaskManager] Task {id} marked as: {newStatus}");
                RefreshTasks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating task: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshTasks();
        }

        private void ClearInputFields()
        {
            txtTitle.Text = "";
            txtDescription.Text = "";
            cmbPriority.SelectedIndex = 1;
            dtpDue.Checked = false;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}
