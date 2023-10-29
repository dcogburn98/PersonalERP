using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PERP_CommLibrary;

namespace PersonalERP_Server
{
    internal class PERP_CommModel : IPERP_CommModel
    {
        public static List<dynamic> Modules;

        public static void Initialize()
        {
            Modules = new List<dynamic>();
        }

        public byte[] DownloadModule(string ModuleName)
        {
            dynamic Module = Modules.FirstOrDefault(el => el.ModuleName == ModuleName);
            if (Module == default(dynamic))
                return null;

            string ModulesDir = Path.Combine(Directory.GetCurrentDirectory(), "Modules", Module.ModuleFileName);
            return File.ReadAllBytes(ModulesDir);
        }

        public List<string> ListModules()
        {
            List<string> names = new List<string>();
            foreach (dynamic module in Modules)
            {
                names.Add(module.ModuleName);
            }
            return names;
        }

        public void NotifyClose()
        {
            
        }
    }
}
