namespace Pkn_HostSystem.Models.Core
{
    public class CameraDetailed
    {
        public string CameraName { get; set; }

        public string GenICamTL { get; set; }



        /// <summary>
        /// 判断是否是新行
        /// </summary>
        public bool IsNewLine { get; set; }
    }
}