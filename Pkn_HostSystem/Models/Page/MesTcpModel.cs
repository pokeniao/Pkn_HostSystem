using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using Pkn_HostSystem.Pojo.Page.HomePage;
using Pkn_HostSystem.Pojo.Page.MESTcp;

namespace Pkn_HostSystem.Models.Page;

public partial class MesTcpModel:ObservableObject
{
    [ObservableProperty] private ObservableCollectionExtended<NetWorkPoJo> netWorkList;
    [ObservableProperty] private ObservableCollectionExtended<MesTcpPojo> dynNetList;
    [ObservableProperty] private ObservableCollection<DynConditionItem> dynConditionItemList;

    [ObservableProperty] private string message;

    public List<string> MethodName { get; set; } = ["读线圈","读寄存器","Socket返回"];

    public List<string> BitNet { get; set; } = ["单寄存器", "双寄存器","ASCII字符串"];

}