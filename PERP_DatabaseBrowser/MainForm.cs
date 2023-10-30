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

        public MainForm(PERP_API_Contract proxy)
        {
            InitializeComponent();
            api = proxy;
            FormClosing += MainForm_FormClosing;

            foreach (string TableName in api.DB_ListTables())
            {
                comboBox1.Items.Add(TableName);
            }

            comboBox1.SelectedIndex = 0;
            dataGridView1.DataSource = api.DB_GetTableSchema(comboBox1.SelectedItem.ToString());
        }

        /// <summary>
        /// This method keeps the form from disposing when it is closed. Required to 
        /// keep the "EntryForm" property alive in the PERP_Module class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}
