using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCPConManageDll
{
    public class TCPConnectionItem
    {
        public string othersideIp { get; set; }
        public int othersidePort { get; set; }
        public string state { get; set; }
    }
}
