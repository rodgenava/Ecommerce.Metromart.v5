using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface ISFTPPandamart
    {
        Task SendtoPAndamartsftp(string localFilePath = "");
        Task MoveToNewFileLocation();
    }
}
