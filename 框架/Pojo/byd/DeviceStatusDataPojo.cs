using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame
{
    public class DeviceStatusDataPojo
    {
        public string deviceCode { get; set; }
        public string errorCode { get; set; }
        public string errorName { get; set; }
        public string lineCode { get; set; }
        public DeviceParams paramsList { get; set; }
        public string stationCode { get; set; }
        public int status { get; set; }

        public string time { get; set; }

    
    }
}