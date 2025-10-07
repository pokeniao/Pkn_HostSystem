using DynamicData;
using DynamicData.Binding;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.Node.Base;
using Pkn_HostSystem.NodifyControl.Node.Connector;
using Pkn_HostSystem.Static;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Pkn_HostSystem.Views.UserControls.OperationDataGrid
{
    /// <summary>
    /// PknOperationDataGrid.xaml 的交互逻辑
    /// </summary>
    public partial class PknOperationDataGrid : UserControl
    {
        public PknOperationDataGrid()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 参数
        /// </summary>
        public ObservableCollection<OperationParam> NeedItemSource
        {
            get => (ObservableCollection<OperationParam>)GetValue(NeedItemSourceProperty);
            set => SetValue(NeedItemSourceProperty, value);
        }

        public static readonly DependencyProperty NeedItemSourceProperty =
            DependencyProperty.Register(
                nameof(NeedItemSource),
                typeof(ObservableCollection<OperationParam>),
                typeof(PknOperationDataGrid),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public ObservableCollectionExtended<OperationParam> InputParams
        {
            get => (ObservableCollectionExtended<OperationParam>)GetValue(InputParamsProperty);
            set => SetValue(InputParamsProperty, value);
        }

        public static readonly DependencyProperty InputParamsProperty =
            DependencyProperty.Register(
                nameof(InputParams),
                typeof(ObservableCollectionExtended<OperationParam>),
                typeof(PknOperationDataGrid),
                new FrameworkPropertyMetadata(new ObservableCollectionExtended<OperationParam>()));


        /// <summary>
        /// 传入DataContent
        /// </summary>
        public object Inputs
        {
            get => GetValue(InputsProperty);
            set => SetValue(InputsProperty, value);
        }

        public static readonly DependencyProperty InputsProperty =
            DependencyProperty.Register(
                nameof(Inputs),
                typeof(object),
                typeof(PknOperationDataGrid),
                new FrameworkPropertyMetadata(null));


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            OperationParam? dataGridOperationParam = DataGrid.SelectedValue as OperationParam;

            if (dataGridOperationParam == null) return;

            string name = dataGridOperationParam.Name;
            if (name != null)
            {
                if (dataGridOperationParam.NoDelete)
                {
                    return;
                }

                bool remove = NeedItemSource.Remove(dataGridOperationParam);
            }
        }


        /// <summary>
        /// 下拉 展示所有动态获取对象
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_OnDropDownOpened(object? sender, EventArgs e)
        {
            //获取全部接入
            var myConnectors = Inputs as ObservableCollection<MyConnector>;
            InputParams.Clear();
            if (Inputs == null)
            {
                InputParams.Clear();
                return;
            }
            foreach (var connector in myConnectors)
            {
                if (connector == null)
                {
                    continue;
                }
                List<ObservableCollection<OperationParam>> myConnectorInputValue = connector.InputValue;
                foreach (var observableCollection in myConnectorInputValue)
                {
                    InputParams.AddRange(observableCollection);
                }
            }
        }
    }
}
