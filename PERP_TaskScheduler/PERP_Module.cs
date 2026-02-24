using System;
using System.Windows.Forms;
using PERP_API;

namespace PERP_TaskScheduler
{
    internal partial class PERP_Module : Module
    {
        public override string ModuleName => "TaskScheduler";
        public override string ModuleFileName => "PERP_TaskScheduler.dll";

        public override void ServerMain()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{ModuleName} successfully loaded on server.");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public override void Help()
        {
            Console.WriteLine("Task Scheduler - view, verify, and manage scheduled tasks.");
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
