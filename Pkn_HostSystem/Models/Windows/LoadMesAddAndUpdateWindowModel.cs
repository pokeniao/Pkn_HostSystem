using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using Newtonsoft.Json;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Pojo.Page.HomePage;
using Pkn_HostSystem.Pojo.Windows.LoadMesAddAndUpdateWindow;

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
    [ObservableProperty] private string requestMethod;
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
    [ObservableProperty] private string triggerType;
    /// <summary>
    /// 站地址
    /// </summary>
    [ObservableProperty] private string stationAddress="1";
    /// <summary>
    /// 起始地址
    /// </summary>
    [ObservableProperty] private string startAddress="0";

    /// <summary>
    /// 触发发送Http的消息内容
    /// </summary>
    [ObservableProperty] private string triggerMessage="1";

    /// <summary>
    /// 触发后:成功返回消息
    /// </summary>
    [ObservableProperty] private string successResponseMessage="2";

    /// <summary>
    /// 触发后:失败返回消息
    /// </summary>
    [ObservableProperty] private string failResponseMessage="3";

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
    [JsonIgnore] public CancellationTokenSource cts { get; set; }

    /// <summary>
    /// 当前Http进程任务
    /// </summary>
    [JsonIgnore] public Lazy<Task> Task { get; set; }
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

    public override string ToString()
    {
        return string.Join(',',
            Enumerable.Select<LoadMesCondition, string>(Condition, c => $"Key={c.Key} Value ={c.Value}"));
    }
}