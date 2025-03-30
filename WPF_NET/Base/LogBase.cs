using log4net;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;
using WPF_NET.Static;

namespace WPF_NET.Base;

public class LogBase<T>
{
    private ISnackbarService snackbarService;
    private readonly ILog log;
    private ObservableCollection<string> list;

    public LogBase()
    {
        this.log = LogManager.GetLogger(typeof(T));
        GlobalMannager.GlobalDictionary.TryGetValue("LogListBox", out object obj);
        list = (ObservableCollection<string>)obj;
    }

    public LogBase(ISnackbarService snackbarService) : this()
    {
        this.snackbarService = snackbarService;
    }

    public LogBase(ISnackbarService snackbarService, ILog log) : this(snackbarService)
    {
        this.log = log;
    }

    public void Info(string message)
    {
        log.Info(message);
        LogListAdd(message);
    }

    public void Error(string message)
    {
        log.Error(message);
        LogListAdd(message);
    }

    public void SuccessAndShow(string message)
    {
        log.Info(message);
        snackbarService.Show("提示", message, ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark16),
            TimeSpan.FromSeconds(1));
        LogListAdd(message);
    }

    public void WarningAndShow(string message)
    {
        log.Info(message);
        snackbarService.Show("提示", message, ControlAppearance.Caution, new SymbolIcon(SymbolRegular.Alert24),
            TimeSpan.FromSeconds(1));
        LogListAdd(message);
    }

    public void ErrorAndShow(string message)
    {
        log.Error(message);
        snackbarService.Show("提示", message, ControlAppearance.Danger,
            new SymbolIcon(SymbolRegular.Alert24), TimeSpan.FromSeconds(10));
        LogListAdd(message);
    }

    public void SuccessAndShow(string message, string logMessage)
    {
        log.Info(logMessage);
        snackbarService.Show("提示", message, ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark16),
            TimeSpan.FromSeconds(1));
        LogListAdd(logMessage);
    }

    public void WarningAndShow(string message, string logMessage)
    {
        log.Info(logMessage);
        snackbarService.Show("提示", message, ControlAppearance.Caution, new SymbolIcon(SymbolRegular.Alert24),
            TimeSpan.FromSeconds(1));
        LogListAdd(logMessage);
    }

    public void ErrorAndShow(string message, string logMessage)
    {
        log.Error(logMessage);
        snackbarService.Show("提示", message, ControlAppearance.Danger,
            new SymbolIcon(SymbolRegular.Alert24), TimeSpan.FromSeconds(5));
        LogListAdd(logMessage);
    }

    public void LogListAdd(string message)
    {
        if (list.Count == 500)
        {
            list.Clear();
        }
        list.Add($"{DateTime.UtcNow.ToString("MM-dd HH:mm:ss.ffff")}:  {message}");
    }
}