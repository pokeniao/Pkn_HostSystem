using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Pojo.Page.HomePage;

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
    /// 变量,当前选择的名字
    /// </summary>
    [ObservableProperty] private string currentSetName;

}