using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Data.Sqlite;

using PERP_CommLibrary;

namespace PersonalERP_Server
{
    internal class PERP_CommModel : IPERP_CommModel
    {
        public static List<dynamic> Modules;
        public static SqliteConnection sql;

        public static void Initialize(SqliteConnection sql_conn)
        {
            Modules = new List<dynamic>();
            sql = sql_conn;
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
