using System.Collections.ObjectModel;
using System.IO.Ports;
using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Pojo.Page.ModbusTool;

namespace Pkn_HostSystem.Models.Page
{
    public partial class ModbusToolModel : ObservableObject
    {
        [ObservableProperty]
        private List<string> modbusTcp_Ip;
        [ObservableProperty]
        private int modbusTcp_Port;
        [ObservableProperty]
        private List<string> modbusRtu_COM;
        [ObservableProperty]
        private List<string> modbusRtu_baudRate;
        [ObservableProperty]
        private List<string> modbusRtu_dataBits;
        [ObservableProperty]
        private List<StopBits> modbusRtu_stopBits;
        [ObservableProperty]
        private List<Parity> modbusRtu_parity;

        [ObservableProperty] private List<string> funtionCode;
        [ObservableProperty] private int slaveAddress;
        [ObservableProperty] private int startAddress;
        [ObservableProperty] private int readCount;

        [ObservableProperty] private string netMethod_select;
        [ObservableProperty] private string modbusTcp_Ip_select;
        [ObservableProperty] private string funtionCode_select;
        [ObservableProperty] private string modbusRtu_COM_select;
        [ObservableProperty] private string modbusRtu_baudRate_select;
        [ObservableProperty] private string modbusRtu_dataBits_select;
        [ObservableProperty] private StopBits modbusRtu_stopBits_select = StopBits.One;
        [ObservableProperty] private Parity modbusRtu_parity_select;
       
        [ObservableProperty] private List<ModbusPojo<object>> readDvgList;
        [ObservableProperty] private ObservableCollection<ModbusPojo<object>> writeDvgList;
    }
}
