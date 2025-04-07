using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using DynamicData.Binding;
using WPF_NET.Pojo;
using WPF_NET.Pojo.Page.MESTcp;

namespace WPF_NET.Models;

public partial class MesTcpModel:ObservableObject
{
    [ObservableProperty] private ObservableCollectionExtended<NetWorkPoJo> netWorkList;
    [ObservableProperty] private ObservableCollectionExtended<MesTcpPojo> dynNetList;
    [ObservableProperty] private ObservableCollection<DynConditionItem> dynConditionItemList;

    public List<string> MethodName { get; set; } = ["读线圈","通讯名","请求方式","参数"];

}