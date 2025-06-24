using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Win32;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Service.UserDefined;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.Views.Pages;
using SkiaSharp;
using System.Diagnostics;
using System.Reflection;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;

namespace Pkn_HostSystem.ViewModels.Page;

public partial class SettingsPageViewModel : ObservableRecipient
{
    public SnackbarService SnackbarService { get; set; }
    public LogBase<SettingsPageViewModel> Log;
    public SettingsPageModel SettingsPageModel { get; set; }

    public SettingsPageViewModel()
    {

        SettingsPageModel = AppJsonTool<SettingsPageModel>.Load();
        if (SettingsPageModel == null)
        {
            //Model初始化
            SettingsPageModel = new SettingsPageModel();
        }

      
        SnackbarService = new SnackbarService();
        Log = new LogBase<SettingsPageViewModel>(SnackbarService);
        Init();

    }
    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        TraceContext.Name = "系统初始化";
        switch (SettingsPageModel.CurrentTheme)
        {
            case "亮主题":
                LightThemeRadioButtonCheckedCommand.Execute(null);
                break;
            case "暗主题":
                DarkThemeRadioButtonCheckedCommand.Execute(null);
                break;
            case "系统主题":
                AutoThemeRadioButtonCheckedCommand.Execute(null);
                break;
        }
        SelfStartCommand.Execute(null);
    }
    /// <summary>
    /// 亮主题
    /// </summary>
    [RelayCommand]
    public void LightThemeRadioButtonChecked(SettingsPage page)
    {
        ApplicationThemeManager.Apply(ApplicationTheme.Light);
        //设置LiveCharts报表图片主题
        LiveCharts.Configure(config =>
        {
            config.AddLightTheme();
            GlobalMannager.ThemeSkColor = new SKColor(0, 0, 0);
            Ioc.Default.GetRequiredService<LiveChartsTestViewModel>().RefreshCommand.Execute(null);
        });
        if (TraceContext.Name!= "系统初始化")
        {
            Log.SuccessAndShow("切换主题成功,LiveCharts渲染需要重启后生效");
        }
    }

    /// <summary>
    /// 暗主题
    /// </summary>
    [RelayCommand]
    public void DarkThemeRadioButtonChecked(SettingsPage page)
    {
        ApplicationThemeManager.Apply(ApplicationTheme.Dark);
        //设置LiveCharts报表图片主题
        LiveCharts.Configure(config =>
        {
            config.AddDarkTheme();
            GlobalMannager.ThemeSkColor = new SKColor(255, 255, 255);
            Ioc.Default.GetRequiredService<LiveChartsTestViewModel>().RefreshCommand.Execute(null);
        });
        if (TraceContext.Name != "系统初始化")
        {
            Log.SuccessAndShow("切换主题成功,LiveCharts渲染需要重启后生效");
        }
    }


    /// <summary>
    /// 自适应主题
    /// </summary>
    [RelayCommand]
    public void AutoThemeRadioButtonChecked(SettingsPage page)
    {
        ApplicationThemeManager.ApplySystemTheme();
        var systemTheme = ApplicationThemeManager.GetSystemTheme();

        if (systemTheme is SystemTheme.Light)
        {
            //设置LiveCharts报表图片主题
            LiveCharts.Configure(config =>
            {
                config.AddLightTheme();
                GlobalMannager.ThemeSkColor = new SKColor(0, 0, 0);
                Ioc.Default.GetRequiredService<LiveChartsTestViewModel>().RefreshCommand.Execute(null);
            });
        } else if (systemTheme is SystemTheme.Dark)
        {
            //设置LiveCharts报表图片主题
            LiveCharts.Configure(config =>
            {
                config.AddDarkTheme();
                GlobalMannager.ThemeSkColor = new SKColor(255, 255, 255);
                Ioc.Default.GetRequiredService<LiveChartsTestViewModel>().RefreshCommand.Execute(null);
            });
        }
        if (TraceContext.Name != "系统初始化")
        {
            Log.SuccessAndShow("切换主题成功,LiveCharts渲染需要重启后生效");
        }
    }

    #region 开机自启动

    //注册列表路径
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName = "Pkn_HostSystem"; // 可自定义名称
    [RelayCommand]
    public void SelfStart()
    {
        // Assembly.GetExecutingAssembly().Location; 是获取到编译后的程序集 .dll文件路径
        // string exePath = Assembly.GetExecutingAssembly().Location;

        //Process.GetCurrentProcess().MainModule.FileName 是获取到编译后的执行主入口 .exe文件路径
        string exePath = Process.GetCurrentProcess().MainModule.FileName;

        RegistryKey runKey = Registry.CurrentUser.OpenSubKey(RunKeyPath, true);

        if (SettingsPageModel.IsSelfStart)
        {
            if (!IsAutoStartEnabled())
            {
                //写入注册表
                runKey.SetValue(AppName, $"\"{exePath}\"");
            }

            if (IsAutoStartEnabled())
            {
                Log.SuccessAndShow("开机自启动设置完毕");
            }
            else
            {
                Log.WarningAndShow("开机自启动设置未成功,注册表写入失败");
            }
        }
        else
        {
            //从注册表中获取
            if (runKey.GetValue(AppName) != null)
            {
                //删除
                runKey.DeleteValue(AppName);
            }

            if (!IsAutoStartEnabled())
            {
         
                Log.SuccessAndShow("开机自启动关闭");
            }
            else
            {
                Log.WarningAndShow("开机自启动关闭设置未成功,注册表删除失败");
            }
        }
    }
    /// <summary>
    /// 判断注册表是否存在,开机自启动
    /// </summary>
    /// <returns></returns>
    public bool IsAutoStartEnabled()
    {
        RegistryKey runKey = Registry.CurrentUser.OpenSubKey(RunKeyPath, false);
        string value = runKey?.GetValue(AppName)?.ToString();
        string exePath = $"\"{Process.GetCurrentProcess().MainModule.FileName}\"";
        return value == exePath;
    }
    #endregion




    #region 保存存档

    [RelayCommand]
    public void SaveProgram()
    {
        if (SaveAll())
        {
            Log.SuccessAndShow("存档成功");
        }
        else
        {
            Log.ErrorAndShow("存档失败");
        }
    }

    [RelayCommand]
    public async Task ReSetProgram()
    {
        var uiMessageBox = new Wpf.Ui.Controls.MessageBox { Title = "重置", Content = "是否确定重置,重置后配置会清空", };
        uiMessageBox.PrimaryButtonText = "确认重置";
        uiMessageBox.IsPrimaryButtonEnabled = true;
        MessageBoxResult messageBoxResult = await uiMessageBox.ShowDialogAsync();

        if (messageBoxResult == MessageBoxResult.Primary)
        {
            Reset();
            Log.SuccessAndShow("重置成功");
        }
    }

    public void Reset()
    {
        AppJsonTool<HomePageModel>.Reset();
        AppJsonTool<LoadMesPageModel>.Reset();
        AppJsonTool<MesTcpModel>.Reset();
        AppJsonTool<ProductiveModel>.Reset();
        AppJsonTool<SettingsPageModel>.Reset();
        AppJsonTool<VisionPageModel>.Reset();
    }

    #region 保存程序

    [RelayCommand]
    public void Save()
    {
        AppJsonTool<SettingsPageModel>.Save(SettingsPageModel);
      
    }
    #endregion


    public bool SaveAll()
    {
        HomePageViewModel homePageViewModel = Ioc.Default.GetRequiredService<HomePageViewModel>();
        homePageViewModel.SaveCommand.Execute(null);

        LoadMesPageViewModel loadMesPageViewModel = Ioc.Default.GetRequiredService<LoadMesPageViewModel>();
        loadMesPageViewModel.SaveCommand.Execute(null);

        MesTcpViewModel mesTcpViewModel = Ioc.Default.GetRequiredService<MesTcpViewModel>();
        mesTcpViewModel.SaveCommand.Execute(null);

        ProductiveViewModel productiveViewModel = Ioc.Default.GetRequiredService<ProductiveViewModel>();
        productiveViewModel.SaveCommand.Execute(null);

        SettingsPageViewModel settingsPageViewModel = Ioc.Default.GetRequiredService<SettingsPageViewModel>();
        settingsPageViewModel.SaveCommand.Execute(null);


        VisionPageViewModel visionPageViewModel = Ioc.Default.GetRequiredService<VisionPageViewModel>();
        visionPageViewModel.SaveCommand.Execute(null);

        return true;
    }

    #endregion

    #region 弹窗SnackbarService
    public void setSnackbarPresenter(SnackbarPresenter snackbarPresenter)
    {
        SnackbarService.SetSnackbarPresenter(snackbarPresenter);
    }
    #endregion
}