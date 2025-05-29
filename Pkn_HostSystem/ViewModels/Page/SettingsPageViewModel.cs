using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Views.Pages;
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
    }
    /// <summary>
    /// 亮主题
    /// </summary>
    [RelayCommand]
    public void LightThemeRadioButtonChecked(SettingsPage page)
    {
        ApplicationThemeManager.Apply(ApplicationTheme.Light);
        // page.LightThemeRadioButton.IsChecked = true;
    }

    /// <summary>
    /// 暗主题
    /// </summary>
    [RelayCommand]
    public void DarkThemeRadioButtonChecked(SettingsPage page)
    {
        ApplicationThemeManager.Apply(ApplicationTheme.Dark);
        // page.DarkThemeRadioButton.IsChecked =true;
    }


    /// <summary>
    /// 自适应主题
    /// </summary>
    [RelayCommand]
    public void AutoThemeRadioButtonChecked(SettingsPage page)
    {
        ApplicationThemeManager.ApplySystemTheme();
        // page.AutoThemeRadioButton.IsChecked = true;
    }

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