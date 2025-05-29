using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.Stations;
using System.Collections.ObjectModel;
using System.Windows.Documents;
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
                //实例化工位对象
                StationModel = new StationModel()
                {
                    Stations = new ObservableCollectionExtended<IEachStation>()
                };

                
                GlobalMannager.StationDictionary.AddOrUpdate(new EachStation<Station1>()
                {
                    Header = "工位1",
                    Items = new ObservableCollection<Station1>(),
                    UserLog = new LogControl<Station1>(new FlowDocument()),
                    ErrorLog = new LogControl<Station1>(new FlowDocument()),
                    DevLog = new LogControl<Station1>(new FlowDocument())
                });

                GlobalMannager.StationDictionary.AddOrUpdate(new EachStation<Station2>()
                {
                    Header = "工位2",
                    Items = new ObservableCollection<Station2>(),
                    UserLog = new LogControl<Station2>(new FlowDocument()),
                    ErrorLog = new LogControl<Station2>(new FlowDocument()),
                    DevLog = new LogControl<Station2>(new FlowDocument())
                });
                GlobalMannager.StationDictionary.Connect().Bind(StationModel.Stations).Subscribe();
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