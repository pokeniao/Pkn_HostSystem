using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Models.Pojo;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.Pojo.Page.HomePage;
using Pkn_HostSystem.ViewModels.Page;
using System.Runtime.CompilerServices;

namespace Pkn_HostSystem.Models.Page;

public partial class HomePageModel : ObservableObject
{
    /// <summary>
    /// Log日志列表,用于显示日志
    /// </summary>
    [ObservableProperty] private ObservableCollection<string> logListBox;
    /// <summary>
    /// 网络连接对象列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<NetworkDetailed> setConnectDg;
    /// <summary>
    /// 工单集合
    /// </summary>
    [ObservableProperty] private ObservableCollection<BydOrderList> bydOrderLists;
    /// <summary>
    /// 变量,当前选择的名字
    /// </summary>
    [ObservableProperty] private string currentSetName;
    /// <summary>
    /// 当前选中的工单
    /// </summary>
    [ObservableProperty] private BydOrderList currentSelectBydOrder;

    /// <summary>
    /// Http请求的列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<LoadMesAddAndUpdateWindowModel> httpLists;

    /// <summary>
    /// 当前选中的Http名
    /// </summary>
    [ObservableProperty] private string httpName;

}