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
        private static ChannelFactory<PERP_API_Contract> APIFactory;
        private static PERP_API_Contract APIProxy;

        public static void SetCommModelEndpointAddress(string newAddress)
        {
            EndpointIdentity spn = EndpointIdentity.CreateSpnIdentity("PERP_Endpoint");
            Uri uri = new Uri(newAddress);
            var address = new EndpointAddress(uri, spn);
            channelFactory = new ChannelFactory<IPERP_CommModel>("PERP_Endpoint", address);
            proxy = channelFactory.CreateChannel();
        }

        public static void SetAPIEndpointAddress(string newAddress)
        {
            EndpointIdentity spn = EndpointIdentity.CreateSpnIdentity("PERP_API_Endpoint");
            Uri uri = new Uri(newAddress);
            var address = new EndpointAddress(uri, spn);
            APIFactory = new ChannelFactory<PERP_API_Contract>("PERP_API_Endpoint", address);
            APIProxy = APIFactory.CreateChannel();
        }

        public Form1()
        {
            Modules = new List<dynamic>();

            InitializeComponent();
            SetCommModelEndpointAddress("http://localhost:3740/endpoint");
            SetAPIEndpointAddress("http://localhost:3443/endpoint");

            string ModulesDir = Path.Combine(Directory.GetCurrentDirectory(), "Modules");
            if (!Directory.Exists(ModulesDir))
            {
                Directory.CreateDirectory(ModulesDir);
            }

            foreach (string item in proxy.ListModules())
            {
                listBox1.Items.Add(item);
                byte[] ModuleFile = proxy.DownloadModule(item);
                string ModulePath = Path.Combine(Directory.GetCurrentDirectory(), "Modules", item + ".dll");
                File.WriteAllBytes(ModulePath, ModuleFile);

                Assembly ModuleAssembly = Assembly.LoadFile(ModulePath);
                Type type = ModuleAssembly.GetTypes().ToList().FirstOrDefault(el => el.Name.Contains("PERP_Module"));
                dynamic c = Activator.CreateInstance(type);
                c.proxy = APIProxy;
                c.ClientMain();
                Modules.Add(c);
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
                return;

            Modules.FirstOrDefault(el => el.ModuleName == listBox1.SelectedItem.ToString()).EntryForm.Show();
        }
    }
}
