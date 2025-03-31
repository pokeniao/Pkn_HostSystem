using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Reflection;

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


    static GlobalMannager()
    {
        GlobalDictionary.TryAdd("LogListBox",new ObservableCollection<string>()); 
    }
}