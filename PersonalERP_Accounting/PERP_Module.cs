using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using PERP_API;

namespace PERP_Accounting
{
    internal partial class PERP_Module : Module
    {
        public override string ModuleName => "Accounting"; //The module name as listed on clients' windows.
        public override string ModuleFileName => "PERP_Accounting.dll"; //The actual filename as compiled and put in the server's "modules" folder

        /// <summary>
        /// This method can be edited by the module creator. It can be treated as a "Main" method in a console application.
        /// </summary>
        public override void ServerMain()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{ModuleName} successfully loaded on server.");
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// This method runs when the '{ModuleName] help' command is run on the server. It can simply print text and return.
        /// </summary>
        public override void Help()
        {
            Console.WriteLine("Welcome to my new module!");
        }
    }

    /// <summary>
    /// These methods and properties should not be changed, assigned to, or otherwise changed in any way by the module creator.
    /// Only make changes to the code in this part of the class if you fully understand the client and server code. Most things
    /// can be done without ever making any changes here.
    /// </summary>
    internal partial class PERP_Module
    {
        public override PERP_API_Contract proxy { get; set; }
        public override Form EntryForm { get; set; }

        /// <summary>
        /// This method should not be edited by the module creator. Any code should be added within the MainForm code or
        /// added as classes referred to by the MainForm.
        /// </summary>
        public override void ClientMain()
        {
            EntryForm = new MainForm();
            (EntryForm as MainForm).SetProxy(proxy);
        }
    }
}