using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Windows
{
    public partial class LoginViewModel:ObservableRecipient
    {
        public SnackbarService SnackbarService { get; set; }
        public LogControl<LoginViewModel> log;
        public LoginModel LoginModel { get; set; }

        public LoginViewModel()
        {
            SnackbarService = new SnackbarService();
            log = new LogControl<LoginViewModel>(SnackbarService);
            //Model初始化
            LoginModel = new LoginModel();
        }

        #region 弹窗SnackbarService
        public void setSnackbarPresenter(SnackbarPresenter snackbarPresenter)
        {
            SnackbarService.SetSnackbarPresenter(snackbarPresenter);
        }
        #endregion
    }
}