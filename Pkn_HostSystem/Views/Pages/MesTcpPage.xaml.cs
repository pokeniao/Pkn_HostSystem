using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Pojo.Page.HomePage;
using Pkn_HostSystem.Pojo.Page.MESTcp;
using MesTcpViewModel = Pkn_HostSystem.ViewModels.Page.MesTcpViewModel;

namespace Pkn_HostSystem.Views.Pages
{
    /// <summary>
    /// MesTcpPage.xaml 的交互逻辑
    /// </summary>
    public partial class MesTcpPage : Page
    {
        public MesTcpViewModel viewModel { get; set; } = Ioc.Default.GetRequiredService<MesTcpViewModel>();

        public MesTcpPage()
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.setSnackbarPresenter(SnackbarPresenter);
        }


        /// <summary>
        /// 选择方式修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DynNameListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MesTcpPojo? mesTcpPojo = DynNameListBox.SelectedItem as MesTcpPojo;

            if (mesTcpPojo != null)
            {
                //绑定选中行的数据到页面
                viewModel.MesTcpModel.DynConditionItemList = mesTcpPojo.DynCondition;
                viewModel.MesTcpModel.Message = mesTcpPojo.Message;
            }
        }

        //每当文本发生修改
        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            MesTcpPojo? mesTcpPojo = DynNameListBox.SelectedItem as MesTcpPojo;

            if (mesTcpPojo != null)
            {
                mesTcpPojo.Message = viewModel.MesTcpModel.Message;
            }
        }

        #region 请求方式Col 的更新

        //当连接对象发生改变
        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox? comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                var selectedItem = comboBox.SelectedItem as NetWorkPoJo;

                string netMethod = selectedItem.ConnectPojo.NetMethod;

                switch (netMethod)
                {
                    case "ModbusTcp":
                        viewModel.MesTcpModel.MethodName = ["读线圈", "读寄存器"];
                        break;
                    case "ModbusRtu":
                        viewModel.MesTcpModel.MethodName = ["读线圈", "读寄存器"];
                        break;
                    case "Tcp客户端":
                        viewModel.MesTcpModel.MethodName = ["Socket返回"];
                        break;
                    case "Tcp服务器":
                        viewModel.MesTcpModel.MethodName = ["Socket返回"];
                        viewModel.MesTcpModel.TcpServerConnectionClint =
                            new ObservableCollection<string>(selectedItem.WatsonTcpTool.GetConnectedClients());
                        break;
                }
            }
        }

        /// <summary>
        /// 当 请求方式 的下拉栏打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ComboBox_OnDropDownOpened(object? sender, EventArgs e)
        {
            DynConditionItem? item = DynConditionDataGrid.SelectedItem as DynConditionItem;
            string connectName = item.ConnectName;
            string netMethod = null;
            foreach (NetWorkPoJo netWorkPoJo in viewModel.MesTcpModel.NetWorkList)
            {
                if (netWorkPoJo.ConnectPojo.Name == connectName)
                {
                    netMethod = netWorkPoJo.ConnectPojo.NetMethod;
                }
            }
            switch (netMethod)
            {
                case "ModbusTcp":
                    viewModel.MesTcpModel.MethodName = ["读线圈", "读寄存器"];
                    break;
                case "ModbusRtu":
                    viewModel.MesTcpModel.MethodName = ["读线圈", "读寄存器"];
                    break;
                case "Tcp客户端":
                    viewModel.MesTcpModel.MethodName = ["Socket返回"];
                    break;
                case "Tcp服务器":
                    viewModel.MesTcpModel.MethodName = ["Socket返回"];
                    break;
            }
        }

        #endregion
    }
}