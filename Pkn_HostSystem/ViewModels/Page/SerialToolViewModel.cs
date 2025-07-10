using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Page
{
    public partial class SerialToolViewModel :ObservableRecipient
    {
        public SnackbarService SnackbarService { get; set; }
        public LogBase<SerialToolViewModel> log;
        public SerialToolModel SerialToolModel { get; set; }

        public SerialToolViewModel()
        {
            SnackbarService = new SnackbarService();
            log = new LogBase<SerialToolViewModel>(SnackbarService);
            //Model初始化
            SerialToolModel = new SerialToolModel();
        }

        #region 弹窗SnackbarService

        public void setSnackbarPresenter(SnackbarPresenter snackbarPresenter)
        {
            SnackbarService.SetSnackbarPresenter(snackbarPresenter);
        }

        #endregion
    }
}