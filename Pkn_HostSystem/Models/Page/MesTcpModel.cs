using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using Pkn_HostSystem.Pojo.Page.HomePage;
using Pkn_HostSystem.Pojo.Page.MESTcp;
using System.Collections.ObjectModel;
using Newtonsoft.Json;


namespace Pkn_HostSystem.Models.Page;

public partial class MesTcpModel : ObservableObject
{
    /// <summary>
    /// 用于操作当前连接
    /// </summary>
    [ObservableProperty] private ObservableCollectionExtended<NetWork> netWorkList;
    /// <summary>
    /// 动态嵌入内容数据,列表
    /// </summary>
    [ObservableProperty] private ObservableCollectionExtended<LoadMesDynContent> dynNetList;
    /// <summary>
    /// 选中一行的动态嵌入内容数据
    /// </summary>
    [ObservableProperty] private ObservableCollection<DynCondition> dynConditionItemList;

    [ObservableProperty] private string message;
    /// <summary>
    /// 读取消息的方式,用于动态显示,如读写线圈等
    /// </summary>
    [JsonIgnore] [ObservableProperty] private ObservableCollection<string> methodName;
    /// <summary>
    /// 用于选择Modbus是什么读取方式
    /// </summary>
    [JsonIgnore] public List<string> BitNet { get; set; } 
    /// <summary>
    /// Tcp服务器连接客户端的名称,用于显示已连接客户端
    /// </summary>
    [ObservableProperty] private ObservableCollection<string> tcpServerConnectionClint;
    /// <summary>
    /// 用于显示当前设置的页面
    /// </summary>
    [ObservableProperty] private bool showSwitchSet;
    /// <summary>
    /// 用于显示当前选中的Switch属性
    /// </summary>
    [ObservableProperty] private ObservableCollection<DynSwitch> switchList;

}