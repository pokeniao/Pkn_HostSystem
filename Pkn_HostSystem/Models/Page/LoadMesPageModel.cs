using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Models.Windows;

namespace Pkn_HostSystem.Models.Page;

public partial class LoadMesPageModel : ObservableObject
{
    /// <summary>
    /// HTTP发送的列表
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<LoadMesAddAndUpdateWindowModel> mesPojoList;
    /// <summary>
    /// 返回消息数据,显示列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<string> returnMessageList;
}