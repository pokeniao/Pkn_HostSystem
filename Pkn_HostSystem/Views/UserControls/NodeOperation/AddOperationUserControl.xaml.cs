using Pkn_HostSystem.Models.Core;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Pkn_HostSystem.Views.UserControls.NodeOperation
{
    /// <summary>
    /// AddOperationUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class AddOperationUserControl : UserControl
    {
        public AddOperationUserControl()
        {
            InitializeComponent();
        }


        public ObservableCollection<OperationParam> InputOperationParams
        {
            get => (ObservableCollection<OperationParam>)GetValue(InputOperationParamsProperty);
            set => SetValue(InputOperationParamsProperty, value);
        }

        public static readonly DependencyProperty InputOperationParamsProperty =
            DependencyProperty.Register(
                nameof(InputOperationParams),
                typeof(OperationParam),
                typeof(AddOperationUserControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public ObservableCollection<OperationParam> OutputOperationParams
        {
            get => (ObservableCollection<OperationParam>)GetValue(OutputOperationParamsProperty);
            set => SetValue(OutputOperationParamsProperty, value);
        }

        public static readonly DependencyProperty OutputOperationParamsProperty =
            DependencyProperty.Register(
                nameof(OutputOperationParams),
                typeof(OperationParam),
                typeof(AddOperationUserControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    }
}
