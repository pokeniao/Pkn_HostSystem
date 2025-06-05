using System.Collections.ObjectModel;
using System.IO.Ports;
using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Models.Core;

namespace Pkn_HostSystem.Models.Page
{
    public partial class ModbusToolModel : ObservableObject
    {


        public static ModbusBase modbusBase = new ModbusBase();
        /// <summary>
        /// modbusTcp和ModbusRtu需要显示的参数
        /// </summary>
        [ObservableProperty]
        private List<string> modbusTcp_Ip;
        [ObservableProperty]
        private int modbusTcp_Port = int.Parse("502");
        [ObservableProperty]
        private List<string> modbusRtu_COM  = modbusBase.getCOM().ToList();

        [ObservableProperty] private List<string> modbusRtu_baudRate = new() { "9600", "14400", "19200" };
        [ObservableProperty]
        private List<string> modbusRtu_dataBits = new() { "8", "7" };
        [ObservableProperty]
        private List<StopBits> modbusRtu_stopBits = Enum.GetValues(typeof(StopBits)).Cast<StopBits>().ToList();
        [ObservableProperty]
        private List<Parity> modbusRtu_parity = Enum.GetValues(typeof(Parity)).Cast<Parity>().ToList();

        #region 暂时先放在这里

        [ObservableProperty] private bool tcpServerNeedListen;

        #endregion

        /// <summary>
        /// 读写Modbus的功能码,站地址,起始地址等
        /// </summary>
        [ObservableProperty] private List<string> funtionCode = new()
        {
            "01读线圈",
            "02读输入状态",
            "03读保持寄存器",
            "04读输入寄存器",
            "05写单线圈",
            "06写单寄存器",
            "0F写多线圈",
            "10写多寄存器"
        };
        [ObservableProperty] private int slaveAddress =1;
        [ObservableProperty] private int startAddress =0;
        [ObservableProperty] private int readCount =1;

        /// <summary>
        /// 选中的数据
        /// </summary>
        [ObservableProperty] private string netMethod_select;
        [ObservableProperty] private string modbusTcp_Ip_select;
        [ObservableProperty] private string funtionCode_select;
        [ObservableProperty] private string modbusRtu_COM_select = modbusBase.getCOM().Length > 0 ? modbusBase.getCOM()[0] : null;
        [ObservableProperty] private string modbusRtu_baudRate_select = "9600";
        [ObservableProperty] private string modbusRtu_dataBits_select = "8";
        [ObservableProperty] private StopBits modbusRtu_stopBits_select = StopBits.One;
        [ObservableProperty] private Parity modbusRtu_parity_select;
       
        /// <summary>
        /// 读写的列表,用于显示
        /// </summary>
        [ObservableProperty] private List<ModbusToolPojo<object>> readDvgList;
        [ObservableProperty] private ObservableCollection<ModbusToolPojo<object>> writeDvgList;
    }
}
