using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using Pkn_HostSystem.Pojo.Page.HomePage;
using Pkn_HostSystem.Pojo.Page.MESTcp;
using System.Collections.ObjectModel;
using Newtonsoft.Json;


namespace Pkn_HostSystem.Models.Page;

public partial class MesTcpModel : ObservableObject
{
    //用于操作当前连接
    [ObservableProperty] private ObservableCollectionExtended<NetWorkPoJo> netWorkList;

    //动态内容数据
    [ObservableProperty] private ObservableCollectionExtended<MesTcpPojo> dynNetList;

    //绑定到页面显示当前行 动态内容数据
    [ObservableProperty] private ObservableCollection<DynConditionItem> dynConditionItemList;

    [ObservableProperty] private string message;

    [JsonIgnore] [ObservableProperty] private ObservableCollection<string> methodName;
    [JsonIgnore] public List<string> BitNet { get; set; } = ["单寄存器", "双寄存器", "ASCII字符串"];

    [ObservableProperty] private ObservableCollection<string> tcpServerConnectionClint;

}