using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Pojo;
using Pkn_HostSystem.Models.Windows;
using System.Collections.ObjectModel;


namespace Pkn_HostSystem.Models.Page;

public partial class HomePageModel : ObservableObject
{

    #region 通讯设置页
    /// <summary>
    /// 网络连接对象列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<NetworkDetailed> setConnectDg;
    /// <summary>
    /// 变量,当前选择的名字
    /// </summary>
    [ObservableProperty] private string currentSetName;

    #endregion

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

    /// <summary>
    /// 连接数据库
    /// </summary>
    [ObservableProperty] private JdbcUrl jdbcUrl = new();

    /// <summary>
    /// 连接数据URL地址
    /// </summary>
    [ObservableProperty] private string realJdbcUrl = "Server=服务器名;DataBase=数据名;Uid=sa;Pwd=密码;TrustServerCertificate=True;";
    [ObservableProperty] private string showJdbcUrl = "Server=服务器名;DataBase=数据名;Uid=sa;Pwd=密码;TrustServerCertificate=True;";

}