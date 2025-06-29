﻿using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Windows;
using System.Windows;
using System.Windows.Controls;
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

        #region 请求类型
        /// <summary>
        /// 当 请求类型进行选择时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetMessageChange(object sender, SelectionChangedEventArgs e)
        {
            ComboBox? comboBox = sender as ComboBox;
            string selectedValue = comboBox.SelectedValue as string;
            DynCondition? selectedItem = DynConditionDataGrid.SelectedItem as DynCondition;
            //更具不同的请求方式,修改方法名
            switch (selectedValue)
            {
                case "HTTP":
                    selectedItem.MethodName = "Http";
                    break;
                case "自定义":
                    selectedItem.MethodName = "自定义(无法填写)";
                    break;
                case "通讯":
                    //得到当前通讯方式
                    string netMethod = selectedItem.NetWork?.NetworkDetailed.NetMethod;
                    //切换选项
                    ChangeParam(selectedItem);
                    SetDefault(selectedItem);
                    break;
                default:
                    selectedItem.MethodName = "";
                    break;
            }
        }
        #endregion

        #region 通讯名

        /// <summary>
        /// 通讯名下拉修改 - 通讯模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DynCondition? item = DynConditionDataGrid.SelectedItem as DynCondition;
            ChangeParam(item);
            SetDefault(item);
        }

        /// <summary>
        /// 通讯名下拉修改 - HTTP模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HttpSelectChange(object sender, SelectionChangedEventArgs e)
        {
            DynCondition? item = DynConditionDataGrid.SelectedItem as DynCondition;
            item.MethodName = "Http";
        }

        #endregion

        #region 请求方式
        /// <summary>
        /// 请求方式下拉修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ComboBox_OnDropDownOpened(object? sender, EventArgs e)
        {
            DynCondition? item = DynConditionDataGrid.SelectedItem as DynCondition;
            ChangeParam(item);
        }

        #endregion

        /// <summary>
        /// 用于判断当前通讯对象是什么类型
        /// </summary>
        /// <param name="netMethod"></param>
        private void ChangeParam(DynCondition dynCondition)
        {
            //得到当前通讯方式
            string netMethod = dynCondition.NetWork?.NetworkDetailed.NetMethod;
            switch (netMethod)
            {
                case "ModbusTcp":
                    viewModel.MesTcpModel.MethodName = ["读线圈", "读寄存器"];
                    viewModel.MesTcpModel.BitNet =
                    [
                        "单寄存器(无符号)", "单寄存器(有符号)",
                        "双寄存器;无符号;BigEndian", "双寄存器;无符号;LittleEndian", "双寄存器;无符号;WordSwap", "双寄存器;无符号;ByteSwap",
                        "双寄存器;有符号;BigEndian", "双寄存器;有符号;LittleEndian", "双寄存器;有符号;WordSwap", "双寄存器;有符号;ByteSwap",
                        "32位浮点数;BigEndian", "32位浮点数;LittleEndian", "32位浮点数;WordSwap", "32位浮点数;ByteSwap", "ASCII字符串(高低位)","ASCII字符串(低高位)"
                    ];
                    break;
                case "ModbusRtu":
                    viewModel.MesTcpModel.MethodName = ["读线圈", "读寄存器"];
                    viewModel.MesTcpModel.BitNet =
                    [
                        "单寄存器(无符号)", "单寄存器(有符号)",
                        "双寄存器;无符号;BigEndian", "双寄存器;无符号;LittleEndian", "双寄存器;无符号;WordSwap", "双寄存器;无符号;ByteSwap",
                        "双寄存器;有符号;BigEndian", "双寄存器;有符号;LittleEndian", "双寄存器;有符号;WordSwap", "双寄存器;有符号;ByteSwap",
                        "32位浮点数;BigEndian", "32位浮点数;LittleEndian", "32位浮点数;WordSwap", "32位浮点数;ByteSwap", "ASCII字符串(高低位)","ASCII字符串(低高位)"
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

        private void SetDefault(DynCondition dynCondition)
        {
            //得到当前通讯方式
            string netMethod = dynCondition.NetWork?.NetworkDetailed.NetMethod;
            switch (netMethod)
            {
                case "ModbusTcp":
                    dynCondition.MethodName = "读寄存器";
                    break;
                case "ModbusRtu":
                    dynCondition.MethodName = "读寄存器";
                    break;
                case "Tcp客户端":
                    dynCondition.MethodName = "Socket返回";
                    break;
                case "Tcp服务器":
                    dynCondition.MethodName = "Socket返回";
                    break;
                case "基恩士上位链路通讯":
                    dynCondition.MethodName = "读DM寄存器";
                    break;
            }
        }
        /// <summary>
        /// 点击编辑Switch按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            viewModel.MesTcpModel.ShowSwitchSet = true;
            showSetPage();

       

            DynCondition? item = DynConditionDataGrid.SelectedItem as DynCondition;

            viewModel.MesTcpModel.SwitchList = item.SwitchList;
            viewModel.MesTcpModel.SetSwitchSetName = $"Switch :{item.Name}";
        }

        /// <summary>
        /// Switch按钮关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
            viewModel.MesTcpModel.ShowSwitchSet = false;
            showSetPage();
        }

        /// <summary>
        /// 套接字的校验
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetVerify(object sender, RoutedEventArgs e)
        {
            viewModel.MesTcpModel.VeritySet = true;
            showSetPage();

            DynCondition? item = DynConditionDataGrid.SelectedItem as DynCondition;

            viewModel.MesTcpModel.VerifyList = item.VerifyList;

            viewModel.MesTcpModel.SetVeritySetName = $"校验 :{item.Name}";
        }

        /// <summary>
        /// VeritySet按钮关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick3(object sender, RoutedEventArgs e)
        {
            viewModel.MesTcpModel.VeritySet = false;
            showSetPage();
        }

        /// <summary>
        /// 点击设置HttpGetObject页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetHttpGetObject(object sender, RoutedEventArgs e)
        {
            viewModel.MesTcpModel.HttpSet = true;
            showSetPage();
            DynCondition? item = DynConditionDataGrid.SelectedItem as DynCondition;
            viewModel.MesTcpModel.HttpObjects = item.HttpObjects;

            viewModel.MesTcpModel.SetHttpObjectName = $"Json映射 :{item.Name}";
        }

        /// <summary>
        /// 点击关闭Http设置页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick4(object sender, RoutedEventArgs e)
        {
            viewModel.MesTcpModel.HttpSet = false;
            showSetPage();
        }

        public void showSetPage()
        {
            int num = 0;
            _ = viewModel.MesTcpModel.HttpSet ? num++ : num;
            _ = viewModel.MesTcpModel.VeritySet ? num++ : num;
            _ = viewModel.MesTcpModel.ShowSwitchSet ? num++ : num;
            _ = viewModel.MesTcpModel.TranspondSet ? num++ : num;

            switch (num)
            {
                case 1:
                    viewModel.MesTcpModel.SetRows = 1;
                    viewModel.MesTcpModel.SetColumns = 1;
                    break;
                case 2:
                    viewModel.MesTcpModel.SetRows = 1;
                    viewModel.MesTcpModel.SetColumns = 2;
                    break;
                case 3:
                    viewModel.MesTcpModel.SetRows = 2;
                    viewModel.MesTcpModel.SetColumns = 2;
                    break;
                case 4:
                    viewModel.MesTcpModel.SetRows = 2;
                    viewModel.MesTcpModel.SetColumns = 2;
                    break;
            }
        }


        /// <summary>
        /// 转发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResultTranspond_OnClick(object sender, RoutedEventArgs e)
        {
            
            viewModel.MesTcpModel.TranspondSet = true;
            showSetPage();
 
            DynCondition? item = DynConditionDataGrid.SelectedItem as DynCondition;

            viewModel.MesTcpModel.TranspondModbusDetailed = item.TranspondModbusDetailed;
            viewModel.MesTcpModel.TranspondSetName = $"转发设置 :{item.Name}";
        }

        /// <summary>
        /// 转发的设置关闭按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ButtonBase_OnClick5(object sender, RoutedEventArgs e)
        {
            viewModel.MesTcpModel.TranspondSet = false;
            showSetPage();
        }

        /// <summary>
        /// 窗口大小改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MesTcpPage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            DynGrid.MaxHeight = e.NewSize.Height;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            DynSwitch? value = DynSwitchDataGrid.SelectedValue as DynSwitch;

            viewModel.MesTcpModel.SwitchList?.Remove(value);
        }

    }
}