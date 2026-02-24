using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PERP_CommLibrary;
using PERP_API;

namespace PersonalERP_Client
{
    public partial class Form1 : Form
    {
        private static List<dynamic> Modules;
        private static ChannelFactory<IPERP_CommModel> channelFactory;
        private static IPERP_CommModel proxy;
        private static PERP_API_Contract APIProxy;
        private string sessionToken;
        private UserInfo currentUser;

        public Form1(PERP_API_Contract apiProxy, string token, UserInfo user)
        {
            Modules = new List<dynamic>();
            APIProxy = apiProxy;
            sessionToken = token;
            currentUser = user;

            InitializeComponent();

            this.Text = $"PersonalERP - {currentUser.DisplayName} ({currentUser.Role})";
            lblUser.Text = $"Logged in as: {currentUser.DisplayName} [{currentUser.Role}]";

            SetCommModelEndpointAddress("http://localhost:3740/endpoint");

            string ModulesDir = Path.Combine(Directory.GetCurrentDirectory(), "Modules");
            if (!Directory.Exists(ModulesDir))
                Directory.CreateDirectory(ModulesDir);

            foreach (string item in proxy.ListModules())
            {
                if (!APIProxy.HasPermission(sessionToken, item, "view"))
                    continue;

                listBox1.Items.Add(item);
                byte[] ModuleFile = proxy.DownloadModule(item);
                string ModulePath = Path.Combine(ModulesDir, item + ".dll");
                File.WriteAllBytes(ModulePath, ModuleFile);

                Assembly ModuleAssembly = Assembly.LoadFile(ModulePath);
                Type type = ModuleAssembly.GetTypes().ToList()
                    .FirstOrDefault(el => el.Name.Contains("PERP_Module"));
                dynamic c = Activator.CreateInstance(type);
                c.proxy = APIProxy;
                c.SessionToken = sessionToken;
                c.CurrentUser = currentUser;
                c.ClientMain();
                Modules.Add(c);
            }
        }

        public static void SetCommModelEndpointAddress(string newAddress)
        {
            EndpointIdentity spn = EndpointIdentity.CreateSpnIdentity("PERP_Endpoint");
            Uri uri = new Uri(newAddress);
            var address = new EndpointAddress(uri, spn);
            channelFactory = new ChannelFactory<IPERP_CommModel>("PERP_Endpoint", address);
            proxy = channelFactory.CreateChannel();
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
                return;

            string modName = listBox1.SelectedItem.ToString();
            var mod = Modules.FirstOrDefault(el => el.ModuleName == modName);
            if (mod != null)
                mod.EntryForm.Show();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            try { APIProxy.Logout(sessionToken); } catch { }
            Application.Restart();
        }
    }
}
