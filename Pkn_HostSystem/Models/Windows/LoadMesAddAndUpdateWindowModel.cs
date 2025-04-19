using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Pkn_HostSystem.Pojo.Windows.LoadMesAddAndUpdateWindow;

namespace Pkn_HostSystem.Models.Windows;

public partial class LoadMesAddAndUpdateWindowModel : ObservableObject
{
    //动态绑定
    [ObservableProperty] private string name;
    [ObservableProperty] private string ajax;
    [ObservableProperty] private string httpPath;
    [ObservableProperty] private string api;
    [ObservableProperty] private int cycTime;

    [ObservableProperty] private string requestMethod;
    [ObservableProperty] private string request;
    [ObservableProperty] private ObservableCollection<ConditionItem> condition;
    [ObservableProperty] private string response;
    [ObservableProperty] private bool runCyc;


    //触发类型
    [ObservableProperty] private string triggerType;

    //触发通讯模式
    [ObservableProperty] private string netTrigger;

    //Modbus通讯内容
    [ObservableProperty] private string modbusMethod;
    [ObservableProperty] private string stationAddress;
    [ObservableProperty] private string startAddress;
    [ObservableProperty] private string endAddress;

    //触发消息
    [ObservableProperty] private string triggerMessage;

    //成功返回消息
    [ObservableProperty] private string successResponseMessage;

    //失败返回消息
    [ObservableProperty] private string failResponseMessage;

    //是否需要本地保存
    [ObservableProperty] private bool localSave;

    //令牌 循环进程任务
    [JsonIgnore] public CancellationTokenSource cts { get; set; }

    //循环进程任务
    [JsonIgnore] public Lazy<Task> Task { get; set; }


    public override string ToString()
    {
        return string.Join(',',
            Enumerable.Select<ConditionItem, string>(Condition, c => $"Key={c.Key} Value ={c.Value}"));
    }
}