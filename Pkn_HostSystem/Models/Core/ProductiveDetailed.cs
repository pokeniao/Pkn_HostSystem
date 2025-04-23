namespace Pkn_HostSystem.Models.Core
{
    /// <summary>
    /// 只能读寄存器
    /// </summary>
    public class ProductiveDetailed
    {
        /// <summary>
        /// 生产者站地址
        /// </summary>
        public string ProducterStationAddress { get; set; } = "1";

        /// <summary>
        /// 生产者起始地址
        /// </summary>
        public string ProducterStartAddress { get; set; } = "0";

        /// <summary>
        /// 生产者寄存器长度
        /// </summary>
        public string ProducterLength { get; set; } = "1";

        /// <summary>
        /// 消费者站地址
        /// </summary>
        public string ConsumerStationAddress { get; set; } = "1";

        /// <summary>
        /// 消费者起始地址
        /// </summary>
        public string ConsumerStartAddress { get; set; } = "0";

        /// <summary>
        /// 消费者存储长度
        /// </summary>
        public string ConsumerLength { get; set; } = "1";
        /// <summary>
        /// 存储消费的内容
        /// </summary>
        public ushort[] Message { get; set; }

        public string ShowProducterValue
        {
            get
            {
                return $"站地址{ProducterStationAddress},起始地址{ProducterStartAddress},长度{ProducterLength}";
            }
        }

        public string ShowConsumerValue
        {
            get
            {
                return $"站地址{ConsumerStationAddress},起始地址{ConsumerStartAddress},长度{ConsumerLength}";
            }
        }
    }
}