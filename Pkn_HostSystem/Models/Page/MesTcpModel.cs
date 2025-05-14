using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using Pkn_HostSystem.Pojo.Page.HomePage;
using Pkn_HostSystem.Pojo.Page.MESTcp;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Windows;


namespace Pkn_HostSystem.Models.Page;

public partial class MesTcpModel : ObservableObject
{
    /// <summary>
    /// 用于操作当前连接
    /// </summary>
    [ObservableProperty] private ObservableCollectionExtended<NetWork> netWorkList;
    /// <summary>
    /// 用于操作Http连接
    /// </summary>
    [ObservableProperty] private ObservableCollection<LoadMesAddAndUpdateWindowModel> httpList;
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
    /// 当前在设置的名称
    /// </summary>
    [ObservableProperty] private string setSwitchSetName;
    /// <summary>
    /// 用于显示当前选中的Switch属性
    /// </summary>
    [ObservableProperty] private ObservableCollection<DynSwitch> switchList;
    /// <summary>
    /// 用于显示设置的页面
    /// </summary>
    [ObservableProperty] private bool veritySet;

 

    /// <summary>
    /// 当前在设置的名称
    /// </summary>
    [ObservableProperty] private string setVeritySetName;
    /// <summary>
    /// 用于显示校验的配置列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<DynVerify> verifyList;
    /// <summary>
    /// 用于显示设置的页面
    /// </summary>
    [ObservableProperty] private bool httpSet;

    /// <summary>
    /// 用于显示当前转发的页面名字
    /// </summary>
    [ObservableProperty] private string transpondSetName;

    /// <summary>
    /// 用于显示当前转发的连接名
    /// </summary>
    [ObservableProperty] private TranspondModbusDetailed transpondModbusDetailed;


    /// <summary>
    /// 用于显示当前转发的页面
    /// </summary>
    [ObservableProperty] private bool transpondSet;



    /// <summary>
    /// 用于显示Http映射的值的配置
    /// </summary>
    [ObservableProperty] private ObservableCollection<GetHttpObject> httpObjects;
    /// <summary>
    /// 当前在设置的名称
    /// </summary>
    [ObservableProperty] private string setHttpObjectName;
    /// <summary>
    /// 用于设置UniformGrid的行列
    /// </summary>
    [ObservableProperty] private int setRows =1;
    [ObservableProperty] private int setColumns =1;
}