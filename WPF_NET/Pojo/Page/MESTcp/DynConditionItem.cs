using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WPF_NET.Pojo.Page.MESTcp;

public class DynConditionItem : ObservableObject
{
    public string Name { get; set; }

    public string ConnectName { get; set; }

    private string methodName;
    //站地址
    private int stationAddress =1;

    public int StationAddress
    {
        get => stationAddress;
        set
        {
            SetProperty(ref stationAddress, value);
            OnPropertyChanged(nameof(ShowValue));
        }

    }

    //起始地址
    private int startAddress;

    public int StartAddress
    {
        get => startAddress;
        set
        {
            SetProperty(ref startAddress, value);
            OnPropertyChanged(nameof(ShowValue));
        }
    }
    //数量
    private int endAddress =1;


    public int EndAddress
    {
        get => endAddress;
        set
        {
            SetProperty(ref endAddress, value);
            OnPropertyChanged(nameof(ShowValue));
        }
    }

    //模式选择
    private string bitNet = "单寄存器";

    public string BitNet
    {
        get => bitNet;
        set
        {
            SetProperty(ref bitNet, value);
            OnPropertyChanged(nameof(ShowValue));
        }
    }

    public string ShowValue
    {
        get
        {
            string value = null;
            switch (MethodName)
            {
                case "读寄存器":
                    value = $"站地址:{StationAddress} 起始地址:{StartAddress} 读取数量{EndAddress} {bitNet}";
                    break;
                case "读线圈":
                    value = $"站地址:{StationAddress} 起始地址:{StartAddress} 读取数量{EndAddress}";
                    break;
                case "Socket返回":
                    value = "暂无";
                    break;
            }

            return value;
        }
    }

    public string MethodName
    {
        get => methodName;
        set
        {
            SetProperty(ref methodName, value);
            OnPropertyChanged(nameof(showReadReg));
            OnPropertyChanged(nameof(showReadCoil));
            OnPropertyChanged(nameof(showSocket));
            OnPropertyChanged(nameof(ShowValue));
        }
    }


    public bool showReadReg => MethodName == "读寄存器";

    public bool showReadCoil => MethodName == "读线圈";

    public bool showSocket => MethodName == "Socket返回";
}