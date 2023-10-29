using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PERP_API;

namespace PERP_Accounting
{
    public partial class MainForm : Form
    {
        private PERP_API_Contract api;

        public MainForm()
        {
            InitializeComponent();
        }

        public void SetProxy(PERP_API_Contract proxy)
        {
            api = proxy;
            api.Log("How bout here?");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            api.Log("Accounting module can communicate with the server API.");
        }
    }
}