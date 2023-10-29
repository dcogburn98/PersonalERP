using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PERP_CommLibrary
{
    [ServiceContract]
    public interface IPERP_CommModel
    {
        [OperationContract]
        List<string> ListModules();

        [OperationContract]
        void NotifyClose();

        [OperationContract]
        byte[] DownloadModule(string ModuleName);
    }
}
