using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Pkn_HostSystem.Models.Core;

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
    /// 内部连接的名称(通讯方式 集合or队列)
    /// </summary>
    public string InteriorName { get; set; }
    /// <summary>
    /// 内部通讯数组地址
    /// </summary>
    public int InteriorArrayIndex { get; set; }
    /// <summary>
    /// 内部通讯队列地址
    /// </summary>
    public int InteriorQueueIndex { get; set; }


    public NetWork NetWork { get; set; }
    /// <summary>
    /// 当前连接Http的名称
    /// </summary>
    public string HttpName { get; set; }
    /// <summary>
    /// 当前选中的用户自定义选项
    /// </summary>
    public Type UserDefined { get; set; }

    /// <summary>
    /// 打开用户自定义逻辑的JSON映射
    /// </summary>

    public bool OpenGetPropertyValue { get; set; }

    
    /// <summary>
    /// 发送消息的模式
    /// </summary>
    private string sendMessageMethod = "常量";

    public string SendMessageMethod
    {
        get => sendMessageMethod;
        set
        {
            SetProperty(ref sendMessageMethod, value);
            OnPropertyChanged(nameof(showStaticMessage));
            OnPropertyChanged(nameof(showRegisterMessage));
        }
    }
    public bool showStaticMessage => SendMessageMethod == "常量";
    public bool showRegisterMessage => SendMessageMethod != "常量";

    /// <summary>
    /// 内部地址/队列 数组地址
    /// </summary>
    public string InteriorGetRegisterMessageIndex { get; set; } = "0";


    public Dictionary<string, string> MethodNameMemory = new Dictionary<string, string>();
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
            OnPropertyChanged(nameof(showUserDefined));
            OnPropertyChanged(nameof(showSerial));
            OnPropertyChanged(nameof(showInterior));
        }
    }


    /// <summary>
    /// 内部的触发寄存器地址
    /// </summary>
    public int InteriorTrigger { get; set; } = 0;
    /// <summary>
    /// 串口发送超时
    /// </summary>
    private int sendTimeOut =1000;

    public int SendTimeOut
    {
        get => sendTimeOut;
        set
        {
            SetProperty(ref sendTimeOut, value);
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
                    value = $"发送内容: {SocketSendMessage}";
                    break;
                case "读DM寄存器":
                    value = $"起始地址:{StartAddress} {bitNet}";
                    break;
                case "读R线圈状态":
                    value = $"起始地址:{StartAddress}";
                    break;
                case "Http":
                    value = "双击设置参数";
                    break;
                case "自定义(无法填写)":
                    value = $"{UserDefined?.Name}";
                    break;
                case "串口通讯":
                    value = $"发送内容: {SerialSendMessage}";
                    break;
                case "集合":
                    value = $"读取地址:{InteriorArrayIndex}";
                    break;
                case "队列":
                    value = $"读取地址:{InteriorQueueIndex}";
                    break;
            }
            return value;
        }
    }
    /// <summary>
    /// TCP套接字发送内容
    /// </summary>
    public string SocketSendMessage { get; set; }
    /// <summary>
    /// 串口发送内容
    /// </summary>
    public string SerialSendMessage { get; set; }
    /// <summary>
    /// 用于显示[请求类型]
    /// </summary>
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
            OnPropertyChanged(nameof(showUserDefined));
            OnPropertyChanged(nameof(showSerial));
            OnPropertyChanged(nameof(showInterior));
        }
    }

    /// <summary>
    /// 触发返回的消息需要 重新定义
    /// </summary>
    public bool NeedInteriorTriggerUserSetReturn { get; set; }

    /// <summary>
    /// 触发是否返回自定义消息
    /// </summary>
    public STRING InteriorTriggerReturnMessage { get; set; } = new STRING();
    /// <summary>
    /// 触发是否返回
    /// </summary>
    public bool InteriorTriggerReturn { get; set; } = false;

    /// <summary>
    /// 打开Switch映射
    /// </summary>
    public bool OpenSwitch { get; set; }

    /// <summary>
    /// 打开校验
    /// </summary>
    public bool OpenVerify { get; set; }


    /// <summary>
    /// 转发Modbus细节
    /// </summary>
    public TranspondModbusDetailed TranspondModbusDetailed { get; set; } = new();


    public bool ResultTranspond { get; set; }


    public ObservableCollection<DynSwitch> SwitchList { get; set; } = new ObservableCollection<DynSwitch>();


    /// <summary>
    /// 用于显示连接
    /// </summary>
    public ObservableCollection<DynVerify> VerifyList { get; set; } = new ObservableCollection<DynVerify>();


    public ObservableCollection<GetHttpObject> HttpObjects { get; set; } = new ObservableCollection<GetHttpObject>();

    /// <summary>
    /// 请求方式 :进行控制显示
    /// </summary>
    public bool showReadReg => MethodName == "读寄存器";

    public bool showReadCoil => MethodName == "读线圈";

    public bool showSocket => MethodName == "Socket返回";

    public bool showHostLinkReadReg => MethodName == "读DM寄存器";
    public bool showHostLinkReadCoid => MethodName == "读R线圈状态";

    public bool showHttp => MethodName == "Http";

    public bool showSerial => MethodName == "串口通讯";

    public bool showInteriorArrarySet => MethodName == "集合";

    public bool showInteriorQueueSet => MethodName == "队列";


    /// <summary>
    /// 通讯名: 用于控制连接名显示
    /// </summary>
    public bool showConnectName => GetMessageType == "通讯";

    public bool showHttpName => GetMessageType == "HTTP";

    public bool showUserDefined => GetMessageType == "自定义";

    public bool showInterior => GetMessageType == "内部";
}

public class DynSwitch
{
    public string Case { get; set; }
    public string Value { get; set; }
}