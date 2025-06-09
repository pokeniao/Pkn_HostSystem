using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Page
{
    public partial class TcpToolViewModel:ObservableRecipient
    {
        public TcpToolModel TcpToolModel { get; set; } = AppJsonTool<TcpToolModel>.Load();

        private LogBase<TcpToolViewModel> Log;

        public SnackbarService SnackbarService { get; set; } = new();

        public TcpToolViewModel()
        {
            if (TcpToolModel == null)
            {
                TcpToolModel = new TcpToolModel();
            }

            Log = new LogBase<TcpToolViewModel>(SnackbarService);
        }


        #region 弹窗SnackbarService
        public void setSnackbarPresenter(SnackbarPresenter snackbarPresenter)
        {
            SnackbarService.SetSnackbarPresenter(snackbarPresenter);
        }
        #endregion

        #region 保存程序

        [RelayCommand]
        public void Save()
        {
            AppJsonTool<TcpToolModel>.Save(TcpToolModel);

        }

        #endregion
    }
}