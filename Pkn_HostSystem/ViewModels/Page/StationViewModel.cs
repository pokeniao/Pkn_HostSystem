using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Core.Interface;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Static;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Page
{
    public partial class StationViewModel : ObservableRecipient
    {
        public StationModel StationModel { get; set; } = JsonTool<StationModel>.Load();

        public SnackbarService SnackbarService { get; set; } = new();

        public LogBase<StationViewModel> Log { get; set; }

        public StationViewModel()
        {
            if (StationModel == null)
            {
                //实例化工位对象
                StationModel = new StationModel()
                {
                    Stations = new ObservableCollectionExtended<IEachStation>()
                };
                GlobalManager.StationDictionary.Connect().Bind(StationModel.Stations).Subscribe();
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
            JsonTool<StationModel>.Save(StationModel);
        }
    }
}