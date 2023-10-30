# PersonalERP
PersonalERP is a modular system whose name is a little misleading. It can be used as an ERP, and that is the type of modules I will be developing for it. It can be used in an enterprise setting, or it could be used in your personal life to assist with anything. It all depends on the installed modules.

PersonalERP is only a software that provides a method of centralizing all components and data contained in the system. It allows you to have a server distributing modules to clients connected over a network.

## Creating Modules
PersonalERP makes it easy to create new modules that can be added on the server and automatically distributed to clients. Not only that, but modules have a UI that is designed with the familiar WinForms designer. Here are the simple steps to creating a module:

#### Create a new WinForms project
A module basically runs as an independent program, but there are going to be a few changes you must make before you can use it in PersonalERP. So create a WinForms project, then rename the Form1 to MainForm, and rename Program.cs to "PERP_Module.cs". Add a reference to the PERP_API.dll that is found in your server installation's "Modules" folder. Add this using statement to both the MainForm and "PERP_Module.cs":
``` C#
using PERP_API;
```

#### Replace your MainForm class with the following:
``` C#
public partial class MainForm : Form
    {
        private PERP_API_Contract api;

        public MainForm(PERP_API_Contract proxy)
        {
            InitializeComponent();
            api = proxy;
            FormClosing += MainForm_FormClosing;
        }

        /// <summary>
        /// This method keeps the form from disposing when it is closed. Required to 
        /// keep the "EntryForm" property alive in the PERP_Module class.
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
```

#### Replace your "PERP_Module.cs" class with this code:
``` C#
    /// <summary>
    /// These methods and properties can be edited as seen fit by the module creator.
    /// </summary>
    internal partial class PERP_Module : Module
    {
        //The module name as listed on clients' windows.
        public override string ModuleName => "Accounting";

        //The actual filename as compiled and put in the server's "modules" folder
        public override string ModuleFileName => "PERP_Accounting.dll";

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
            Console.WriteLine("Accounting module!");
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
            EntryForm = new MainForm(proxy);
        }
    }
```

#### Change your project settings
Go to your project settings for your module and change the "Output Type" to "Class Library". Now when you compile your code, you will get a dll that can be put in the "Modules" folder of your server installation. Or if you have the entire client and server project loaded into visual studio, set the output path of your module to the the bin/modules folder of the server project. Then building your module will automatically be "installed" for the server when you run the project.

The PERP_DatabaseBrowser project is a good place to start digging and finding out exactly how a module works and interfaces with the client and server separately.
