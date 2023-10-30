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
            //Create the modules folder if it doesn't exist already
            string ModulesDir = Path.Combine(Directory.GetCurrentDirectory(), "Modules");
            if (!Directory.Exists(ModulesDir))
            {
                Directory.CreateDirectory(ModulesDir);
                Console.WriteLine("'Modules' directory created successfully. You can add modules at " + ModulesDir);
                Console.WriteLine("It is recommended to do that, this software doesn't do much of anything without modules.");
            }

            //Initialize the database connection with SQLite
            sql = new SqliteConnection(
              @"Data Source=database.db; 
                Pooling = true;");
            PERP_CommModel.Initialize(sql);
            PERP_APIModel.Initialize(sql);
            sql.Open();
            sql.Close();
            Console.WriteLine("Database initialized.");

            //Obtain the external IP address of the server
            string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();
            IPAddress externalIp = IPAddress.Parse(externalIpString);

            //Load all valid modules
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
                    Console.WriteLine();
                    continue;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Succesfully loaded module: " + Path.GetFileName(module));
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Here is a list of assemblies referenced by this module:");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    foreach (AssemblyName str in Module.GetReferencedAssemblies())
                        Console.WriteLine("  " + str.Name);
                    Console.ForegroundColor = ConsoleColor.White;

                    dynamic c = Activator.CreateInstance(type);
                    c.ServerMain();
                    PERP_CommModel.Modules.Add(c);
                }
            }

            Console.WriteLine("Starting PERP client service...");
            ServiceHost host = new ServiceHost(typeof(PERP_CommModel));
            host.Open();
            Console.WriteLine("Server is open for connections from PERP clients.");

            Console.WriteLine("Starting PERP API service...");
            ServiceHost APIHost = new ServiceHost(typeof(PERP_APIModel));
            APIHost.Open();
            Console.WriteLine("API service is open for connections from PERP modules.");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Internal IP: " + GetLocalIPAddress());
            Console.WriteLine("External IP: " + externalIp.ToString());
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

            Console.ReadLine();
        }

        public static string GetLocalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
