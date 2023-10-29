using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PERP_API
{
    public abstract partial class Module //Use this partial class for server methods and properties
    {
        public abstract string ModuleFileName { get; }
        public abstract string ModuleName { get; }
        public abstract Form EntryForm { get; set; }
        public abstract PERP_API_Contract proxy { get; set; }

        public abstract void ServerMain();

        public abstract void ClientMain();

        public abstract void Help();
    }
}