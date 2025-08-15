using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace Pkn_HostSystem.Models.Windows
{
    public partial class SetLiveChartsParamModel : ObservableObject
    {
        [ObservableProperty] [NotifyPropertyChangedFor(nameof(showxAxesDayTimeLabelsRunStopString))]
        private string xDayTimeMethod;

        [ObservableProperty] private string yDayTimeMethod;


        [ObservableProperty] private string xAxesDayTimeLabelsYieldString;

        [ObservableProperty] [NotifyPropertyChangedFor(nameof(showxAxesOeeLabelsYieldString))]
        private string xOeeMethod;

        [ObservableProperty] private string yOeeMethod;
        [ObservableProperty] private string xAxesOeeLabelsYieldString;

        public Visibility showxAxesDayTimeLabelsRunStopString =>
            XDayTimeMethod == "常量设置" ? Visibility.Visible : Visibility.Collapsed;

        public Visibility showxAxesOeeLabelsYieldString =>
            XOeeMethod == "常量设置" ? Visibility.Visible : Visibility.Collapsed;
    }
}