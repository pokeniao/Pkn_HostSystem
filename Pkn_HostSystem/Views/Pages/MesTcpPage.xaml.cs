using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Pojo.Page.HomePage;
using Pkn_HostSystem.Pojo.Page.MESTcp;
using Wpf.Ui.Controls;
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
            if (viewModel.MesTcpModel.DynNetList.Count > 0)
            {
                DynNameListBox.SelectedValue = viewModel.MesTcpModel.DynNetList[0];
            }
        }


        /// <summary>
        /// 选择方式修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DynNameListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadMesDynContent? mesTcpPojo = DynNameListBox.SelectedItem as LoadMesDynContent;

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
            LoadMesDynContent? mesTcpPojo = DynNameListBox.SelectedItem as LoadMesDynContent;

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
                var selectedItem = comboBox.SelectedItem as NetWork;

                string netMethod = selectedItem.NetworkDetailed.NetMethod;

                ChangeParam(netMethod);
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
            DynCondition? item = DynConditionDataGrid.SelectedItem as DynCondition;
            string connectName = item.ConnectName;
            string netMethod = null;
            foreach (NetWork netWorkPoJo in viewModel.MesTcpModel.NetWorkList)
            {
                if (netWorkPoJo.NetworkDetailed.Name == connectName)
                {
                    netMethod = netWorkPoJo.NetworkDetailed.NetMethod;
                }
            }
            ChangeParam(netMethod);
        }

        public void ChangeParam(string netMethod)
        {
            switch (netMethod)
            {
                case "ModbusTcp":
                    viewModel.MesTcpModel.MethodName = ["读线圈", "读寄存器"];
                    viewModel.MesTcpModel.BitNet =
                    [
                        "单寄存器(无符号)", "单寄存器(有符号)",
                        "双寄存器;无符号;BigEndian", "双寄存器;无符号;LittleEndian", "双寄存器;无符号;WordSwap", "双寄存器;无符号;ByteSwap",
                        "双寄存器;有符号;BigEndian", "双寄存器;有符号;LittleEndian", "双寄存器;有符号;WordSwap", "双寄存器;有符号;ByteSwap",
                        "32位浮点数;BigEndian", "32位浮点数;LittleEndian", "32位浮点数;WordSwap", "32位浮点数;ByteSwap", "ASCII字符串"
                    ];
                    break;
                case "ModbusRtu":
                    viewModel.MesTcpModel.MethodName = ["读线圈", "读寄存器"];
                    viewModel.MesTcpModel.BitNet =
                    [
                        "单寄存器(无符号)", "单寄存器(有符号)",
                        "双寄存器;无符号;BigEndian", "双寄存器;无符号;LittleEndian", "双寄存器;无符号;WordSwap", "双寄存器;无符号;ByteSwap",
                        "双寄存器;有符号;BigEndian", "双寄存器;有符号;LittleEndian", "双寄存器;有符号;WordSwap", "双寄存器;有符号;ByteSwap",
                        "32位浮点数;BigEndian", "32位浮点数;LittleEndian", "32位浮点数;WordSwap", "32位浮点数;ByteSwap", "ASCII字符串"
                    ];
                    break;
                case "Tcp客户端":
                    viewModel.MesTcpModel.MethodName = ["Socket返回"];
                    break;
                case "Tcp服务器":
                    viewModel.MesTcpModel.MethodName = ["Socket返回"];
                    break;
                case "基恩士上位链路通讯":
                    viewModel.MesTcpModel.MethodName = ["读DM寄存器", "读R线圈状态"];
                    viewModel.MesTcpModel.BitNet =
                        ["单寄存器(无符号)", "单寄存器(有符号)", "双寄存器(无符号)", "双寄存器(有符号)", "32位浮点数", "ASCII字符串"];
                    break;
            }
        }
        #endregion

        /// <summary>
        /// 点击编辑Switch按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            bool showSwitch = viewModel.MesTcpModel.ShowSwitchSet = true;
            bool showVerify = viewModel.MesTcpModel.VeritySet;
            if (showSwitch == true && showVerify == false)
            {
                DynVerifyDataGrid.Width = 300;
                DynSwitchDataGrid.Width = 500;
            }
            else
            {
                DynVerifyDataGrid.Width = 300;
                DynSwitchDataGrid.Width = 300;
            }
            DynCondition? item = DynConditionDataGrid.SelectedItem as DynCondition;

            viewModel.MesTcpModel.SwitchList = item.SwitchList;
        }
        /// <summary>
        /// 点击编辑Switch按钮关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
           bool showSwitch = viewModel.MesTcpModel.ShowSwitchSet = false;
           bool showVerify = viewModel.MesTcpModel.VeritySet;
            if (showSwitch == false && showVerify ==true)
            {
                DynVerifyDataGrid.Width = 500;
                DynSwitchDataGrid.Width = 300;
            }
            else
            {
                DynVerifyDataGrid.Width = 300;
                DynSwitchDataGrid.Width = 300;
            }
        }
        /// <summary>
        /// 点击编辑Switch按钮关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick3(object sender, RoutedEventArgs e)
        {
            bool showVerify = viewModel.MesTcpModel.VeritySet = false;
            bool showSwitch = viewModel.MesTcpModel.ShowSwitchSet;
            if (showSwitch == true && showVerify == false)
            {
                DynVerifyDataGrid.Width = 300;
                DynSwitchDataGrid.Width = 500;
            }
            else
            {
                DynVerifyDataGrid.Width = 300;
                DynSwitchDataGrid.Width = 300;
            }
        }

        /// <summary>
        /// 套接字的校验
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetVerify(object sender, RoutedEventArgs e)
        {

            bool showSwitch = viewModel.MesTcpModel.ShowSwitchSet = false;
            bool showVerify = viewModel.MesTcpModel.VeritySet = true;
            if (showSwitch == false && showVerify == true)
            {
                DynVerifyDataGrid.Width = 500;
                DynSwitchDataGrid.Width = 300;
            }
            else
            {
                DynVerifyDataGrid.Width = 300;
                DynSwitchDataGrid.Width = 300;
            }
            DynCondition? item = DynConditionDataGrid.SelectedItem as DynCondition;

            viewModel.MesTcpModel.VerifyList = item.VerifyList;
        }
    }
}