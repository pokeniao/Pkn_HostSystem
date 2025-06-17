using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using log4net;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Page;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.Base.Log;

public class LogBase<T>
{
    private ISnackbarService snackbarService;
    private readonly ILog log;
    private ObservableCollection<string> list;



    public LogBase()
    {
        log = LogManager.GetLogger(typeof(T));
        GlobalMannager.GlobalDictionary.TryGetValue("LogListBox", out var obj);
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

    public virtual void Info(string message)
    {
        log.Info(message);
        LogListAdd(message);
    }

    public virtual void Error(string message)
    {
        log.Error(message);
        LogListAdd(message);
    }

    #region 显示并且记录

    public void SuccessAndShow(string message)
    {
        log.Info(message);

        //判断一下有没有设置SnackbarPresenter , 没有不需要页面显示
        if (snackbarService.GetSnackbarPresenter() != null)
        {
            snackbarService.Show("提示", message, ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark16),
                TimeSpan.FromSeconds(1));
        }
        LogListAdd(message);
    }

    public void WarningAndShow(string message)
    {
        log.Info(message);
        //判断一下有没有设置SnackbarPresenter , 没有不需要页面显示
        if (snackbarService.GetSnackbarPresenter() != null)
        {
            snackbarService.Show("提示", message, ControlAppearance.Caution, new SymbolIcon(SymbolRegular.Alert24),
                TimeSpan.FromSeconds(1));
        }
        LogListAdd(message);
    }

    public void ErrorAndShow(string message)
    {
        log.Error(message);
        //判断一下有没有设置SnackbarPresenter , 没有不需要页面显示
        if (snackbarService.GetSnackbarPresenter() != null)
        {
            snackbarService.Show("提示", message, ControlAppearance.Danger,
                new SymbolIcon(SymbolRegular.Alert24), TimeSpan.FromSeconds(10));
        }
        LogListAdd(message);
    }

    public void SuccessAndShow(string message, string logMessage)
    {
        log.Info(logMessage);
        if (snackbarService.GetSnackbarPresenter() != null)
        {
            snackbarService.Show("提示", message, ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark16),
                TimeSpan.FromSeconds(1));
        }
        LogListAdd(logMessage);
    }

    public void WarningAndShow(string message, string logMessage)
    {
        log.Info(logMessage);
        if (snackbarService.GetSnackbarPresenter() != null)
        {
            snackbarService.Show("提示", message, ControlAppearance.Caution, new SymbolIcon(SymbolRegular.Alert24),
                TimeSpan.FromSeconds(1));
        }
        LogListAdd(logMessage);
    }

    public void ErrorAndShow(string message, string logMessage)
    {
        log.Error(logMessage);
        if (snackbarService.GetSnackbarPresenter() != null)
        {
            snackbarService.Show("提示", message, ControlAppearance.Danger,
                new SymbolIcon(SymbolRegular.Alert24), TimeSpan.FromSeconds(5));
        }
        LogListAdd(logMessage);
    }

    #endregion


    #region 显示并且记录,异步线程

    public void SuccessAndShowTask(string message)
    {
        log.Info(message);
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (snackbarService.GetSnackbarPresenter() != null)
            {
                snackbarService.Show("提示", message, ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark16),
                    TimeSpan.FromSeconds(1));
            }
        });
        LogListAdd(message);
    }

    public void WarningAndShowTask(string message)
    {
        log.Info(message);
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (snackbarService.GetSnackbarPresenter() != null)
            {
                snackbarService.Show("提示", message, ControlAppearance.Caution, new SymbolIcon(SymbolRegular.Alert24),
                    TimeSpan.FromSeconds(1));
            }
        });
        LogListAdd(message);
    }

    public void ErrorAndShowTask(string message)
    {
        log.Error(message);
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (snackbarService.GetSnackbarPresenter() != null)
            {
                snackbarService.Show("提示", message, ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Alert24), TimeSpan.FromSeconds(10));
            }
        });
        LogListAdd(message);
    }

    public void SuccessAndShowTask(string message, string logMessage)
    {
        log.Info(logMessage);
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (snackbarService.GetSnackbarPresenter() != null)
            {
                snackbarService.Show("提示", message, ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark16),
                    TimeSpan.FromSeconds(1));
            }
        });
        LogListAdd(logMessage);
    }

    public void WarningAndShowTask(string message, string logMessage)
    {
        log.Info(logMessage);
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (snackbarService.GetSnackbarPresenter() != null)
            {
                snackbarService.Show("提示", message, ControlAppearance.Caution, new SymbolIcon(SymbolRegular.Alert24),
                    TimeSpan.FromSeconds(1));
            }
        });
        LogListAdd(logMessage);
    }

    public void ErrorAndShowTask(string message, string logMessage)
    {
        log.Error(logMessage);
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (snackbarService.GetSnackbarPresenter() != null)
            {
                snackbarService.Show("提示", message, ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Alert24), TimeSpan.FromSeconds(5));
            }
        });
        LogListAdd(logMessage);
    }

    #endregion

    public void LogListAdd(string message)
    {
        Application.Current.Dispatcher.InvokeAsync(() =>
        {
                if (list.Count == 300) list.Clear();
                list.Add($"{DateTime.Now.ToString("MM-dd HH:mm:ss.ffff")}:  {message}");
        });
    }
}



