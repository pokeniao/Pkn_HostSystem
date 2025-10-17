using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.ParamOperationModel;
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


        public ObservableCollection<OperationParamModel> InputOperationParams
        {
            get => (ObservableCollection<OperationParamModel>)GetValue(InputOperationParamsProperty);
            set => SetValue(InputOperationParamsProperty, value);
        }

        public static readonly DependencyProperty InputOperationParamsProperty =
            DependencyProperty.Register(
                nameof(InputOperationParams),
                typeof(OperationParamModel),
                typeof(AddOperationUserControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public ObservableCollection<OperationParamModel> OutputOperationParams
        {
            get => (ObservableCollection<OperationParamModel>)GetValue(OutputOperationParamsProperty);
            set => SetValue(OutputOperationParamsProperty, value);
        }

        public static readonly DependencyProperty OutputOperationParamsProperty =
            DependencyProperty.Register(
                nameof(OutputOperationParams),
                typeof(OperationParamModel),
                typeof(AddOperationUserControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    }
}
