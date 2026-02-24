using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PersonalERP_Server
{
    internal class Program
    {
        public static List<string> Modules = new List<string>();
        public static SqliteConnection sql;

        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║        PersonalERP Server            ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.ForegroundColor = ConsoleColor.White;

            string ModulesDir = Path.Combine(Directory.GetCurrentDirectory(), "Modules");
            if (!Directory.Exists(ModulesDir))
            {
                Directory.CreateDirectory(ModulesDir);
                Console.WriteLine("'Modules' directory created at " + ModulesDir);
            }

            sql = new SqliteConnection(@"Data Source=database.db; Pooling = true;");
            PERP_CommModel.Initialize(sql);
            PERP_APIModel.Initialize(sql);
            AuthManager.Initialize(sql);
            TaskSchedulerEngine.Initialize(sql);

            sql.Open();
            sql.Close();
            Console.WriteLine("Database connection verified.");

            SchemaManager.InitializeSchema(sql);
            SchemaManager.SeedDefaults(sql);

            SeedDefaultScheduledTasks();

            foreach (string module in Directory.EnumerateFiles(ModulesDir))
            {
                if (!module.EndsWith(".dll"))
                    continue;

                var Module = Assembly.LoadFile(module);
                Type type = Module.GetTypes().ToList().FirstOrDefault(el => el.Name.Contains("PERP_Module"));
                if (type == default)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid module: " + Path.GetFileName(module));
                    Console.ForegroundColor = ConsoleColor.White;
                    continue;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Loaded module: " + Path.GetFileName(module));
                Console.ForegroundColor = ConsoleColor.White;

                dynamic c = Activator.CreateInstance(type);
                c.ServerMain();
                PERP_CommModel.Modules.Add(c);
            }

            Console.WriteLine("Starting PERP client service...");
            ServiceHost host = new ServiceHost(typeof(PERP_CommModel));
            host.Open();
            Console.WriteLine("Client service open on port 3740.");

            Console.WriteLine("Starting PERP API service...");
            ServiceHost APIHost = new ServiceHost(typeof(PERP_APIModel));
            APIHost.Open();
            Console.WriteLine("API service open on port 3443.");

            TaskSchedulerEngine.Start();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Internal IP: " + GetLocalIPAddress());
            try
            {
                string externalIpString = new WebClient()
                    .DownloadString("http://icanhazip.com")
                    .Replace("\\r\\n", "").Replace("\\n", "").Trim();
                Console.WriteLine("External IP: " + externalIpString);
            }
            catch { Console.WriteLine("External IP: (unavailable)"); }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Server running. Press Enter to stop.");
            Console.ReadLine();

            TaskSchedulerEngine.Stop();
        }

        private static void SeedDefaultScheduledTasks()
        {
            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM perp_scheduled_tasks";
                long count = (long)cmd.ExecuteScalar();
                if (count > 0) return;
            }
            finally { sql.Close(); }

            TaskSchedulerEngine.Register(
                "Session Cleanup", "System", "Remove expired login sessions",
                "CLEANUP_SESSIONS", "", 3600, null);

            TaskSchedulerEngine.Register(
                "History Cleanup", "System", "Remove task history older than 30 days",
                "CLEANUP_HISTORY", "30", 86400, null);

            Console.WriteLine("Default scheduled tasks registered.");
        }

        public static string GetLocalIPAddress()
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                        return ip.ToString();
            }
            catch { }
            return "127.0.0.1";
        }
    }
}
