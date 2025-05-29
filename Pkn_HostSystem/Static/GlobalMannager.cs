using DynamicData;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Stations;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Pkn_HostSystem.Static;

public static class GlobalMannager
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
         
    static GlobalMannager()
    {
        GlobalDictionary = new ConcurrentDictionary<string, object>();
        GlobalDictionary.TryAdd("LogListBox", new ObservableCollection<string>());
        GlobalDictionary.TryAdd("MesLogListBox", new ObservableCollection<string>());
        NetWorkDictionary =
            new SourceCache<NetWork, string>(n => n.NetWorkId);
        DynDictionary = new SourceCache<LoadMesDynContent, string>(n => n.Name);
        StationDictionary = new SourceCache<IEachStation, string>(n => n.Header);
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
        return  types.ToList();
    }
}