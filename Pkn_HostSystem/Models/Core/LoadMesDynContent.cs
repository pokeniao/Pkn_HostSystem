using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Models.Core;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.Pojo.Page.MESTcp;

public class LoadMesDynContent
{
    /// <summary>
    /// 当前名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 当前动态连接的内容,详细行数据
    /// </summary>
    public ObservableCollection<DynCondition> DynCondition { get; set; }

    /// <summary>
    /// 当前动态内容消息体,需要嵌入内容的消息
    /// </summary>
    public string Message { get; set; }
}

public class DynCondition : ObservableObject
{
    /// <summary>
    /// 当前动态名
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 当前连接的名称
    /// </summary>
    public string ConnectName { get; set; }

    /// <summary>
    /// 当前连接Http的名称
    /// </summary>
    public string HttpName { get; set; }

    /// <summary>
    /// 当前方法的名称(请求方式) :读线圈,读寄存器
    /// </summary>
    private string methodName;
    public string MethodName
    {
        get => methodName;
        set
        {
            //当选择方法发生更改,更新属性
            SetProperty(ref methodName, value);
            OnPropertyChanged(nameof(showReadReg));
            OnPropertyChanged(nameof(showReadCoil));
            OnPropertyChanged(nameof(showSocket));
            OnPropertyChanged(nameof(ShowValue));
            OnPropertyChanged(nameof(showHostLinkReadReg));
            OnPropertyChanged(nameof(showHostLinkReadCoid));
            OnPropertyChanged(nameof(showConnectName));
            OnPropertyChanged(nameof(showHttpName));
            OnPropertyChanged(nameof(showHttp));
        }
    }
    /// <summary>
    /// 站地址
    /// </summary>
    private int stationAddress = 1;
    public int StationAddress
    {
        get => stationAddress;
        set
        {
            SetProperty(ref stationAddress, value);
            OnPropertyChanged(nameof(ShowValue));
        }

    }
    /// <summary>
    /// 存储当前连接服务器的端口号
    /// </summary>
    public int SelectPost { get; set; }
    /// <summary>
    /// 起始地址
    /// </summary>
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
    /// <summary>
    /// 结束地址
    /// </summary>
    private int endAddress = 1;

    public int EndAddress
    {
        get => endAddress;
        set
        {
            SetProperty(ref endAddress, value);
            OnPropertyChanged(nameof(ShowValue));
        }
    }

    /// <summary>
    /// 模式选择
    /// </summary>
    private string bitNet = "单寄存器(无符号)";

    public string BitNet
    {
        get => bitNet;
        set
        {
            SetProperty(ref bitNet, value);
            OnPropertyChanged(nameof(ShowValue));
        }
    }

    /// <summary>
    /// 用于显示当前参数的值
    /// </summary>
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
                case "读DM寄存器":
                    value = $"起始地址:{StartAddress} {bitNet}";
                    break;
                case "读R线圈状态":
                    value = $"起始地址:{StartAddress}";
                    break;
                case "Http方式":
                    value = "双击设置解析的Json";
                    break;

            }

            return value;
        }

    }


    public string SocketSendMessage { get; set; }



    private string getMessageType;

    public string GetMessageType
    {
        get => getMessageType;
        set
        {
            SetProperty(ref getMessageType, value);
            OnPropertyChanged(nameof(showReadReg));
            OnPropertyChanged(nameof(showReadCoil));
            OnPropertyChanged(nameof(showSocket));
            OnPropertyChanged(nameof(ShowValue));
            OnPropertyChanged(nameof(showHostLinkReadReg));
            OnPropertyChanged(nameof(showHostLinkReadCoid));
            OnPropertyChanged(nameof(showConnectName));
            OnPropertyChanged(nameof(showHttpName));
            OnPropertyChanged(nameof(showHttp));
        }

    }


    /// <summary>
    /// 打开Switch映射
    /// </summary>
    public bool OpenSwitch { get; set; }

    /// <summary>
    /// 打开校验
    /// </summary>
    public bool OpenVerify { get; set; }

    /// <summary>
    /// 转发的站地址
    /// </summary>
    public string TranspondStationAddress { get; set; }

    /// <summary>
    /// 转发的起始地址
    /// </summary>
    public string TranspondStartAddress { get; set; }
    /// <summary>
    /// 转发的对象
    /// </summary>
    public string TranspondObject { get; set; }
    /// <summary>
    /// 转发的长度
    /// </summary>
    public string TranspondLen { get; set; }

    public bool ResultTranspond { get; set; }


    public ObservableCollection<DynSwitch> SwitchList { get; set; } = new ObservableCollection<DynSwitch>();
    /// <summary>
    /// 当动态获取失败直接返回失败
    /// </summary>
    public bool DynFailReturnFail { get; set; }

    /// <summary>
    /// 用于显示连接
    /// </summary>
    public ObservableCollection<DynVerify> VerifyList { get; set; } = new ObservableCollection<DynVerify>();


    public ObservableCollection<GetHttpObject> HttpObjects { get; set; } = new ObservableCollection<GetHttpObject>();

    /// <summary>
    /// 用于控制参数在编辑时候的显示
    /// </summary>
    public bool showReadReg => MethodName == "读寄存器";

    public bool showReadCoil => MethodName == "读线圈";

    public bool showSocket => MethodName == "Socket返回";

    public bool showHostLinkReadReg => MethodName == "读DM寄存器";
    public bool showHostLinkReadCoid => MethodName == "读R线圈状态";

    public bool showHttp => MethodName == "Http方式";
    /// <summary>
    /// 用于控制连接名显示
    /// </summary>
    public bool showConnectName => GetMessageType == "通讯";

    public bool showHttpName => GetMessageType == "HTTP";

}
public class DynSwitch
{

    public string Case { get; set; }
    public string Value { get; set; }

}