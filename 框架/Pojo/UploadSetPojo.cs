using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame
{
    public  class UploadSetPojo
    {
        public string plcIp { get; set; }

        public string MesIp { get; set; }
        public string deviceCode { get; set; }
        public string lineCode { get; set; }
        public string stationCode { get; set; }

        public int upload { get; set; } 

        public int plcStatus { get; set; }

        public int plcPort { get; set; }

        public ushort hread { get; set; }
    }
}
