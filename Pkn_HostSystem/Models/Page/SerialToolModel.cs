using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Base;
using System.IO.Ports;

namespace Pkn_HostSystem.Models.Page
{
    public partial class SerialToolModel :ObservableObject
    {
        public List<string> ComList { get; set; } = ScpiSerialTool.GetCOM();

        public List<string> BaudRates { get; set; } = ScpiSerialTool.BaudRates;

        public List<string> DataBitss { get; set; } = ScpiSerialTool.DataBits;

        public List<StopBits> StopBitsList { get; set; } = ScpiSerialTool.StopBitsList;

        public List<Parity> Parities { get; set; } = ScpiSerialTool.Parities;

        public Dictionary<string,string> NewLines { get; set; } = ScpiSerialTool.NewLines;

        [ObservableProperty] private string sendMessageText;

        [ObservableProperty] private string acceptMessageText;

        [ObservableProperty] private string connectButton = "连接";

        [ObservableProperty] private string whileReadButton = "循环读取";

        [ObservableProperty] private string com;

        [ObservableProperty] private string baudRate = "9600";

        [ObservableProperty] private string dataBits = "8";
        

        [ObservableProperty] private StopBits stopBits =StopBits.One;

        [ObservableProperty] private Parity paritie = Parity.None;

        [ObservableProperty] private int timeOut = 1000;

        [ObservableProperty] private string newLine = "\n";

        [ObservableProperty] private int writeTimeOut = 1000;

    }
}