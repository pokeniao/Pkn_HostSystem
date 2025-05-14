namespace Pkn_HostSystem.Models.Core
{
    public class TranspondModbusDetailed :TranspondDetailed
    {
        public string ConnectName { get; set; }
        public string SlaveAddress { get; set; }
        public string StartAddress { get; set; }
        public string Length { get; set; }

    }
}