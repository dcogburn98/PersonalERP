using System;
using System.Collections.Generic;
using System.Data;
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

        [OperationContract]
        List<string> DB_ListTables();

        [OperationContract]
        DataTable DB_GetTableSchema(string TableName);
    }
}