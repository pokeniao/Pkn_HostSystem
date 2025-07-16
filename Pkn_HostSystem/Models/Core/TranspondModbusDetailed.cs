namespace Pkn_HostSystem.Models.Core
{
    public class TranspondModbusDetailed :TranspondDetailed
    {
        public string ConnectName { get; set; }
        public string SlaveAddress { get; set; } = "1";
        public string StartAddress { get; set; } = "0";
        public string Length { get; set; } = "1";

    }
}