using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
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
    /// 触发的通讯模式:ModbusTcp,ModbusRtu,Socket等
    /// </summary>
    [ObservableProperty] private string netTrigger;

    /// <summary>
    /// 触发的通讯模式: Modbus需要什么方法 读线圈?读寄存器?
    /// </summary>
    [ObservableProperty] private string modbusMethod;
    /// <summary>
    /// 站地址
    /// </summary>
    [ObservableProperty] private string stationAddress;
    /// <summary>
    /// 起始地址
    /// </summary>
    [ObservableProperty] private string startAddress;
    /// <summary>
    /// 结束地址
    /// </summary>
    [ObservableProperty] private string endAddress;

    /// <summary>
    /// 触发发送Http的消息内容
    /// </summary>
    [ObservableProperty] private string triggerMessage;

    /// <summary>
    /// 触发后:成功返回消息
    /// </summary>
    [ObservableProperty] private string successResponseMessage;

    /// <summary>
    /// 触发后:失败返回消息
    /// </summary>
    [ObservableProperty] private string failResponseMessage;

    /// <summary>
    /// 是否需要本地保存
    /// </summary>
    [ObservableProperty] private bool localSave;

    /// <summary>
    /// 令牌 循环进程任务
    /// </summary>
    [JsonIgnore] public CancellationTokenSource cts { get; set; }

    /// <summary>
    /// 当前Http进程任务
    /// </summary>
    [JsonIgnore] public Lazy<Task> Task { get; set; }


    public override string ToString()
    {
        return string.Join(',',
            Enumerable.Select<LoadMesCondition, string>(Condition, c => $"Key={c.Key} Value ={c.Value}"));
    }
}