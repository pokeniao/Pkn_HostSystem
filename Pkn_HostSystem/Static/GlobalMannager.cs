using DynamicData;
using Newtonsoft.Json;
using Pkn_HostSystem.Pojo.Page.HomePage;
using Pkn_HostSystem.Pojo.Page.MESTcp;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

namespace Pkn_HostSystem.Static;

/// <summary>
/// 旧版本写法 , 之后不用静态 ,改用 Ioc控制
/// </summary>
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

    /// <summary>
    /// 管理 连接的线程池
    /// </summary>
    public static SourceCache<NetWorkPoJo, string> NetWorkDictionary;

    /// <summary>
    /// 动态连接的字典
    /// </summary>
    public static SourceCache<MesTcpPojo, string> DynDictionary;

    static GlobalMannager()
    {
        GlobalDictionary = new ConcurrentDictionary<string, object>();
        GlobalDictionary.TryAdd("LogListBox", new ObservableCollection<string>());
        GlobalDictionary.TryAdd("MesLogListBox", new ObservableCollection<string>());


        NetWorkDictionary =
            new SourceCache<NetWorkPoJo, string>(n => n.NetWorkId);


        DynDictionary = new SourceCache<MesTcpPojo, string>(n => n.Name);
    }
}