using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PERP_API;

namespace PersonalERP_Server
{
    internal class PERP_APIModel : PERP_API_Contract
    {
        public void Log(string Message)
        {
            Console.WriteLine(Message);
        }
    }
}
