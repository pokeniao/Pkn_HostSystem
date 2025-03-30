using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace WPF_NET.Static;

public static class GlobalMannager
{
    public static ConcurrentDictionary<string ,object> GlobalDictionary = new ConcurrentDictionary<string ,object>();

     static GlobalMannager()
    {
        GlobalDictionary.TryAdd("LogListBox",new ObservableCollection<string>()); 
    }
}