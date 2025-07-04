using Pkn_HostSystem.Base;

namespace Pkn_HostSystem.Models.Mapper
{
    public class ScanQR :BaseMapper<ScanQR>
    {
        public long id { get; set; }

        public string qr_code { get; set; }

        public string orderCode { get; set; }

        public string materialCode { get; set; }

        public string CT { get; set; }

        public char Pass { get; set; }
    }
}