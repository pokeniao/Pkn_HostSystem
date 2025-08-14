using CommunityToolkit.Mvvm.ComponentModel;

namespace Pkn_HostSystem.Models.Windows
{
    public partial class SetLiveChartsParamModel : ObservableObject
    {

        [ObservableProperty] private string xDayTimeMethod;
        [ObservableProperty] private string yDayTimeMethod;


        [ObservableProperty] private string xAxesDayTimeLabelsYieldString;

        [ObservableProperty] private string xOeeMethod;
        [ObservableProperty] private string yOeeMethod;
        [ObservableProperty] private string xAxesOeeLabelsYieldString;
        
    }
}