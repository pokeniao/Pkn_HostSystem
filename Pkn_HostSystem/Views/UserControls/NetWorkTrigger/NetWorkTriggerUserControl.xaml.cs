using Pkn_HostSystem.Models.Core;
using System.Windows;
using System.Windows.Controls;

namespace Pkn_HostSystem.Views.UserControls.NetWorkTrigger
{
    /// <summary>
    /// NetWorkTriggerUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class NetWorkTriggerUserControl : UserControl
    {
        public NetWorkTriggerUserControl()
        {
            InitializeComponent();
        }

        public NetWorkTriggerModel NetWorkTriggerModel
        {
            get => (NetWorkTriggerModel)GetValue(NetWorkTriggerModelProperty);
            set => SetValue(NetWorkTriggerModelProperty, value);
        }
        public static readonly DependencyProperty NetWorkTriggerModelProperty = DependencyProperty.Register(
            nameof(NetWorkTriggerModel),
            typeof(NetWorkTriggerModel),
            typeof(NetWorkTriggerUserControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
            );
        /// <summary>
        /// 通讯选择改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
