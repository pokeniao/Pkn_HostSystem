using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using log4net;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Page;
using Pkn_HostSystem.Views.Windows;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Windows
{
    public partial class SetLiveChartsParamViewModel :ObservableRecipient
    {
        private LogBase<SetLiveChartsParamViewModel> Log ;
        public SnackbarService SnackbarService { get; set; } = new SnackbarService();
        public SetLiveChartsParamModel SetLiveChartsParamModel { get; set; }

        public ObservableCollectionExtended<LoadMesDynContent> DynNetList { get; set; }

        public SetLiveChartsParamViewModel()
        {
            SetLiveChartsParamModel = new SetLiveChartsParamModel();
            Log = new LogBase<SetLiveChartsParamViewModel>(SnackbarService);

            DynNetList = Ioc.Default.GetRequiredService<MesTcpViewModel>().MesTcpModel.DynNetList;
        }

        /// <summary>
        /// 导入参量统计配置
        /// </summary>
        /// <param name="window"></param>
        [RelayCommand]
        public void DayTotalImportButton(SetLiveChartsParamWindow window)
        {
            ObservableCollection<DynCondition> observableCollection = new();
            observableCollection.Add(new DynCondition(){Name = "okh0-1" });
            observableCollection.Add(new DynCondition(){Name = "okh1-2" });
            observableCollection.Add(new DynCondition(){Name = "okh2-3" });
            observableCollection.Add(new DynCondition(){Name = "okh3-4" });
            observableCollection.Add(new DynCondition(){Name = "okh4-5" });
            observableCollection.Add(new DynCondition(){Name = "okh5-6" });
            observableCollection.Add(new DynCondition(){Name = "okh6-7" });
            observableCollection.Add(new DynCondition(){Name = "okh7-8" });
            observableCollection.Add(new DynCondition(){Name = "okh8-9" });
            observableCollection.Add(new DynCondition(){Name = "okh9-10" });
            observableCollection.Add(new DynCondition(){Name = "okh10-11" });
            observableCollection.Add(new DynCondition(){Name = "okh11-12" });
            observableCollection.Add(new DynCondition(){Name = "okh12-13" });
            observableCollection.Add(new DynCondition(){Name = "okh13-14" });
            observableCollection.Add(new DynCondition(){Name = "okh14-15" });
            observableCollection.Add(new DynCondition(){Name = "okh15-16" });
            observableCollection.Add(new DynCondition(){Name = "okh16-17" });
            observableCollection.Add(new DynCondition(){Name = "okh17-18" });
            observableCollection.Add(new DynCondition(){Name = "okh18-19" });
            observableCollection.Add(new DynCondition(){Name = "okh19-20" });
            observableCollection.Add(new DynCondition(){Name = "okh20-21" });
            observableCollection.Add(new DynCondition(){Name = "okh21-22" });
            observableCollection.Add(new DynCondition(){Name = "okh22-23" });
            observableCollection.Add(new DynCondition(){Name = "okh23-0" });
            observableCollection.Add(new DynCondition() { Name = "ngh0-1" });
            observableCollection.Add(new DynCondition() { Name = "ngh1-2" });
            observableCollection.Add(new DynCondition() { Name = "ngh2-3" });
            observableCollection.Add(new DynCondition() { Name = "ngh3-4" });
            observableCollection.Add(new DynCondition() { Name = "ngh4-5" });
            observableCollection.Add(new DynCondition() { Name = "ngh5-6" });
            observableCollection.Add(new DynCondition() { Name = "ngh6-7" });
            observableCollection.Add(new DynCondition() { Name = "ngh7-8" });
            observableCollection.Add(new DynCondition() { Name = "ngh8-9" });
            observableCollection.Add(new DynCondition() { Name = "ngh9-10" });
            observableCollection.Add(new DynCondition() { Name = "ngh10-11" });
            observableCollection.Add(new DynCondition() { Name = "ngh11-12" });
            observableCollection.Add(new DynCondition() { Name = "ngh12-13" });
            observableCollection.Add(new DynCondition() { Name = "ngh13-14" });
            observableCollection.Add(new DynCondition() { Name = "ngh14-15" });
            observableCollection.Add(new DynCondition() { Name = "ngh15-16" });
            observableCollection.Add(new DynCondition() { Name = "ngh16-17" });
            observableCollection.Add(new DynCondition() { Name = "ngh17-18" });
            observableCollection.Add(new DynCondition() { Name = "ngh18-19" });
            observableCollection.Add(new DynCondition() { Name = "ngh19-20" });
            observableCollection.Add(new DynCondition() { Name = "ngh20-21" });
            observableCollection.Add(new DynCondition() { Name = "ngh21-22" });
            observableCollection.Add(new DynCondition() { Name = "ngh22-23" });
            observableCollection.Add(new DynCondition() { Name = "ngh23-0" });



            LoadMesDynContent loadMesDynContent = new()
            {
                Name="产量统计",
                DynCondition = observableCollection,
                Message = "{\r\n\"OKS\":[\"[okh0-1]\",\"[okh1-2]\",\"[okh2-3]\",\"[okh3-4]\",\"[okh4-5]\",\"[okh5-6]\",\"[okh6-7]\",\"[okh7-8]\",\"[okh8-9]\",\"[okh9-10]\",\"[okh10-11]\",\"[okh11-12]\",\"[okh12-13]\",\"[okh13-14]\",\"[okh14-15]\",\"[okh15-16]\",\"[okh16-17]\",\"[okh17-18]\",\"[okh18-19]\",\"[okh19-20]\",\"[okh20-21]\",\"[okh21-22]\",\"[okh22-23]\",\"[okh23-0]\"]\r\n\"NGS\":[\"[ngh0-1]\",\"[ngh1-2]\",\"[ngh2-3]\",\"[ngh3-4]\",\"[ngh4-5]\",\"[ngh5-6]\",\"[ngh6-7]\",\"[ngh7-8]\",\"[ngh8-9]\",\"[ngh9-10]\",\"[ngh10-11]\",\"[ngh11-12]\",\"[ngh12-13]\",\"[ngh13-14]\",\"[ngh14-15]\",\"[ngh15-16]\",\"[ngh16-17]\",\"[ngh17-18]\",\"[ngh18-19]\",\"[ngh19-20]\",\"[ngh20-21]\",\"[ngh21-22]\",\"[ngh22-23]\",\"[ngh23-0]\"]\r\n\r\n\r\n}\r\n\r\n"
            };


          
            if (GlobalManager.DynDictionary.Lookup("产量统计").HasValue)
            {
                Log.WarningAndShow("添加动态通讯名称已存在", $"添加动态通讯名称已存在: 产量统计");
                return;
            }

            GlobalManager.DynDictionary.AddOrUpdate(loadMesDynContent);
        }

        /// <summary>
        /// 选中配置
        /// </summary>
        [RelayCommand]
        public void SelectDayTotalParamButton()
        {
            // string selectName = SetLiveChartsParamModel.DayProductionDynName;

        }

        /// <summary>
        /// 运行按钮
        /// </summary>
        public void RunButton(SetLiveChartsParamWindow window)
        {
            if (window.RunButton.Content == "启用")
            {
              



                window.RunButton.Content = "停用";
            }
            else
            {
                window.RunButton.Content = "启用";
            }

        }


        public void RunLiveCharts(CancellationTokenSource cts)
        {
            while (!cts.Token.IsCancellationRequested)
            {
                //产量统计
                var dayProductionDynName = SetLiveChartsParamModel.DayProductionDynName;
                //执行当前动态嵌入内容



                //解析JSON
                // JsonTool<object>.TryFormatJson()
            }
        }


        public void setSnackbarService(SnackbarPresenter snackbarPresenter)
        {
            SnackbarService.SetSnackbarPresenter(snackbarPresenter);
        }

    }
}