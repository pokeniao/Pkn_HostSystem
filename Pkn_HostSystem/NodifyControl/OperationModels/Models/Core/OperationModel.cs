using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.OperationModels.Models.Core
{
    /// <summary>
    /// 参数配置中的传递输入输出
    /// </summary>
    public partial class OperationModel : ObservableObject
    {
        //姓名
        [ObservableProperty] private string name;

        //方式
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowStaticParamVisibility))]
        [NotifyPropertyChangedFor(nameof(ShowDynamicParamVisibility))]
        private string paramMethod = "常量";
        /// <summary>
        /// 常量值
        /// </summary>
        [ObservableProperty] private string paramValue;

        /// <summary>
        /// 动态名
        /// </summary>
        [ObservableProperty] private string dynName;
        /// <summary>
        /// 动态值
        /// </summary>
        [ObservableProperty] private OperationModel _dyn;


        partial void OnDynChanged(OperationModel value)
        {
            //用于显示
            DynName = Dyn?.Name;
        }

        /// <summary>
        /// 禁止修改
        /// </summary>
        [ObservableProperty] private bool isEnable = true;
        /// <summary>
        /// 禁止删除
        /// </summary>
        public bool NoDelete { get; set; } = false;

        public Visibility ShowStaticParamVisibility => ParamMethod == "常量" ? Visibility.Visible : Visibility.Collapsed;

        public Visibility ShowDynamicParamVisibility => ParamMethod == "动态获取" ? Visibility.Visible : Visibility.Collapsed;
    }
}