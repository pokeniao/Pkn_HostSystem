using CommunityToolkit.Mvvm.ComponentModel;

namespace Pkn_HostSystem.Models.Pojo
{
    public partial class ElectricityTest : ObservableObject
    {
        /// <summary>
        /// 电阻上限
        /// </summary>
        [ObservableProperty] private double resistanceUpLimit;


        [ObservableProperty] private double setResistanceUpLimit;

        /// <summary>
        /// 电阻下限
        /// </summary>
        [ObservableProperty] private double resistanceLowLimit;

        [ObservableProperty] private double setResistanceLowLimit;

        /// <summary>
        /// 电压上限
        /// </summary>
        [ObservableProperty] private double voltageUpLimit;


        [ObservableProperty] private double setVoltageUpLimit;
        /// <summary>
        /// 电压下限
        /// </summary>
        [ObservableProperty] private double voltageLowLimit;

        [ObservableProperty] private double setVoltageLowLimit;

        /// <summary>
        /// 当前条码
        /// </summary>
        [ObservableProperty] private string curSN;

        /// <summary>
        /// 岗位编码
        /// </summary>

        [ObservableProperty] private string machineId;
        [ObservableProperty] private string setMachineId;

        /// <summary>
        /// 工序
        /// </summary>
        [ObservableProperty] private string groupCode;
        [ObservableProperty] private string setGroupCode;


        /// <summary>
        ///电压标准
        /// </summary>
        [ObservableProperty] private string voltageStandard ="";

        [ObservableProperty] private string setVoltageStandard = "";

        [ObservableProperty] private string resistanceStandard ="";

        [ObservableProperty] private string setResistanceStandard ="";


        /// <summary>
        /// HTTP路径
        /// </summary>
        [ObservableProperty] private string httpPath = "http://10.30.98.94";
        [ObservableProperty] private string setHttpPath = "";
        /// <summary>
        /// Api路径
        /// </summary>
        [ObservableProperty] private string apiPath = "/mes/service/SendDeviceInfo?WSDL";
        [ObservableProperty] private string setApiPath = "";


    }
}