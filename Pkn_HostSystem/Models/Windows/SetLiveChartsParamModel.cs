using CommunityToolkit.Mvvm.ComponentModel;

namespace Pkn_HostSystem.Models.Windows
{
    public partial class SetLiveChartsParamModel : ObservableObject
    {
        /// <summary>
        /// 产量统计,动态获取的名
        /// </summary>
        [ObservableProperty] private string dayProductionDynName;

        /// <summary>
        /// 运行LiveCharts进行数据展示按钮
        /// </summary>
        [ObservableProperty] private string runLiveChartsButton ="启用";
    }
}