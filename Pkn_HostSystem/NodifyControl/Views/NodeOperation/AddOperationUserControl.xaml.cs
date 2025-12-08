using Pkn_HostSystem.NodifyControl.OperationModels.Models.Core;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Pkn_HostSystem.NodifyControl.Views.NodeOperation
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


        public ObservableCollection<OperationModel> InputOperationParams
        {
            get => (ObservableCollection<OperationModel>)GetValue(InputOperationParamsProperty);
            set => SetValue(InputOperationParamsProperty, value);
        }

        public static readonly DependencyProperty InputOperationParamsProperty =
            DependencyProperty.Register(
                nameof(InputOperationParams),
                typeof(OperationModel),
                typeof(AddOperationUserControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public ObservableCollection<OperationModel> OutputOperationParams
        {
            get => (ObservableCollection<OperationModel>)GetValue(OutputOperationParamsProperty);
            set => SetValue(OutputOperationParamsProperty, value);
        }

        public static readonly DependencyProperty OutputOperationParamsProperty =
            DependencyProperty.Register(
                nameof(OutputOperationParams),
                typeof(OperationModel),
                typeof(AddOperationUserControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    }
}
