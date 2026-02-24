using System;
using System.Windows.Forms;
using PERP_API;

namespace PersonalERP_Client
{
    public partial class LoginForm : Form
    {
        private PERP_API_Contract api;
        public string SessionToken { get; private set; }
        public UserInfo CurrentUser { get; private set; }

        public LoginForm(PERP_API_Contract apiProxy)
        {
            InitializeComponent();
            api = apiProxy;
            AcceptButton = btnLogin;
            txtUsername.Text = "admin";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                lblStatus.Text = "Please enter username and password.";
                return;
            }

            lblStatus.Text = "Authenticating...";
            btnLogin.Enabled = false;

            try
            {
                string token = api.Authenticate(user, pass);
                if (string.IsNullOrEmpty(token))
                {
                    lblStatus.Text = "Invalid username or password.";
                    btnLogin.Enabled = true;
                    return;
                }

                SessionToken = token;
                CurrentUser = api.GetCurrentUser(token);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Connection error: " + ex.Message;
                btnLogin.Enabled = true;
            }
        }
    }
}
