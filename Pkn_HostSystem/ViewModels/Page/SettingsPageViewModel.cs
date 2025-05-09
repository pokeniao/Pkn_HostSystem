﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Page;

public partial class SettingsPageViewModel : ObservableRecipient
{
    public SnackbarService SnackbarService { get; set; }
    public LogBase<SettingsPageViewModel> log;
    public SettingsPageModel SettingsPageModel { get; set; }

    public SettingsPageViewModel()
    {
        SnackbarService = new SnackbarService();
        log = new LogBase<SettingsPageViewModel>(SnackbarService);
        //Model初始化
        SettingsPageModel = new SettingsPageModel();
    }

    #region 保存存档

    [RelayCommand]
    public void SaveProgram()
    {
        if (Save())
        {
            log.Info("存档成功");
        }
        else
        {
            log.Error("存档失败");

        }
    }
    [RelayCommand]
    public void ReSetProgram()
    {
        AppJsonTool<HomePageModel>.Reset();
        AppJsonTool<LoadMesPageModel>.Reset();
        AppJsonTool<MesTcpModel>.Reset();
        AppJsonTool<ProductiveModel>.Reset();
        log.Info("重置成功");
    }

    public bool Save()
    {
        HomePageViewModel homePageViewModel = Ioc.Default.GetRequiredService<HomePageViewModel>();
        homePageViewModel.SaveCommand.Execute(null);

        LoadMesPageViewModel loadMesPageViewModel = Ioc.Default.GetRequiredService<LoadMesPageViewModel>();
        loadMesPageViewModel.SaveCommand.Execute(null);

        MesTcpViewModel mesTcpViewModel = Ioc.Default.GetRequiredService<MesTcpViewModel>();
        mesTcpViewModel.SaveCommand.Execute(null);

        ProductiveViewModel productiveViewModel = Ioc.Default.GetRequiredService<ProductiveViewModel>();
        productiveViewModel.SaveCommand.Execute(null);

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