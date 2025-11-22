using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Page;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Windows
{
    public partial class SetLiveChartsParamViewModel : ObservableRecipient
    {
        private LogControl<SetLiveChartsParamViewModel> Log;
        public SnackbarService SnackbarService { get; set; } = new SnackbarService();
        public LiveChartsModel LiveChartsModel { get; set; }

        public ObservableCollectionExtended<LoadMesDynContent> DynNetList { get; set; }

        public SetLiveChartsParamModel SetLiveChartsParamModel { get; set; } = new SetLiveChartsParamModel();

        public List<string> XMethod { get; set; } = ["常量设置", "随小时更新(保存前24小时)", "随月份更新(保存31天)"];
        public List<string> YMethod { get; set; } = ["数量坐标", "百分比坐标"];



        public SetLiveChartsParamViewModel(LiveChartsModel liveChartsModel)
        {
            LiveChartsModel = liveChartsModel;
            Log = new LogControl<SetLiveChartsParamViewModel>(SnackbarService);
            DynNetList = Ioc.Default.GetRequiredService<MesTcpViewModel>().MesTcpModel.DynNetList;

            //赋值到页面
            SetLiveChartsParamModel.XDayTimeMethod = LiveChartsModel.XDayTimeMethod;
            SetLiveChartsParamModel.YDayTimeMethod = LiveChartsModel.YDayTimeMethod;
            SetLiveChartsParamModel.XAxesDayTimeLabelsYieldString = string.Join(",", Array.ConvertAll(LiveChartsModel.LabelsXAxesDayTimeYield.ToArray(), s =>
                 $"{s}"));
            SetLiveChartsParamModel.XOeeMethod = LiveChartsModel.XOeeMethod;
            SetLiveChartsParamModel.YOeeMethod = LiveChartsModel.YOeeMethod;
            SetLiveChartsParamModel.XAxesOeeLabelsYieldString = string.Join(",", Array.ConvertAll(LiveChartsModel.LabelsXAxesOEEYield.ToArray(), s =>
                $"{s}"));
        }

        /// <summary>
        /// 导入参量统计配置
        /// </summary>
        /// <param name="window"></param>
        [RelayCommand]
        public void DayTotalImportButton()
        {
            ObservableCollection<DynCondition> observableCollection = new();
            observableCollection.Add(new DynCondition() { Name = "okh0-1" });
            observableCollection.Add(new DynCondition() { Name = "okh1-2" });
            observableCollection.Add(new DynCondition() { Name = "okh2-3" });
            observableCollection.Add(new DynCondition() { Name = "okh3-4" });
            observableCollection.Add(new DynCondition() { Name = "okh4-5" });
            observableCollection.Add(new DynCondition() { Name = "okh5-6" });
            observableCollection.Add(new DynCondition() { Name = "okh6-7" });
            observableCollection.Add(new DynCondition() { Name = "okh7-8" });
            observableCollection.Add(new DynCondition() { Name = "okh8-9" });
            observableCollection.Add(new DynCondition() { Name = "okh9-10" });
            observableCollection.Add(new DynCondition() { Name = "okh10-11" });
            observableCollection.Add(new DynCondition() { Name = "okh11-12" });
            observableCollection.Add(new DynCondition() { Name = "okh12-13" });
            observableCollection.Add(new DynCondition() { Name = "okh13-14" });
            observableCollection.Add(new DynCondition() { Name = "okh14-15" });
            observableCollection.Add(new DynCondition() { Name = "okh15-16" });
            observableCollection.Add(new DynCondition() { Name = "okh16-17" });
            observableCollection.Add(new DynCondition() { Name = "okh17-18" });
            observableCollection.Add(new DynCondition() { Name = "okh18-19" });
            observableCollection.Add(new DynCondition() { Name = "okh19-20" });
            observableCollection.Add(new DynCondition() { Name = "okh20-21" });
            observableCollection.Add(new DynCondition() { Name = "okh21-22" });
            observableCollection.Add(new DynCondition() { Name = "okh22-23" });
            observableCollection.Add(new DynCondition() { Name = "okh23-0" });
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
                Name = "产量统计",
                DynCondition = observableCollection,
                Message = "{\r\n\"OKS\":[\"[okh0-1]\",\"[okh1-2]\",\"[okh2-3]\",\"[okh3-4]\",\"[okh4-5]\",\"[okh5-6]\",\"[okh6-7]\",\"[okh7-8]\",\"[okh8-9]\",\"[okh9-10]\",\"[okh10-11]\",\"[okh11-12]\",\"[okh12-13]\",\"[okh13-14]\",\"[okh14-15]\",\"[okh15-16]\",\"[okh16-17]\",\"[okh17-18]\",\"[okh18-19]\",\"[okh19-20]\",\"[okh20-21]\",\"[okh21-22]\",\"[okh22-23]\",\"[okh23-0]\"],\r\n\"NGS\":[\"[ngh0-1]\",\"[ngh1-2]\",\"[ngh2-3]\",\"[ngh3-4]\",\"[ngh4-5]\",\"[ngh5-6]\",\"[ngh6-7]\",\"[ngh7-8]\",\"[ngh8-9]\",\"[ngh9-10]\",\"[ngh10-11]\",\"[ngh11-12]\",\"[ngh12-13]\",\"[ngh13-14]\",\"[ngh14-15]\",\"[ngh15-16]\",\"[ngh16-17]\",\"[ngh17-18]\",\"[ngh18-19]\",\"[ngh19-20]\",\"[ngh20-21]\",\"[ngh21-22]\",\"[ngh22-23]\",\"[ngh23-0]\"]\r\n}\r\n"
            };

            if (GlobalManager.DynDictionary.Lookup("产量统计").HasValue)
            {
                Log.WarningAndShowTask($"添加动态通讯名称已存在: 产量统计");
                return;
            }

            LiveChartsModel.DayProductionDynName = "产量统计";
            GlobalManager.DynDictionary.AddOrUpdate(loadMesDynContent);
        }

        /// <summary>
        /// 导入参量统计配置
        /// </summary>
        /// <param name="window"></param>
        [RelayCommand]

        public void OEEImportButton()
        {
            ObservableCollection<DynCondition> observableCollection = new();
            observableCollection.Add(new DynCondition() { Name = "运行时间" });
            observableCollection.Add(new DynCondition() { Name = "报警时间" });
            observableCollection.Add(new DynCondition() { Name = "待机时间" });
            observableCollection.Add(new DynCondition() { Name = "当日总参量" });
            observableCollection.Add(new DynCondition() { Name = "合格产量" });
            observableCollection.Add(new DynCondition() { Name = "CT" });
            observableCollection.Add(new DynCondition() { Name = "额定产量" });


            LoadMesDynContent loadMesDynContent = new()
            {
                Name = "OEE",
                DynCondition = observableCollection,
                Message = "{\r\n\"运行时间\":\"[运行时间]\",\r\n\"报警时间\":\"[报警时间]\",\r\n\"待机时间\":\"[待机时间]\",\r\n\"当日总产量\":\"[当日总参量]\",\r\n\"合格产量\":\"[合格产量]\",\r\n\"CT\":\"[CT]\",\r\n\"额定产量\":\"[额定产量]\",\r\n}\r\n"
            };

            if (GlobalManager.DynDictionary.Lookup("OEE").HasValue)
            {
                Log.WarningAndShowTask($"添加动态通讯名称已存在: OEE");
                return;
            }

            LiveChartsModel.OeeDynName = "OEE";
            GlobalManager.DynDictionary.AddOrUpdate(loadMesDynContent);
        }


        /// <summary>
        /// 停机运行时长配置
        /// </summary>
        [RelayCommand]
        public void RunTimeImportButton()
        {
            ObservableCollection<DynCondition> observableCollection = new();
            observableCollection.Add(new DynCondition() { Name = "运行总时长" });
            observableCollection.Add(new DynCondition() { Name = "报警总时长" });
            observableCollection.Add(new DynCondition() { Name = "待机总时长" });


            LoadMesDynContent loadMesDynContent = new()
            {
                Name = "停机运行时长",
                DynCondition = observableCollection,
                Message = "{\r\n\"运行总时长\":\"[运行总时长]\",\r\n\"报警总时长\":\"[报警总时长]\",\r\n\"待机总时长\":\"[待机总时长]\"\r\n}\r\n"
            };

            if (GlobalManager.DynDictionary.Lookup("停机运行时长").HasValue)
            {
                Log.WarningAndShowTask($"添加动态通讯名称已存在: 停机运行时长");
                return;
            }


            GlobalManager.DynDictionary.AddOrUpdate(loadMesDynContent);
            LiveChartsModel.RunStopTimeDynName = "停机运行时长";
        }



        /// <summary>
        /// OKNG配置
        /// </summary>
        [RelayCommand]
        public void OkNgImportButton()
        {
            ObservableCollection<DynCondition> observableCollection = new();
            observableCollection.Add(new DynCondition() { Name = "NG总数" });
            observableCollection.Add(new DynCondition() { Name = "OK总数" });

            LoadMesDynContent loadMesDynContent = new()
            {
                Name = "良品统计",
                DynCondition = observableCollection,
                Message = "{\r\n\"NG总数\":\"[NG总数]\",\r\n\"OK总数\":\"[OK总数]\"\r\n}\r\n"
            };

            if (GlobalManager.DynDictionary.Lookup("良品统计").HasValue)
            {
                Log.WarningAndShowTask($"添加动态通讯名称已存在: 良品统计");
                return;
            }


            GlobalManager.DynDictionary.AddOrUpdate(loadMesDynContent);
            LiveChartsModel.OkNgDynName = "良品统计";
        }
        /// <summary>
        /// 运行按钮
        /// </summary>
        [RelayCommand]
        public async Task RunButton()
        {
            LiveChartsTestViewModel liveChartsTestViewModel = Ioc.Default.GetRequiredService<LiveChartsTestViewModel>();
            liveChartsTestViewModel.RunCommand.Execute(null);
        }
        public void setSnackbarService(SnackbarPresenter snackbarPresenter)
        {
            SnackbarService.SetSnackbarPresenter(snackbarPresenter);
        }


        [RelayCommand]
        public void OEEAxisSet()
        {
            LiveChartsModel.XOeeMethod = SetLiveChartsParamModel.XOeeMethod;
            LiveChartsModel.YOeeMethod = SetLiveChartsParamModel.YOeeMethod;

            switch (SetLiveChartsParamModel.XOeeMethod)
            {
                case "常量设置":
                    LiveChartsModel.LabelsXAxesOEEYield.Clear();
                    string[] strings = SetLiveChartsParamModel.XAxesOeeLabelsYieldString.Split(",");
                    LiveChartsModel.LabelsXAxesOEEYield.AddRange(strings);
                    break;
                case "随月份更新(保存31天)":
                    int count = LiveChartsModel.LabelsXAxesOEEYield.Count;
                    if (count > 0)
                    {
                        if (!Regex.IsMatch(LiveChartsModel.LabelsXAxesOEEYield[count - 1] , @"^\d{4}-\d{1,2}-\d{1,2}"))
                        {
                            LiveChartsModel.LabelsXAxesOEEYield.Clear();
                        }
                    }
                    break;
            }

          
        }

        [RelayCommand]
        public void DateTimeAxisSet()
        {
            LiveChartsModel.XDayTimeMethod = SetLiveChartsParamModel.XDayTimeMethod;
            LiveChartsModel.YDayTimeMethod = SetLiveChartsParamModel.YDayTimeMethod;

            if (SetLiveChartsParamModel.XDayTimeMethod == "常量设置")
            {
                LiveChartsModel.LabelsXAxesDayTimeYield.Clear();
                string[] strings = SetLiveChartsParamModel.XAxesDayTimeLabelsYieldString.Split(",");
                LiveChartsModel.LabelsXAxesDayTimeYield.AddRange(strings);
            }
        }
    }
}