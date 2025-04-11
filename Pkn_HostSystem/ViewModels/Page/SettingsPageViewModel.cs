using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Page;

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