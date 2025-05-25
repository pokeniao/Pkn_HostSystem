using Pkn_HostSystem.Static;
using System.Collections.ObjectModel;
using System.Windows;
using Wpf.Ui;

namespace Pkn_HostSystem.Base.Log;

public class MesLogBase<T> : LogBase<T>
{
    private ObservableCollection<string>? list;

    public MesLogBase()
    {
        GlobalMannager.GlobalDictionary.TryGetValue("MesLogListBox", out object value);
        list = (ObservableCollection<string>)value;
    }
    public MesLogBase(SnackbarService snackbarService) : base(snackbarService)
    {
    }

    public override void Info(string message)
    {
        base.Info(message);
    }

    public override void Error(string message)
    {
        base.Error(message);
    }

    public void InfoOverride(string message)
    {
        LogListAdd(message);
        base.Info(message);
    }


    public void ErrorOverride(string message)
    {
        LogListAdd(message);
        base.Error(message);
    }
    public void LogListAdd(string message)
    {
        Application.Current.Dispatcher.InvokeAsync(() =>
        {
            if (list.Count == 500) list.Clear();

            list.Add($"{DateTime.Now.ToString("MM-dd HH:mm:ss.ffff")}:  {message}");
        });
    }
}