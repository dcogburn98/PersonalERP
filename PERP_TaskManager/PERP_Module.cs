using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using PERP_API;

namespace PERP_TaskManager
{
    internal partial class PERP_Module : Module
    {
        public override string ModuleName => "TaskManager";
        public override string ModuleFileName => "PERP_TaskManager.dll";

        public override void ServerMain()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{ModuleName} successfully loaded on server.");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public override void Help()
        {
            Console.WriteLine("Task Manager module - manage tasks with priorities and statuses.");
        }
    }

    internal partial class PERP_Module
    {
        public override PERP_API_Contract proxy { get; set; }
        public override Form EntryForm { get; set; }

        public override void ClientMain()
        {
            EntryForm = new MainForm(proxy, SessionToken, CurrentUser);
        }
    }
}
