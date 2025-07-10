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



        [ObservableProperty] private string com;

        [ObservableProperty] private string baudRate;

        [ObservableProperty] private string dataBits;
        

        [ObservableProperty] private StopBits stopBits;

        [ObservableProperty] private Parity paritie;

        [ObservableProperty] private string newLine;

    }
}