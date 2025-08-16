using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using DynamicData;
using DynamicData.Binding;
using Newtonsoft.Json;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.ViewModels.Page;
using System.Collections.ObjectModel;
using System.Windows;


namespace Pkn_HostSystem.Models.Page;

public partial class HomePageModel : ObservableObject
{
    #region 需要保存的

    /// <summary>
    /// 网络连接对象列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<NetworkDetailed> setConnectDg;

    /// <summary>
    /// 已连接的对象
    /// </summary>
    [ObservableProperty] private ObservableCollectionExtended<NetWork> netWorkList;

    /// <summary>
    /// 采集方式
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PictureFilePathShowVisibility))]
    [NotifyPropertyChangedFor(nameof(ShowCameraVisibility))]
    private CameraInterfaceEnum collectMethod;

    /// <summary>
    /// 相机的连接集合
    /// </summary>
    [ObservableProperty] private ObservableCollection<CameraDetailed> cameraList;

    /// <summary>
    /// 当前相机是否实时显示
    /// </summary>
    [ObservableProperty] private string realTimeName = "实时";

    /// <summary>
    /// 图片的地址
    /// </summary>
    [ObservableProperty] private string picturePath;


    [ObservableProperty] private string selectCamera;


    /// <summary>
    /// 连接数据库
    /// </summary>
    [ObservableProperty] private JdbcUrl jdbcUrl = new();

    /// <summary>
    /// 连接数据URL地址
    /// </summary>
    [ObservableProperty]
    private string realJdbcUrl = "Server=服务器名;DataBase=数据名;Uid=sa;Pwd=密码;TrustServerCertificate=True;";

    [ObservableProperty]
    private string showJdbcUrl = "Server=服务器名;DataBase=数据名;Uid=sa;Pwd=密码;TrustServerCertificate=True;";

    #region 自定义页

    // /// <summary>
    // /// 工单集合
    // /// </summary>
    // [ObservableProperty] private ObservableCollection<PppOrderList> pppOrderLists;
    // /// <summary>
    // /// 当前选中的工单
    // /// </summary>
    // [ObservableProperty] private PppOrderList currentSelectPppOrder;

    //低电阻
    [ObservableProperty] private string rLow;

    //高电阻
    [ObservableProperty] private string rHight;

    //低电压
    [ObservableProperty] private string vLow;

    //高电压
    [ObservableProperty] private string vHight;

    #endregion


    // [ObservableProperty]
    // private ObservableCollection<RegisterItem> triggerRegisterItems = new ObservableCollection<RegisterItem>(Enumerable.Range(0, 100).Select(s => new RegisterItem()
    // {
    //     Index = s.ToString()
    // }));

    private ObservableCollection<RegisterItem> registerItems;
    
    public ObservableCollection<RegisterItem> RegisterItems
    {
        get => registerItems;
        set
        {
            SetProperty(ref registerItems, value);
        }

    }
    // [ObservableProperty] private ObservableCollection<RegisterItem> queueItems = new ObservableCollection<RegisterItem>(Enumerable.Range(0, 100).Select(s => new RegisterItem()
    // {
    //     Index = s.ToString()
    // }));

    #endregion


    #region 不需要保存的变量

    /// <summary>
    /// 网络连接对象列表,当前选择的名字
    /// </summary>
    private string currentSetName;

    [JsonIgnore]
    public string CurrentSetName
    {
        get => currentSetName;
        set
        {
            SetProperty(ref currentSetName, value);
        }
    }

    [JsonIgnore]
    public Visibility PictureFilePathShowVisibility =>
        CollectMethod == CameraInterfaceEnum.图片 ? Visibility.Visible : Visibility.Collapsed;
    [JsonIgnore]
    public Visibility ShowCameraVisibility =>
        CollectMethod == CameraInterfaceEnum.GenICamTL ? Visibility.Visible : Visibility.Collapsed;


    /// <summary>
    /// Http请求的列表
    /// </summary>
    private ObservableCollection<LoadMesAddAndUpdateWindowModel> httpLists;

    [JsonIgnore]
    public ObservableCollection<LoadMesAddAndUpdateWindowModel> HttpLists
    {
        get => httpLists;
        set
        {
            SetProperty(ref httpLists, value);
        }
    }

    /// <summary>
    /// 当前选中的Http名
    /// </summary>
    private string httpName;

    [JsonIgnore]
    public string HttpName
    {
        get => httpName;
        set
        {
            SetProperty(ref httpName, value);
        }
    }

    #endregion
}