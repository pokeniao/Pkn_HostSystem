using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Force.DeepCloner;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.SkiaSharpView.VisualElements;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.Views.Pages;
using Pkn_HostSystem.Views.Windows;
using SkiaSharp;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Page
{
    public partial class LiveChartsTestViewModel : ObservableRecipient
    {
        public LiveChartsModel LiveChartsModel { get; set; } = JsonTool<LiveChartsModel>.Load();
        public LogControl<LiveChartsTestViewModel> Log;
        public SnackbarService SnackbarService = new SnackbarService();

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

        #region 柱状图

        public ISeries[] DayTimeYieldSeries { get; set; }

        public Axis[] XAxesDayTimeYield { get; set; }
            =
            [
                new Axis
                {
                    Name = "时间",
                    Labels =
                    [
                        "0:00-1:00", "1:00-2:00", "2:00-3:00", "3:00-4:00", "4:00-5:00", "5:00-6:00", "6:00-7:00", "7:00-8:00", "8:00-9:00", "9:00-10:00", "10:00-11:00",
                        "11:00-12:00", "12:00-13:00", "13:00-14:00", "14:00-15:00", "15:00-16:00", "16:00-17:00", "17:00-18:00", "18:00-19:00", "19:00-20:00", "20:00-21:00",
                        "21:00-22:00", "22:00-23:00", "23:00-0:00"
                    ],
                    NamePaint = new SolidColorPaint(GlobalManager.ThemeSkColor)
                    {
                        SKTypeface = SKTypeface.FromFamilyName("Microsoft YaHei")
                    },
                    LabelsPaint = new SolidColorPaint(SKColors.Blue),
                    TextSize = 13,
                }
            ];

        public Axis[] YAxesDayTimeYield { get; set; }
            =
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

        #endregion


        public LiveChartsTestViewModel()
        {
            if (LiveChartsModel == null)
            {
                LiveChartsModel = new LiveChartsModel();
            }
            else
            {
            }

            Log = new LogControl<LiveChartsTestViewModel>(SnackbarService);


            DayTimeYieldSeries =
            [
                new ColumnSeries<ObservableValue>
                {
                    Values = LiveChartsModel.Oks,
                    Fill = green,
                    Stroke = null,
                    MaxBarWidth = double.MaxValue,
                    IgnoresBarPosition = true
                },
                new ColumnSeries<ObservableValue>
                {
                    Values = LiveChartsModel.Ngs,
                    Fill = red,
                    Stroke = null,
                    MaxBarWidth = 30,
                    IgnoresBarPosition = true
                },
                new LineSeries<ObservableValue>
                {
                    Values = LiveChartsModel.All,
                    Fill = null,
                    GeometrySize = 0
                }
            ];

            OkTotalPieSeries =
            [
                new PieSeries<ObservableValue>
                {
                    Name = "OK",
                    Values = [LiveChartsModel.Ok],
                    Stroke = null,
                    Fill = green,
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
                    DataLabelsPaint = new SolidColorPaint(new SKColor(0,0,255))
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
            var setLiveChartsParamWindow = new SetLiveChartsParamWindow();

            var showDialog = setLiveChartsParamWindow.ShowDialog();
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
            JsonTool<LiveChartsModel>.Save(LiveChartsModel);
        }

        #endregion
    }
}