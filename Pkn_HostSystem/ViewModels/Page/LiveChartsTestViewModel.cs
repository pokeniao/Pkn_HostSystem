using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Page
{
    public partial class LiveChartsTestViewModel :ObservableRecipient
    {
        public LiveChartsTestModel LiveChartsTestModel { get; set; } = AppJsonTool<LiveChartsTestModel>.Load();
        public LogBase<LiveChartsTestViewModel> Log;
        public SnackbarService SnackbarService = new SnackbarService();

        #region 树状图
        public ISeries[] Series { get; set; } = [
            new ColumnSeries<int>(3, 4, 2),
            new ColumnSeries<int>(4, 2, 6),
            new ColumnSeries<double, DiamondGeometry>(4, 3, 4)
        ];
        #endregion

        #region 世界热地图
        /// <summary>
        /// 世界热地图
        /// </summary>
        public HeatLandSeries[] Series2 { get; set; } = [new HeatLandSeries()
        {
            Lands = new HeatLand[]
            {
                new() { Name = "bra", Value = 13 },
                new() { Name = "mex", Value = 10 },
                new() { Name = "usa", Value = 15 },
                new() { Name = "can", Value = 8 },
                new() { Name = "ind", Value = 12 },
                new() { Name = "deu", Value = 13 },
                new() { Name= "jpn", Value = 15 },
                new() { Name = "chn", Value = 14 },
                new() { Name = "rus", Value = 11 },
                new() { Name = "fra", Value = 8 },
                new() { Name = "esp", Value = 7 },
                new() { Name = "kor", Value = 10 },
                new() { Name = "zaf", Value = 12 },
                new() { Name = "are", Value = 13 }
            }
        }];


        #endregion

        #region 饼图 , 分散型
        public IEnumerable<ISeries> Series3 { get; set; } =
            new[] { 6, 5, 4, 3, 2 }.AsPieSeries((value, series) =>
            {
                // pushes out the slice with the value of 6 to 30 pixels.
                if (value != 6) return;

                series.Pushout = 30;
            });
        #endregion

        #region 波动图
        private readonly Random _random = new();
        private readonly List<DateTimePoint> _values = [];
        private DateTimeAxis _customAxis ;

        public ObservableCollection<ISeries> Series4 { get; set; }

        public Axis[] XAxes { get; set; }

        public object Sync { get; } = new object();

        public bool IsReading { get; set; } = true;
        public void CartesianChartInit()
        {
            Series4 = [
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

        #region 饼图,普通
        private static readonly SKColor[] s_colors = [
            new SKColor(179, 229, 252),
            new SKColor(1, 87, 155)
            // ...

            // you can add as many colors as you require to build the gradient
            // by default all the distance between each color is equal
            // use the colorPos parameter in the constructor of the RadialGradientPaint class
            // to specify the distance between each color
        ];
        public ISeries[] Series5 { get; set; } = [
            new PieSeries<int>
            {
                Name = "Maria",
                Values = [7],
                Stroke = null,
                Fill = new RadialGradientPaint(s_colors),
                Pushout = 10,
                OuterRadiusOffset = 20
            },
            new PieSeries<int>
            {
                Name = "Charles",
                Values = [3],
                Stroke = null,
                Fill = new RadialGradientPaint(new SKColor(255, 205, 210), new SKColor(183, 28, 28))
            }
        ];

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

            CartesianChartInit();
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