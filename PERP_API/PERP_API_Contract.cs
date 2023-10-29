using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PERP_API
{
    [ServiceContract]
    public interface PERP_API_Contract
    {
        [OperationContract]
        void Log(string Message);
    }
}