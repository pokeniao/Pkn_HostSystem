using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Reflection;
using DynamicData;
using WPF_NET.Pojo;
using WPF_NET.Pojo.Page.MESTcp;

namespace WPF_NET.Static;

public static class GlobalMannager
{
    /// <summary>
    /// 当前全局字典
    /// </summary>
    public static ConcurrentDictionary<string ,object> GlobalDictionary = new ConcurrentDictionary<string ,object>();

    /// <summary>
    ///  当前软件版本
    /// </summary>
    public static string AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;

    /// <summary>
    /// 管理 连接的线程池
    /// </summary>
    public static SourceCache<NetWorkPoJo,string > NetWorkDictionary = new SourceCache<NetWorkPoJo, string>(n=>n.NetWorkId);

    /// <summary>
    /// 动态连接的字典
    /// </summary>
    public static SourceCache<MesTcpPojo, string> DynDictionary = new SourceCache<MesTcpPojo, string>(n => n.Name);

    static GlobalMannager()
    {
        GlobalDictionary.TryAdd("LogListBox",new ObservableCollection<string>());
        GlobalDictionary.TryAdd("MesLogListBox", new ObservableCollection<string>());
    }
}
