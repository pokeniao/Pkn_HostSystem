using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.Views.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Windows
{
    public partial class SetLiveChartsParamViewModel :ObservableRecipient
    {
        private LogBase<SetLiveChartsParamViewModel> Log ;
        public SnackbarService SnackbarService { get; set; } = new SnackbarService();
        public SetLiveChartsParamModel SetLiveChartsParamModel { get; set; }

        public SetLiveChartsParamViewModel()
        {
            SetLiveChartsParamModel = new SetLiveChartsParamModel();
            Log = new LogBase<SetLiveChartsParamViewModel>(SnackbarService);
        }

        [RelayCommand]
        public void AckButton(SetLiveChartsParamWindow window)
        {

        }
        public void setSnackbarService(SnackbarPresenter snackbarPresenter)
        {
            SnackbarService.SetSnackbarPresenter(snackbarPresenter);
        }

    }
}