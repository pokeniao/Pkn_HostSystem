namespace Pkn_HostSystem.Models.Core
{
    public class HttpHeader
    {
        public string Key { get; set; }
        public string Value { get; set; }
        /// <summary>
        /// 编制为新行
        /// </summary>
        public bool NewCol { get; set; }
    }
}