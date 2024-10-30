using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class SFTPConnection
    {
        public SFTPInfo SFTPinfo { get; set; }
        public SFTPCredential Credential { get; set; }
        public SFTPDirectory Directory { get; set; }
    }
}
