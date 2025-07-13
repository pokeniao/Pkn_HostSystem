using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Models.Core;
using System.Collections.ObjectModel;
using System.IO.Ports;

namespace Pkn_HostSystem.Models.Page
{
    public partial class HomeSetConnectModel : ObservableObject
    {
        /// <summary>
        /// Ip显示集合
        /// </summary>
        [ObservableProperty] private List<string> ips =ModbusBase.GetIpAddress().ToList();
        /// <summary>
        /// 串口显示集合
        /// </summary>
        [ObservableProperty] private List<string> coms = ScpiSerialTool.GetCOM();
        /// <summary>
        /// 波特率显示集合
        /// </summary>
        public List<string> BaudRates { get; set; }= ScpiSerialTool.BaudRates;
        /// <summary>
        /// 数据位显示集合
        /// </summary>
        public List<string> DataBits { get; set; }= ScpiSerialTool.DataBits;
        /// <summary>
        /// 停止位显示集合
        /// </summary>
        public List<StopBits> StopBitsList { get; set; } = ScpiSerialTool.StopBitsList;
        /// <summary>
        /// 校验位显示集合
        /// </summary>
        public List<Parity> Parities { get; set; }= ScpiSerialTool.Parities;


        public Dictionary<string, string> NewLines { get; set; } = ScpiSerialTool.NewLines;
        /// <summary>
        /// TCP服务器是否需要监听
        /// </summary>
        [ObservableProperty] private bool tcpServerNeedListen;
        /// <summary>
        /// 通讯方式
        /// </summary>
        [ObservableProperty] private string netMethod;
        /// <summary>
        /// Ip
        /// </summary>
        [ObservableProperty] private string ip;
        /// <summary>
        /// 端口号
        /// </summary>
        [ObservableProperty] private int port = int.Parse("502");
        /// <summary>
        /// Com
        /// </summary>
        [ObservableProperty]
        private string com = ModbusBase.GetCOM().Length > 0 ? ModbusBase.GetCOM()[0] : null;
        /// <summary>
        /// 波特率
        /// </summary>
        [ObservableProperty] private string baudRate = "9600";
        /// <summary>
        /// 数据位
        /// </summary>
        [ObservableProperty] private string dataBit = "8";
        /// <summary>
        /// 停止位
        /// </summary>
        [ObservableProperty] private StopBits stopBits = StopBits.One;
        /// <summary>
        /// 校验位
        /// </summary>
        [ObservableProperty] private Parity parity;
        /// <summary>
        /// 超时时间
        /// </summary>
        [ObservableProperty] private int timeOut = 1000;
        /// <summary>
        /// 结束符
        /// </summary>
        [ObservableProperty] private string newLine = "\n";

    }
}