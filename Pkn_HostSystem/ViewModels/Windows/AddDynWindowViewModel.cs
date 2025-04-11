using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Windows;

public partial class AddDynWindowViewModel:ObservableRecipient
{
    public SnackbarService SnackbarService { get; set; }
    public LogBase<AddDynWindowViewModel> log;
    public AddDynWindowModel AddDynWindowModel { get; set; }
    [ObservableProperty] private string name;

    public AddDynWindowViewModel()
    {
        SnackbarService = new SnackbarService();
        log = new LogBase<AddDynWindowViewModel>(SnackbarService);
        //Model初始化
        AddDynWindowModel = new AddDynWindowModel();
    }

    #region 弹窗SnackbarService

    public void setSnackbarPresenter(SnackbarPresenter snackbarPresenter)
    {
        SnackbarService.SetSnackbarPresenter(snackbarPresenter);
    }

    #endregion
}