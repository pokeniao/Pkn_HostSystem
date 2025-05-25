using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Page
{
    public partial class StationViewModel : ObservableRecipient
    {
        public StationModel StationModel { get; set; } = AppJsonTool<StationModel>.Load();

        public SnackbarService SnackbarService { get; set; } = new();

        public LogBase<StationViewModel> Log { get; set; }
        public StationViewModel()
        {
            if (StationModel == null)
            {
                StationModel = new StationModel();
            }
            else
            {

            }
            Log = new LogBase<StationViewModel>(SnackbarService);
        }
        #region 弹窗SnackbarService
        public void setSnackbarPresenter(SnackbarPresenter snackbarPresenter)
        {
            SnackbarService.SetSnackbarPresenter(snackbarPresenter);
        }
        #endregion

        [RelayCommand]
        public void Save()
        {
            AppJsonTool<StationModel>.Save(StationModel);
        }
    }
}