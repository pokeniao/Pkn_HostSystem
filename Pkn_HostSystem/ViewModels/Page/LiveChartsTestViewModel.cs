using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Services.Page.LoadMes;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.Views.Pages;
using Pkn_HostSystem.Views.Windows;
using SkiaSharp;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Page
{
    public partial class LiveChartsTestViewModel : ObservableRecipient
    {
        public LiveChartsModel LiveChartsModel { get; set; } = JsonTool<LiveChartsModel>.Load();
        public LogControl<LiveChartsTestViewModel> Log;

        public SnackbarService SnackbarService = new SnackbarService();

        //循环读取数据的CTS
        public CancellationTokenSource ctsCycRun { get; set; }

        //绿色
        private static readonly RadialGradientPaint green = new RadialGradientPaint(
            [
                new SKColor(130, 220, 100), // 中心亮绿
                new SKColor(150, 230, 110), // 中间柔绿
                new SKColor(100, 180, 90), // 次深绿
                new SKColor(80, 160, 70) // 最外围深绿
            ],
            center: null, // 可选，自定义圆心
            radius: 0, // 0 表示自动半径
            [0f, 0.4f, 0.7f, 1f],
            tileMode: SKShaderTileMode.Clamp
        );

        //红色
        private static readonly RadialGradientPaint red = new RadialGradientPaint(
            [
                new SKColor(200, 50, 70), // 中心酒红
                new SKColor(230, 80, 90), // 柔和过渡红
                new SKColor(240, 110, 110), // 珊瑚红
                new SKColor(160, 40, 50) // 边缘暗红
            ],
            center: null,
            radius: 0,
            [0f, 0.3f, 0.6f, 1f],
            tileMode: SKShaderTileMode.Clamp);

        //蓝色
        private readonly RadialGradientPaint blue = new RadialGradientPaint(
            [
                new SKColor(40, 100, 220), // 深科技蓝
                new SKColor(80, 140, 240), // 过渡蓝
                new SKColor(120, 180, 255), // 天蓝
                new SKColor(30, 80, 160) // 边缘深蓝
            ],
            center: null,
            radius: 0,
            [0f, 0.4f, 0.75f, 1f],
            tileMode: SKShaderTileMode.Clamp
        );

        #region 饼图--良率统计

        public ISeries[] OkTotalPieSeries { get; set; }

        public ISeries[] TimePieSeries { get; set; }

        public LabelVisual TotalTitlePie { get; set; } =
            new()
            {
                Text = "24小时良率产量统计",
                TextSize = 15,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint
                {
                    Color = GlobalManager.ThemeSkColor,
                    SKTypeface = SKTypeface.FromFamilyName("Microsoft YaHei")
                }
            };

        public LabelVisual TotalTitlePie2 { get; set; } =
            new()
            {
                Text = "24小时耗时统计",
                TextSize = 15,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint
                {
                    Color = GlobalManager.ThemeSkColor,
                    SKTypeface = SKTypeface.FromFamilyName("Microsoft YaHei")
                }
            };

        #endregion

        #region 柱状图产量统计

        public ISeries[] DayTimeYieldSeries { get; set; }

        public Axis[] XAxesDayTimeYield { get; set; }


        public Axis[] YAxesDayTimeYield { get; set; }

        #endregion

        #region 柱状图OEE

        public ISeries[] OEEYieldSeries { get; set; }

        public Axis[] XAxesOEEYield { get; set; }


        public Axis[] YAxesOEEYield { get; set; }

        #endregion


        public LiveChartsTestViewModel()
        {
            if (LiveChartsModel == null)
            {
                LiveChartsModel = new LiveChartsModel();
            }

            LiveChartsModel.Oees.AddRange(LiveChartsModel.OeeSave.Select(v => new ObservableValue(v)));

            Log = new LogControl<LiveChartsTestViewModel>(SnackbarService);

            YAxesDayTimeYield =
            [
                new Axis
                {
                    Name = "产量",
                    NamePaint = new SolidColorPaint(GlobalManager.ThemeSkColor)
                    {
                        SKTypeface = SKTypeface.FromFamilyName("Microsoft YaHei")
                    },
                    LabelsPaint = new SolidColorPaint(SKColors.Green),
                    TextSize = 13,
                }
            ];
            XAxesDayTimeYield =
            [
                new Axis
                {
                    Name = "时间",
                    Labels = LiveChartsModel.LabelsXAxesDayTimeYield,
                    NamePaint = new SolidColorPaint(GlobalManager.ThemeSkColor)
                    {
                        SKTypeface = SKTypeface.FromFamilyName("Microsoft YaHei")
                    },
                    LabelsPaint = new SolidColorPaint(SKColors.Blue),
                    TextSize = 13,
                }
            ];

            XAxesOEEYield =
            [
                new Axis
                {
                    Name = "时间",
                    Labels = LiveChartsModel.LabelsXAxesOEEYield,
                    NamePaint = new SolidColorPaint(GlobalManager.ThemeSkColor)
                    {
                        SKTypeface = SKTypeface.FromFamilyName("Microsoft YaHei")
                    },
                    LabelsPaint = new SolidColorPaint(SKColors.Blue),
                    TextSize = 13,
                }
            ];
            YAxesOEEYield =
            [
                new Axis
                {
                    Name = "OEE",
                    Labeler = value => $"{value :P2}",
                    NamePaint = new SolidColorPaint(GlobalManager.ThemeSkColor)
                    {
                        SKTypeface = SKTypeface.FromFamilyName("Microsoft YaHei")
                    },
                    LabelsPaint = new SolidColorPaint(SKColors.Green),
                    TextSize = 13,
                }
            ];
            //柱状图-产量统计-数量统计
            DayTimeYieldSeries =
            [
                new ColumnSeries<ObservableValue>
                {
                    Values = LiveChartsModel.All,
                    Fill = new SolidColorPaint(new SKColor(0 ,0 ,255)),
                    Stroke = null,
                    MaxBarWidth = 35,
                    IgnoresBarPosition = true,
                },
                new ColumnSeries<ObservableValue>
                {
                    Values = LiveChartsModel.Oks,
                    Fill = new SolidColorPaint(new SKColor(0 ,255 ,0 )),
                    Stroke = null,
                    MaxBarWidth = 30,
                    IgnoresBarPosition = true
                },
                //堆叠的柱状图StackedColumnSeries
                new ColumnSeries<ObservableValue>
                {
                    Values = LiveChartsModel.Ngs,
                    Fill = new SolidColorPaint(new SKColor(255 ,0 ,0)),
                    Stroke = null,
                    MaxBarWidth = 30,
                    IgnoresBarPosition = true
                },
                

                // new LineSeries<ObservableValue> { Values = LiveChartsModel.All, Fill = null, GeometrySize = 0 }
            ];
            //柱状图-OEE-数量统计
            OEEYieldSeries =
            [
                new LineSeries<ObservableValue> { Values = LiveChartsModel.Oees, Fill = null, GeometrySize = 10 },
            ];


            //饼图-良率统计
            OkTotalPieSeries =
            [
                new PieSeries<ObservableValue>
                {
                    Name = "OK", Values = [LiveChartsModel.Ok], Stroke = null, Fill = green,
                },
                new PieSeries<ObservableValue>
                {
                    Name = "NG",
                    Values = [LiveChartsModel.Ng],
                    Stroke = null,
                    Fill = red,
                    // DataLabelsPaint = new SolidColorPaint(GlobalManager.ThemeSkColor), //页面上显示数据
                    Pushout = 10,
                    OuterRadiusOffset = 20
                }
            ];

            TimePieSeries =
            [
                new PieSeries<ObservableValue>
                {
                    Values = [LiveChartsModel.RunTime],
                    Fill = green,
                    // GlobalManager.ThemeSkColor
                    DataLabelsPaint = new SolidColorPaint(new SKColor(0, 0, 255))
                    {
                        SKTypeface = SKTypeface.FromFamilyName("Microsoft YaHei")
                    }, //页面上显示数据
                    OuterRadiusOffset = 0,
                    ToolTipLabelFormatter =
                        point =>
                        {
                            var pv = point.Coordinate.PrimaryValue;
                            var sv = point.StackedValue!;
                            var a = $"{pv}/{sv.Total}{Environment.NewLine}{sv.Share:P2}";
                            return a;
                        },
                    DataLabelsFormatter =
                        point =>
                        {
                            var pv = point.Coordinate.PrimaryValue;
                            var sv = point.StackedValue!;
                            var a = $"Start {Environment.NewLine}{pv}/{sv.Total}{Environment.NewLine}{sv.Share:P2}";
                            return a;
                        }
                },
                new PieSeries<ObservableValue>
                {
                    Values = [LiveChartsModel.StopTime],
                    Fill = blue,
                    DataLabelsPaint = new SolidColorPaint(GlobalManager.ThemeSkColor)
                    {
                        SKTypeface = SKTypeface.FromFamilyName("Microsoft YaHei")
                    }, //页面上显示数据
                    OuterRadiusOffset = 25,
                    ToolTipLabelFormatter =
                        point =>
                        {
                            var pv = point.Coordinate.PrimaryValue;
                            var sv = point.StackedValue!;
                            var a = $"{pv}/{sv.Total}{Environment.NewLine}{sv.Share:P2}";
                            return a;
                        },
                    DataLabelsFormatter =
                        point =>
                        {
                            var pv = point.Coordinate.PrimaryValue;
                            var sv = point.StackedValue!;
                            var a = $"Wait {Environment.NewLine}{pv}/{sv.Total}{Environment.NewLine}{sv.Share:P2}";
                            return a;
                        }
                },
                new PieSeries<ObservableValue>
                {
                    Values = [LiveChartsModel.ErrorTime],
                    Fill = red,
                    DataLabelsPaint = new SolidColorPaint(GlobalManager.ThemeSkColor)
                    {
                        SKTypeface = SKTypeface.FromFamilyName("Microsoft YaHei")
                    }, //页面上显示数据

                    OuterRadiusOffset = 50,
                    ToolTipLabelFormatter =
                        point =>
                        {
                            var pv = point.Coordinate.PrimaryValue;
                            var sv = point.StackedValue!;

                            var a = $"{pv}/{sv.Total}{Environment.NewLine}{sv.Share:P2}";
                            return a;
                        },
                    DataLabelsFormatter =
                        point =>
                        {
                            var pv = point.Coordinate.PrimaryValue;
                            var sv = point.StackedValue!;

                            var a = $"Alarm {Environment.NewLine}{pv}/{sv.Total}{Environment.NewLine}{sv.Share:P2}";
                            return a;
                        }
                }
            ];
        }


        #region 设置参数

        [RelayCommand]
        public void SetParamButton(LiveChartsTestPage page)
        {
            var setLiveChartsParamWindow = new SetLiveChartsParamWindow(LiveChartsModel);

            var showDialog = setLiveChartsParamWindow.ShowDialog();
        }

        #endregion


        #region 运行

        /// <summary>
        /// 运行
        /// </summary>
        [RelayCommand]
        public async Task Run()
        {
            if (LiveChartsModel.RunLiveChartsButton == "启用")
            {
                ctsCycRun = new CancellationTokenSource();
                Task.Run(() => RunLiveCharts(ctsCycRun));
                LiveChartsModel.RunLiveChartsButton = "停用";
            }
            else
            {
                ctsCycRun.Cancel();
                LiveChartsModel.RunLiveChartsButton = "启用";
            }
        }

        public async Task ReStart()
        {
            if (LiveChartsModel.RunLiveChartsButton == "停用")
            {
                ctsCycRun = new CancellationTokenSource();
                Task.Run(() => RunLiveCharts(ctsCycRun));
            }
        }

        public async Task RunLiveCharts(CancellationTokenSource cts)
        {
            TraceContext.Name = "产量统计循环";
            LoadMesService loadMesService = new LoadMesService();
            bool isJson;
            bool sueeced;
            string? result;
            string tryFormatJson;
            while (!cts.Token.IsCancellationRequested)
            {
                try
                {
                    var liveChartsTestViewModel = Ioc.Default.GetRequiredService<LiveChartsTestViewModel>();
                    JObject jObject;
                    //产量统计
                    if (LiveChartsModel.DateTimeRun)
                    {
                        var dayProductionDynName = LiveChartsModel.DayProductionDynName;
                        //执行当前动态嵌入内容
                        (sueeced, result) = await loadMesService.DynMessage(dayProductionDynName, cts, true);
                        if (!sueeced)
                        {
                            Log.Error($"[{TraceContext.Name}]--进行动态嵌入返回失败,等待5s后从试");
                            await Task.Delay(5000, cts.Token);
                            continue;
                        }

                        //解析JSON
                        tryFormatJson = JsonTool<object>.TryFormatJson(result, out isJson);

                        if (!isJson)
                        {
                            Log.Error($"[{TraceContext.Name}]--产量统计解析JSON失败,等待5s后从试");
                            await Task.Delay(5000, cts.Token);
                            continue;
                        }


                        jObject = JObject.Parse(result);

                        JArray? OksJArray = jObject.SelectToken("OKS") as JArray;

                        double OksTotal = 0;
                        //OKS修改参数
                        if (OksJArray != null)
                        {
                            for (int i = 0;
                                 i < Math.Min(OksJArray.Count, liveChartsTestViewModel.LiveChartsModel.Oks.Count);
                                 i++)
                            {
                                if (double.TryParse(OksJArray[i]?.ToString(), out double value))
                                {
                                    liveChartsTestViewModel.LiveChartsModel.Oks[i].Value = value;
                                    OksTotal = OksTotal + value;
                                }
                            }
                        }

                        JArray? NgsJArray = jObject.SelectToken("NGS") as JArray;

                        double NgsTotal = 0;
                        //NGS修改参数
                        if (NgsJArray != null)
                        {
                            for (int i = 0;
                                 i < Math.Min(NgsJArray.Count, liveChartsTestViewModel.LiveChartsModel.Ngs.Count);
                                 i++)
                            {
                                if (double.TryParse(NgsJArray[i]?.ToString(), out double value))
                                {
                                    liveChartsTestViewModel.LiveChartsModel.Ngs[i].Value = value;
                                    NgsTotal = NgsTotal + value;
                                }
                            }
                        }

                        //统计
                        for (int i = 0;
                             i < Math.Min(OksJArray.Count, NgsJArray.Count);
                             i++)
                        {
                            if (double.TryParse(NgsJArray[i]?.ToString(), out double value))
                            {
                                if (double.TryParse(OksJArray[i]?.ToString(), out double value2))
                                {
                                    liveChartsTestViewModel.LiveChartsModel.All[i].Value = value + value2;
                                }
                            }
                        }


                    }

                    //良率饼图统计
                    if (LiveChartsModel.OkNgRun)
                    {
                        string okNgDynName = LiveChartsModel.OkNgDynName;

                        //执行当前动态嵌入内容
                        (sueeced, result) = await loadMesService.DynMessage(okNgDynName, cts, true);

                        if (!sueeced)
                        {
                            Log.Error($"[{TraceContext.Name}]--进行动态嵌入返回失败,等待5s后从试");
                            await Task.Delay(5000, cts.Token);
                            continue;
                        }

                        //解析JSON
                        tryFormatJson = JsonTool<object>.TryFormatJson(result, out isJson);

                        if (!isJson)
                        {
                            Log.Error($"[{TraceContext.Name}]--停机运行时长统计解析JSON失败,等待5s后从试");
                            await Task.Delay(5000, cts.Token);
                            continue;
                        }
                        JObject jObject2 = JObject.Parse(result);

                        if (double.TryParse(jObject2.SelectToken("NG总数")?.ToString(), out double value))
                        {
                            liveChartsTestViewModel.LiveChartsModel.Ng.Value = value;
                        }

                        if (double.TryParse(jObject2.SelectToken("OK总数")?.ToString(), out double value2))
                        {
                          
                            liveChartsTestViewModel.LiveChartsModel.Ok.Value = value2;
                        }
                      
                        
                    }

                    //运行和停止时间统计
                    if (LiveChartsModel.WaitAlarmRun)
                    {
                        string runStopTimeDynName = LiveChartsModel.RunStopTimeDynName;

                        //执行当前动态嵌入内容
                        (sueeced, result) = await loadMesService.DynMessage(runStopTimeDynName, cts, true);

                        if (!sueeced)
                        {
                            Log.Error($"[{TraceContext.Name}]--进行动态嵌入返回失败,等待5s后从试");
                            await Task.Delay(5000, cts.Token);
                            continue;
                        }

                        //解析JSON
                        tryFormatJson = JsonTool<object>.TryFormatJson(result, out isJson);

                        if (!isJson)
                        {
                            Log.Error($"[{TraceContext.Name}]--停机运行时长统计解析JSON失败,等待5s后从试");
                            await Task.Delay(5000, cts.Token);
                            continue;
                        }

                        JObject jObject2 = JObject.Parse(result);

                        if (double.TryParse(jObject2.SelectToken("运行总时长")?.ToString(), out double value3))
                        {
                            liveChartsTestViewModel.LiveChartsModel.RunTime.Value = value3;
                        }

                        if (double.TryParse(jObject2.SelectToken("报警总时长")?.ToString(), out double value4))
                        {
                            liveChartsTestViewModel.LiveChartsModel.ErrorTime.Value = value4;
                        }

                        if (double.TryParse(jObject2.SelectToken("待机总时长")?.ToString(), out double value5))
                        {
                            liveChartsTestViewModel.LiveChartsModel.StopTime.Value = value5;
                        }
                    }
                    //OEE
                    if (LiveChartsModel.OeeRun)
                    {
                        //OEE统计
                        //执行当前动态嵌入内容
                        (sueeced, result) = await loadMesService.DynMessage(LiveChartsModel.OeeDynName, cts, true);
                        if (!sueeced)
                        {
                            Log.Error($"[{TraceContext.Name}]--进行动态嵌入返回失败,等待5s后从试");
                            await Task.Delay(5000, cts.Token);
                            continue;
                        }

                        //解析JSON
                        tryFormatJson = JsonTool<object>.TryFormatJson(result, out isJson);

                        if (!isJson)
                        {
                            Log.Error($"[{TraceContext.Name}]--产量统计解析JSON失败,等待5s后从试");
                            await Task.Delay(5000, cts.Token);
                            continue;
                        }

                        jObject = JObject.Parse(result);

                        bool b1 = double.TryParse(jObject.SelectToken("运行时间")?.ToString(), out double runTime);
                        bool b2 = double.TryParse(jObject.SelectToken("报警时间")?.ToString(), out double alarmTime);
                        bool b3 = double.TryParse(jObject.SelectToken("待机时间")?.ToString(), out double waitTime);
                        bool b4 = double.TryParse(jObject.SelectToken("当日总产量")?.ToString(), out double totleProduction);
                        bool b5 = double.TryParse(jObject.SelectToken("合格产量")?.ToString(), out double okProduction);
                        bool b6 = double.TryParse(jObject.SelectToken("CT")?.ToString(), out double CT);
                        bool b7 = double.TryParse(jObject.SelectToken("额定产量")?.ToString(), out double needProduction);

                        if (b1 && b2 && b3 && b4 && b5 && b6 && b7)
                        {
                            double Quality = okProduction / totleProduction;
                            StaticArrayRegister.WriteRegisterValue(10, $"{Quality:P2}");
                            double Availability = runTime / (alarmTime + waitTime + runTime);
                            StaticArrayRegister.WriteRegisterValue(11, $"{Availability:P2}");
                            double Performance = totleProduction / needProduction;
                            StaticArrayRegister.WriteRegisterValue(12, $"{Performance:P2}");
                            double oee = Quality * Availability * Performance;
                            StaticArrayRegister.WriteRegisterValue(13, $"{oee:P2}");

                            switch (LiveChartsModel.XOeeMethod)
                            {
                                case "随月份更新(保存31天)":
                                    //获取今天的时间添加到X轴
                                    string today = DateTime.Now.ToString("yyyy-MM-dd");

                                    int count = LiveChartsModel.LabelsXAxesOEEYield.Count;
                                    if (count > 0)
                                    {
                                        if (!LiveChartsModel.LabelsXAxesOEEYield[count - 1].Equals(today))
                                        {
                                            LiveChartsModel.LabelsXAxesOEEYield.Add(today);
                                            liveChartsTestViewModel.LiveChartsModel.Oees.Add(new ObservableValue());
                                            if (LiveChartsModel.LabelsXAxesOEEYield.Count > 31)
                                            {
                                                LiveChartsModel.LabelsXAxesOEEYield.RemoveAt(0);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LiveChartsModel.LabelsXAxesOEEYield.Add(today);
                                        liveChartsTestViewModel.LiveChartsModel.Oees.Add(new ObservableValue());
                                    }

                                    count = LiveChartsModel.LabelsXAxesOEEYield.Count;

                                    //写入到对应的
                                    liveChartsTestViewModel.LiveChartsModel.Oees[count - 1].Value = oee;
                                    break;
                            }
                        }

                    }


                    await Task.Delay(LiveChartsModel.TimeCyc, cts.Token);
                }
                catch (Exception e)
                {
                    Log.Error($"[{TraceContext.Name}]--发送错误:{e}");
                }
            }
        }

        #endregion


        #region 刷新页面

        /// <summary>
        /// 刷新页面
        /// </summary>
        /// <param name="page"></param>
        [RelayCommand]
        public void Refresh(LiveChartsTestPage page)
        {
            foreach (var timePieSeries in TimePieSeries)
            {
                timePieSeries.DataLabelsPaint = new SolidColorPaint(GlobalManager.ThemeSkColor);
            }

            foreach (var series in OkTotalPieSeries)
            {
                series.DataLabelsPaint = new SolidColorPaint(GlobalManager.ThemeSkColor);
            }
        }

        #endregion

        #region SnackBar弹窗

        public void setSnackbarService(SnackbarPresenter snackbarPresenter)
        {
            SnackbarService.SetSnackbarPresenter(snackbarPresenter);
        }

        #endregion

        #region 保存当前Model

        [RelayCommand]
        public void Save()
        {
            LiveChartsModel.OeeSave = LiveChartsModel.Oees.Select(o => o.Value).ToList();
            JsonTool<LiveChartsModel>.Save(LiveChartsModel);
        }

        #endregion
    }
}