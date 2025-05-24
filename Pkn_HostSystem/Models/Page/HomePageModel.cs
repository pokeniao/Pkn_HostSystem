using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Pojo;
using Pkn_HostSystem.Models.Windows;
using System.Collections.ObjectModel;


namespace Pkn_HostSystem.Models.Page;

public partial class HomePageModel : ObservableObject
{
    /// <summary>
    /// Log日志列表,用于显示日志
    /// </summary>
    private ObservableCollection<string> logListBox =  new ();
    [JsonIgnore]
    public ObservableCollection<string> LogListBox
    {
        get => logListBox;
        set
        {
            SetProperty(ref logListBox, value);
        }

    }


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

    /// <summary>
    /// 相机的连接集合
    /// </summary>
    [ObservableProperty] private ObservableCollection<CameraDetailed> cameraList;

}