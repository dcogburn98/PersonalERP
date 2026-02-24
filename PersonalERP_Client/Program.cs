using System;
using System.ServiceModel;
using System.Windows.Forms;
using PERP_API;

namespace PersonalERP_Client
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            PERP_API_Contract apiProxy;
            try
            {
                var factory = new ChannelFactory<PERP_API_Contract>("PERP_API_Endpoint");
                apiProxy = factory.CreateChannel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot connect to server:\n" + ex.Message,
                    "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var loginForm = new LoginForm(apiProxy);
            if (loginForm.ShowDialog() != DialogResult.OK)
                return;

            Application.Run(new Form1(apiProxy, loginForm.SessionToken, loginForm.CurrentUser));
        }
    }
}
