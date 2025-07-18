using DynamicData;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Core.Interface;
using Pkn_HostSystem.Models.Page;
using SkiaSharp;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Pkn_HostSystem.Static;

public static class GlobalManager
{
    /// <summary>
    /// 当前全局字典
    /// </summary>
    public static ConcurrentDictionary<string, object> GlobalDictionary;

    /// <summary>
    ///  当前软件版本
    /// </summary>
    public static string AssemblyVersion =
        Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;

    public static string? AssemblyName = Assembly.GetEntryAssembly()?.GetName().Name;

    public static UserLoginEnum CurLoginState { get; set; } 

    /// <summary>
    /// 获得当前项目集
    /// </summary>
    public static Assembly Asssembly => Assembly.GetExecutingAssembly();

    /// <summary>
    /// %AppData%路径
    /// </summary>
    public static readonly string AppFolder =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            AssemblyName // 文件夹名
        );

    /// <summary>
    /// 管理 连接的线程池
    /// </summary>
    public static SourceCache<NetWork, string> NetWorkDictionary;

    /// <summary>
    /// 动态连接的字典
    /// </summary>
    public static SourceCache<LoadMesDynContent, string> DynDictionary;

    /// <summary>
    /// 工站字典
    /// </summary>
    public static SourceCache<IEachStation, string> StationDictionary;

    /// <summary>
    /// 静态主题颜色
    /// </summary>
    public static SKColor ThemeSkColor;

    /// <summary>
    /// 数据库
    /// </summary>
    public static string jdbcPath;
    /// <summary>
    /// 内部触发寄存器
    /// </summary>
    public static int[] Register = new int[100];

    /// <summary>
    /// 内部寄存器
    /// </summary>
    public static object[] ArrayRegister = new object[100];
    /// <summary>
    /// 内部队列 ,队列初始化器
    /// </summary>
    public static List<ConcurrentQueue<object>> QueueRegister = Enumerable
        .Range(0, 100)
        .Select(_ => new ConcurrentQueue<object>())
        .ToList();


    static GlobalManager()
    {
        //登入状态
        CurLoginState = UserLoginEnum.NoLogged;
        //向静态字典添加东西
        GlobalDictionary = new ConcurrentDictionary<string, object>();
        GlobalDictionary.TryAdd("LogListBox", new ObservableCollection<string>());
        GlobalDictionary.TryAdd("MesLogListBox", new ObservableCollection<string>());
        //初始化网路连接字典
        NetWorkDictionary =
            new SourceCache<NetWork, string>(n => n.NetWorkId);
        //初始化话动态字典
        DynDictionary = new SourceCache<LoadMesDynContent, string>(n => n.Name);
        //初始化工位字典
        StationDictionary = new SourceCache<IEachStation, string>(n => n.Header);
        //数据库JDBC路径赋值
        jdbcPath = JsonTool<HomePageModel>.Load()?.RealJdbcUrl;
        //初始化工位信息
        StationManager.InitStation();
    }

    public static NetWork GetNetWork(string ConnectName)
    {
        var netWorkPoJoes = GlobalManager.NetWorkDictionary.Items.ToList();
        foreach (var netWorkPoJo in netWorkPoJoes)
        {
            if (netWorkPoJo.NetworkDetailed.Name == ConnectName)
            {
                return netWorkPoJo;
            }
        }

        return null;

    }


    /// <summary>
    /// 获得用户定义类的 所有Types
    /// </summary>
    /// <returns></returns>
    public static List<Type> GetUserDefinedTypes()
    {
        string namespaceName = "Pkn_HostSystem.Service.UserDefined";
        //返回当前项目集下筛选的内容 ,这里筛选的是CLass ,更具命名空间
        IEnumerable<Type> types = Asssembly
            .GetTypes()
            .Where(x =>
                    x.IsClass &&
                    x.Namespace != null &&
                    x.Namespace!.StartsWith(namespaceName, StringComparison.InvariantCultureIgnoreCase) &&
                    !x.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false) // 排除编译器生成的类型
            );
        return types.ToList();
    }


}