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
using SkiaSharp;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Page
{
    public partial class LiveChartsTestViewModel : ObservableRecipient
    {
        public LiveChartsTestModel LiveChartsTestModel { get; set; } = AppJsonTool<LiveChartsTestModel>.Load();
        public LogBase<LiveChartsTestViewModel> Log;
        public SnackbarService SnackbarService = new SnackbarService();

        #region 饼图--良率统计

        public ISeries[] OkTotalPieSeries { get; set; }


        public ISeries[] TimePieSeries { get; set; }

        public LabelVisual TotalTitlePie { get; set; } =
            new()
            {
                Text = "良率产量统计",
                TextSize = 15,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint
                {
                    Color = GlobalMannager.ThemeSkColor,
                    SKTypeface = SKTypeface.FromFamilyName("Microsoft YaHei")
                }
            };

        public LabelVisual TotalTitlePie2 { get; set; } =
            new()
            {
                Text = "耗时统计",
                TextSize = 15,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint
                {
                    Color = GlobalMannager.ThemeSkColor,
                    SKTypeface = SKTypeface.FromFamilyName("Microsoft YaHei")
                }
            };



        #endregion

        #region 实时线图
        private readonly Random _random = new();
        private readonly List<DateTimePoint> _values = [];
        private DateTimeAxis _customAxis ;

        public ObservableCollection<ISeries> Series { get; set; }

        public Axis[] XAxes { get; set; }

        public object Sync { get; } = new object();

        public bool IsReading { get; set; } = true;


        public void HeartBeat()
        {
            Series = [
                new LineSeries<DateTimePoint>
                {
                    Values = _values,
                    Fill = null,
                    GeometryFill = null,
                    GeometryStroke = null
                }
            ];

            _customAxis = new DateTimeAxis(TimeSpan.FromSeconds(1), Formatter)
            {
                CustomSeparators = GetSeparators(),
                AnimationsSpeed = TimeSpan.FromMilliseconds(0),
                SeparatorsPaint = new SolidColorPaint(SKColors.Black.WithAlpha(100))
            };

            XAxes = [_customAxis];

            _ = ReadData();
        }

        private async Task ReadData()
        {
            // to keep this sample simple, we run the next infinite loop 
            // in a real application you should stop the loop/task when the view is disposed 

            while (IsReading)
            {
                await Task.Delay(100);

                // Because we are updating the chart from a different thread 
                // we need to use a lock to access the chart data. 
                // this is not necessary if your changes are made on the UI thread. 
                lock (Sync)
                {
                    _values.Add(new DateTimePoint(DateTime.Now, _random.Next(0, 10)));
                    if (_values.Count > 250) _values.RemoveAt(0);

                    // we need to update the separators every time we add a new point 
                    _customAxis.CustomSeparators = GetSeparators();
                }
            }
        }

        private static double[] GetSeparators()
        {
            var now = DateTime.Now;

            return
            [
                now.AddSeconds(-25).Ticks,
                now.AddSeconds(-20).Ticks,
                now.AddSeconds(-15).Ticks,
                now.AddSeconds(-10).Ticks,
                now.AddSeconds(-5).Ticks,
                now.Ticks
            ];
        }

        private static string Formatter(DateTime date)
        {
            var secsAgo = (DateTime.Now - date).TotalSeconds;

            return secsAgo < 1
                ? "now"
                : $"{secsAgo:N0}s ago";
        }
        #endregion

        #region 柱状图

        public ISeries[] DayTimeYieldSeries { get; set; } = [
            new ColumnSeries<int>
            {   
                Values = [6, 3, 5, 7, 3, 4, 6, 3,70,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],

                Fill = new RadialGradientPaint([new SKColor(0, 240, 0),new SKColor(100,200,50)]),
                Stroke = null,
                MaxBarWidth = double.MaxValue,
                IgnoresBarPosition = true
            },
            new ColumnSeries<int>
            {
                Values = [2, 4, 8, 9, 5, 2, 4, 7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],

                Fill = new RadialGradientPaint([new SKColor(255, 0, 0),new SKColor(200,100,100)]),
                Stroke = null,
                MaxBarWidth = 30,
                IgnoresBarPosition = true
            }, 
            new LineSeries<double>   { 
                Values = [6, 3, 5, 7, 3, 4, 6, 3, 70, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0], 
                Fill = null,
                GeometrySize = 0
            }
        ];

        public Axis[] XAxesDayTimeYield { get; set; }
            = new Axis[]
            {
                new Axis
                {
                    Name = "时间",

                    Labels = ["0:00", "1:00", "2:00", "3:00","4:00","5:00","6:00","7:00","8:00","9:00","10:00","11:00","12:00","13:00","14:00","15:00","16:00","17:00","18:00","19:00","20:00","21:00","22:00","23:00"
                    ],
                    NamePaint = new SolidColorPaint(GlobalMannager.ThemeSkColor)
                    {
                        SKTypeface = SKTypeface.FromFamilyName("Microsoft YaHei")
                    },
                    LabelsPaint = new SolidColorPaint(SKColors.Blue),
                    TextSize = 15,
                    
                    // SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) { StrokeThickness = 2 }
                }
            };

        public Axis[] YAxesDayTimeYield { get; set; }
            = new Axis[]
            {
                new Axis
                {
                    Name = "产量",
                    NamePaint = new SolidColorPaint(GlobalMannager.ThemeSkColor) 
                    {
                        SKTypeface = SKTypeface.FromFamilyName("Microsoft YaHei")
                    },

                    LabelsPaint = new SolidColorPaint(SKColors.Green),
                    TextSize = 15,

                    // SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray)
                    // {
                    //     StrokeThickness = 2,
                    //     PathEffect = new DashEffect(new float[] { 3, 3 })
                    // }
                }
            };

        #endregion


        #region CT图
        public IEnumerable<ISeries> CTSeries { get; set; } =
            GaugeGenerator.BuildSolidGauge(
                new GaugeItem(50, series => SetStyle("Station1", series)),
                new GaugeItem(80, series => SetStyle("Station2", series)),
                new GaugeItem(95, series => SetStyle("Station3", series)),
                new GaugeItem(GaugeItem.Background, series =>
                {
                    series.Fill = null;
                }));
        public static void SetStyle(string name, PieSeries<ObservableValue> series)
        {
            series.Name = name;
            series.DataLabelsSize = 20;
            series.DataLabelsPosition = PolarLabelsPosition.End;
            series.DataLabelsFormatter =
                point => point.Coordinate.PrimaryValue.ToString();
            series.InnerRadius = 20;
            series.MaxRadialColumnWidth = 5;
        }

        #endregion

        public LiveChartsTestViewModel()
        {
            if (LiveChartsTestModel == null)
            {
                LiveChartsTestModel = new LiveChartsTestModel();
            }
            else
            {
            }

            Log = new LogBase<LiveChartsTestViewModel>(SnackbarService);

            OkTotalPieSeries =
            [
                new PieSeries<ObservableValue>
                {
                    Name = "OK",
                    Values = [LiveChartsTestModel.Ok],
                    Stroke = null,
                    Fill = new RadialGradientPaint([new SKColor(0, 240, 0),new SKColor(100,200,50)]),
                    DataLabelsPaint = new SolidColorPaint(GlobalMannager.ThemeSkColor), //页面上显示数据
                },
                new PieSeries<ObservableValue>
                {
                    Name = "NG",
                    Values = [LiveChartsTestModel.Ng],
                    Stroke = null,
                    Fill = new RadialGradientPaint([new SKColor(255, 0, 0),new SKColor(200,100,50)]),
                    DataLabelsPaint = new SolidColorPaint(GlobalMannager.ThemeSkColor), //页面上显示数据
                    Pushout = 10,
                    OuterRadiusOffset = 20
                }
            ];

            TimePieSeries =
            [
                new PieSeries<ObservableValue>
                {
                    Values = [LiveChartsTestModel.RunTime],
                    Fill = new RadialGradientPaint([new SKColor(0, 240, 0),new SKColor(100,200,100)]),
                    DataLabelsPaint = new SolidColorPaint(GlobalMannager.ThemeSkColor)
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
                            var a = $"运行时间{Environment.NewLine}{pv}/{sv.Total}{Environment.NewLine}{sv.Share:P2}";
                            return a;
                        }
                },
                new PieSeries<ObservableValue>
                {
                    Values = [LiveChartsTestModel.StopTime],
                    Fill = new RadialGradientPaint([new SKColor(0, 0, 245),new SKColor(100,100,240)]),
                    DataLabelsPaint = new SolidColorPaint(GlobalMannager.ThemeSkColor)
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
                            var a = $"待机时间{Environment.NewLine}{pv}/{sv.Total}{Environment.NewLine}{sv.Share:P2}";
                            return a;
                        }
                },
                new PieSeries<ObservableValue>
                {
                    Values = [LiveChartsTestModel.ErrorTime],
                    Fill = new RadialGradientPaint([new SKColor(255, 0, 0),new SKColor(200,100,100)]),
                    DataLabelsPaint = new SolidColorPaint(GlobalMannager.ThemeSkColor)
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

                            var a = $"报警时间{Environment.NewLine}{pv}/{sv.Total}{Environment.NewLine}{sv.Share:P2}";
                            return a;
                        }
                }
            ];

            HeartBeat();
        }


        /// <summary>
        /// 刷新页面
        /// </summary>
        /// <param name="page"></param>
        [RelayCommand]
        public void Refresh(LiveChartsTestPage page)
        {
            TotalTitlePie = new()
            {
                Text = "良率产量统计",
                TextSize = 15,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint
                {
                    Color = GlobalMannager.ThemeSkColor,
                    SKTypeface = SKTypeface.FromFamilyName("Microsoft YaHei")
                }
            };

            TotalTitlePie2 =
                new()
                {
                    Text = "耗时统计",
                    TextSize = 15,
                    Padding = new LiveChartsCore.Drawing.Padding(15),
                    Paint = new SolidColorPaint
                    {
                        Color = GlobalMannager.ThemeSkColor,
                        SKTypeface = SKTypeface.FromFamilyName("Microsoft YaHei")
                    }
                };
            foreach (var series in OkTotalPieSeries)
            {
                series.DataLabelsPaint = new SolidColorPaint(GlobalMannager.ThemeSkColor);
            }
        }

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
            AppJsonTool<LiveChartsTestModel>.Save(LiveChartsTestModel);
        }

        #endregion
    }
}