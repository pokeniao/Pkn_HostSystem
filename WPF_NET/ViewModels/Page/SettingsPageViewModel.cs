using WPF_NET.Base;
using WPF_NET.Models;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace WPF_NET.ViewModels;

public class SettingsPageViewModel
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

    #region 弹窗SnackbarService

    public void setSnackbarPresenter(SnackbarPresenter snackbarPresenter)
    {
        SnackbarService.SetSnackbarPresenter(snackbarPresenter);
    }

    #endregion
}