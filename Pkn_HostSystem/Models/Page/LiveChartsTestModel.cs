using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using Pkn_HostSystem.Static;
using SkiaSharp;

namespace Pkn_HostSystem.Models.Page
{
    public partial class LiveChartsTestModel :ObservableObject
    {
       [ObservableProperty] private ObservableValue ok = new ObservableValue(6);
       [ObservableProperty] private ObservableValue ng = new ObservableValue(2);
       [ObservableProperty] private ObservableValue runTime = new ObservableValue(12960);
       [ObservableProperty] private ObservableValue stopTime = new ObservableValue(64800); //一天一共86400秒
       [ObservableProperty] private ObservableValue errorTime = new ObservableValue(8640);



    }
}