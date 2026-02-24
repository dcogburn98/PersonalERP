using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PERP_API;

namespace PERP_DatabaseBrowser
{
    public partial class MainForm : Form
    {
        private PERP_API_Contract api;
        private string sessionToken;
        private UserInfo currentUser;

        public MainForm(PERP_API_Contract proxy, string token = null, UserInfo user = null)
        {
            InitializeComponent();
            api = proxy;
            sessionToken = token;
            currentUser = user;
            FormClosing += MainForm_FormClosing;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            btnRefresh.Click += btnRefresh_Click;
            btnSearch.Click += btnSearch_Click;
            txtSearch.KeyDown += txtSearch_KeyDown;

            RefreshTableList();
        }

        private void RefreshTableList()
        {
            comboBox1.Items.Clear();
            List<string> tables = api.DB_ListTables();
            foreach (string tableName in tables)
            {
                comboBox1.Items.Add(tableName);
            }

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            else
            {
                dataGridView1.DataSource = null;
                lblRowCount.Text = "No tables found";
            }
        }

        private void LoadTableData(string tableName)
        {
            try
            {
                DataTable dt = api.DB_GetTableSchema(tableName);
                dataGridView1.DataSource = dt;
                lblRowCount.Text = $"{dt.Rows.Count} row(s)";
                this.Text = $"Database Browser - [{tableName}]";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading table: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                txtSearch.Text = "";
                LoadTableData(comboBox1.SelectedItem.ToString());
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshTableList();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ApplyFilter();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void ApplyFilter()
        {
            if (comboBox1.SelectedItem == null || dataGridView1.DataSource == null)
                return;

            DataTable dt = dataGridView1.DataSource as DataTable;
            if (dt == null) return;

            string filter = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(filter))
            {
                dt.DefaultView.RowFilter = "";
                lblRowCount.Text = $"{dt.Rows.Count} row(s)";
                return;
            }

            StringBuilder sb = new StringBuilder();
            foreach (DataColumn col in dt.Columns)
            {
                if (col.DataType == typeof(string))
                {
                    if (sb.Length > 0) sb.Append(" OR ");
                    sb.Append($"[{col.ColumnName}] LIKE '%{filter.Replace("'", "''")}%'");
                }
            }

            if (sb.Length > 0)
            {
                try
                {
                    dt.DefaultView.RowFilter = sb.ToString();
                    lblRowCount.Text = $"{dt.DefaultView.Count} of {dt.Rows.Count} row(s)";
                }
                catch
                {
                    dt.DefaultView.RowFilter = "";
                    lblRowCount.Text = $"{dt.Rows.Count} row(s)";
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}
