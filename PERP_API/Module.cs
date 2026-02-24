using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PERP_API
{
    public abstract class Module
    {
        public abstract string ModuleFileName { get; }
        public abstract string ModuleName { get; }
        public abstract Form EntryForm { get; set; }
        public abstract PERP_API_Contract proxy { get; set; }

        public virtual string SessionToken { get; set; }
        public virtual UserInfo CurrentUser { get; set; }

        public abstract void ServerMain();
        public abstract void ClientMain();
        public abstract void Help();
    }
}
