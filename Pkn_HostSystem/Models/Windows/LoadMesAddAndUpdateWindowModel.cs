using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using Newtonsoft.Json;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Service.LoadMes.Interface;

namespace Pkn_HostSystem.Models.Windows;

public partial class LoadMesAddAndUpdateWindowModel : ObservableObject
{
    /// <summary>
    /// 名称
    /// </summary>
    [ObservableProperty] private string name;

    /// <summary>
    /// 请求方式
    /// </summary>
    [ObservableProperty] private string ajax;

    /// <summary>
    /// http路径
    /// </summary>
    [ObservableProperty] private string httpPath;

    /// <summary>
    /// 接口名称
    /// </summary>
    [ObservableProperty] private string api;

    /// <summary>
    /// 循环时间
    /// </summary>
    [ObservableProperty] private int cycTime;

    /// <summary>
    /// 请求体方式:JSON,XML等
    /// </summary>
    [ObservableProperty] private string requestMethod = "JSON";

    /// <summary>
    /// 请求内容
    /// </summary>
    [ObservableProperty] private string request;

    /// <summary>
    /// 嵌入条件集合
    /// </summary>
    [ObservableProperty] private ObservableCollection<LoadMesCondition> condition;

    /// <summary>
    /// 返回消息
    /// </summary>
    [ObservableProperty] private string response;

    /// <summary>
    /// 是否执行循环
    /// </summary>
    [ObservableProperty] private bool runCyc;

    /// <summary>
    /// 触发类型:循环,触发
    /// </summary>
    [ObservableProperty] private string triggerType = "循环触发";

    /// <summary>
    /// 站地址
    /// </summary>
    [ObservableProperty] private string stationAddress = "1";

    /// <summary>
    /// 起始地址
    /// </summary>
    [ObservableProperty] private string startAddress = "0";

    /// <summary>
    /// 触发发送Http的消息内容
    /// </summary>
    [ObservableProperty] private string triggerMessage = "1";

    /// <summary>
    /// 触发后:成功返回消息
    /// </summary>
    [ObservableProperty] private string successResponseMessage = "2";

    /// <summary>
    /// 触发后:失败返回消息
    /// </summary>
    [ObservableProperty] private string failResponseMessage = "3";

    /// <summary>
    /// 是否需要本地保存
    /// </summary>
    [ObservableProperty] private bool localSave;

    /// <summary>
    /// 是否需要发送HTTP请求
    /// </summary>
    [ObservableProperty] private bool httpNeed;

    /// <summary>
    /// 是否需要进行转发
    /// </summary>
    [ObservableProperty] private bool transpondNeed;

    /// <summary>
    /// 令牌 循环进程任务
    /// </summary>
    [JsonIgnore]
    public CancellationTokenSource cts { get; set; }

    /// <summary>
    /// 当前Http进程任务
    /// </summary>
    [JsonIgnore]
    public Lazy<Task> Task { get; set; }

    /// <summary>
    /// 用于页面显示什么循环的形式
    /// </summary>
    [JsonIgnore] [ObservableProperty] private string cycText = "循环时间(s)";

    /// <summary>
    /// 用于绑定显示,已启动的通讯
    /// </summary>
    [JsonIgnore] [ObservableProperty] private ObservableCollectionExtended<NetWork> netWorkList;

    /// <summary>
    /// 当前绑定的触发形的通讯名称
    /// </summary>
    [ObservableProperty] private string triggerConnectName;

    /// <summary>
    /// 创建Http请求头
    /// </summary>
    [ObservableProperty] private ObservableCollection<HttpHeader> httpHeaders = new ObservableCollection<HttpHeader>();


    /// <summary>
    /// 转发ModbusTcp站地址
    /// </summary>
    [ObservableProperty] private string forWardingStationAddress = "1";

    /// <summary>
    /// 转发ModbusTcp起始地址
    /// </summary>
    [ObservableProperty] private string forWardingStartAddress = "0";

    /// <summary>
    /// 转发ModbusTcp长度
    /// </summary>
    [ObservableProperty] private string forWardingLen = "1";

    /// <summary>
    /// 转发对象名
    /// </summary>
    [ObservableProperty] private string forwardingName;

    /// <summary>
    /// 工位名称
    /// </summary>
    [ObservableProperty] private string station;

    /// <summary>
    /// 需要工位日志记录
    /// </summary>
    [ObservableProperty] private bool needStationLog;
    //维护一个集合,用于判断动态嵌入HTTP请求不会循环嵌套;
    [JsonIgnore] public List<string> UseHttpList { get; set; }


    [JsonIgnore] public ILoadMesService LoadMesService { get; set; }

    /// <summary>
    /// Http后需要内部触发调用超时
    /// </summary>
    [ObservableProperty] private int needInteriorTriggerTimeOut = 30;
    /// <summary>
    /// Http后需要内部触发调用
    /// </summary>
    [ObservableProperty] private bool needInteriorTrigger = false;
    /// <summary>
    /// Http后需要内部触发调用地址
    /// </summary>
    [ObservableProperty] private int needInteriorTriggerIndex = 0;
    /// <summary>
    /// 内部触发数组索引地址
    /// </summary>
    [ObservableProperty] private int interiorArrayIndex =0;
    /// <summary>
    /// 显示Http设置页面
    /// </summary>
    [ObservableProperty] private bool showHttpSet;
    /// <summary>
    /// 显示触发设置页面
    /// </summary>
    [ObservableProperty] private bool showTriggerSet;

    /// <summary>
    /// 显示内部触发设置页面
    /// </summary>
    [ObservableProperty] private bool showInteriorTriggerSet;
    /// <summary>
    /// 显示Modbus触发页面
    /// </summary>
    [ObservableProperty] private bool showModbusTriggerParam;

    /// <summary>
    /// 显示Tcp触发页面参数
    /// </summary>
    [ObservableProperty] private bool showTcpTriggerParam;
    /// <summary>
    /// 显示基恩士上链路通讯参数
    /// </summary>
    [ObservableProperty] private bool showKeyenceHostLinkParam;
    /// <summary>
    /// 显示串口通讯参数
    /// </summary>
    [ObservableProperty] private bool showSerialParam;

    public override string ToString()
    {
        return string.Join(',',
            Enumerable.Select<LoadMesCondition, string>(Condition, c => $"Key={c.Key} Value ={c.Value}"));
    }
}