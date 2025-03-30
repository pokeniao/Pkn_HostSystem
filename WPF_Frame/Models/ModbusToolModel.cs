using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace WPF_Frame.Models
{
    public partial class ModbusToolModel : ObservableObject
    {
        [ObservableProperty]
        private List<string> modbusTcp_Ip;
        [ObservableProperty]
        private string modbusTcp_Port;
        [ObservableProperty]
        private List<string> modbusRtu_COM;
        [ObservableProperty]
        private List<string> modbusRtu_baudRate;
        [ObservableProperty]
        private List<string> modbusRtu_dataBits;
        [ObservableProperty]
        private List<string> modbusRtu_stopBits;
        [ObservableProperty]
        private List<string> modbusRtu_parity;

        [ObservableProperty] private List<string> funtionCode;
        [ObservableProperty] private List<string> slaveAddress;
        [ObservableProperty] private List<string> startAddress;
        [ObservableProperty] private string readCount;

    }
}
