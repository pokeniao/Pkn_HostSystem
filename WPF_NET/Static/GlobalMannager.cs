using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Reflection;
using WPF_NET.Pojo;

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
    public static ConcurrentDictionary<string, NetWorkPoJo> NetWorkDictionary = new ConcurrentDictionary<string, NetWorkPoJo>();

    static GlobalMannager()
    {
        GlobalDictionary.TryAdd("LogListBox",new ObservableCollection<string>()); 
    }
}
