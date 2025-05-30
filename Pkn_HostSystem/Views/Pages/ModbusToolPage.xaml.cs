﻿using System.Collections.ObjectModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Models.Core;
using ModbusToolViewModel = Pkn_HostSystem.ViewModels.Page.ModbusToolViewModel;

namespace Pkn_HostSystem.Views.Pages
{
    /// <summary>
    /// ModbusToolPage.xaml 的交互逻辑
    /// </summary>
    public partial class ModbusToolPage : Page
    {
        private ModbusToolViewModel viewModel;

        public ModbusToolPage()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetRequiredService<ModbusToolViewModel>();
            viewModel = (ModbusToolViewModel)DataContext;
            viewModel.setSnackbarPresenter(SnackbarPresenter);
        }

        #region 下拉Combobox

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            viewModel.ModbusToolModel.ModbusRtu_COM = viewModel.ModbusBase.getCOM().ToList();
        }

        private void ComboBox_DropDownOpened_1(object sender, EventArgs e)
        {
            viewModel.ModbusToolModel.ModbusTcp_Ip = viewModel.ModbusBase.getIpAddress().ToList();
        }


        #endregion

        #region 数据改变需要刷新WriteDgv
        //数量改变
        private void NumberBox_ValueChanged(object sender, Wpf.Ui.Controls.NumberBoxValueChangedEventArgs args)
        {
            refreshWriteDgv();
        }
        //功能码的改变
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && viewModel != null)
            {
                //赋值选择的功能码
                viewModel.ModbusToolModel.FuntionCode_select = comboBox.SelectedItem.ToString();
            }
            refreshWriteDgv();
        }

        #endregion


        //刷新WriteDgv
        private void refreshWriteDgv()
        {
            if (viewModel != null)
                switch (viewModel.ModbusToolModel.FuntionCode_select)
                {
                    case "05写单线圈":
                        NumberBox_count.Value = 1;
                        NumberBox_count.IsEnabled = false;
                        viewModel.ModbusToolModel.WriteDvgList = WriteView<bool>();
                        break;
                    case "06写单寄存器":
                        NumberBox_count.Value = 1;
                        NumberBox_count.IsEnabled = false;
                        viewModel.ModbusToolModel.WriteDvgList = WriteView<ushort>();
                        break;
                    case "0F写多线圈":
                        NumberBox_count.IsEnabled = true;
                        viewModel.ModbusToolModel.WriteDvgList = WriteView<bool>();
                        break;
                    case "10写多寄存器":
                        NumberBox_count.IsEnabled = true;
                        viewModel.ModbusToolModel.WriteDvgList = WriteView<ushort>();
                        break;
                    default:
                        NumberBox_count.IsEnabled = true;
                        viewModel.ModbusToolModel.WriteDvgList = null;
                        break;
                }
        }

        private ObservableCollection<ModbusToolPojo<object>> WriteView<A>()
        {
            ObservableCollection<ModbusToolPojo<object>> bindingList = new ObservableCollection<ModbusToolPojo<object>>();
            if (typeof(A) == typeof(bool))
            {
                int key = viewModel.ModbusToolModel.StartAddress;
                for (int i = 0; i < viewModel.ModbusToolModel.ReadCount; i++)
                {
                    bindingList.Add(new ModbusToolPojo<object>() { address = key++, value = (object)false ,valueIsBool =true});
                }

                return bindingList;
            }
            else
            {
                int currentAddress = viewModel.ModbusToolModel.StartAddress;
                for (int i = 0; i < viewModel.ModbusToolModel.ReadCount; i++)
                {
                    bindingList.Add(new ModbusToolPojo<object>()
                        { address = currentAddress, value = (A)(object)(ushort)0, valueIsBool = false });
                    currentAddress++;
                }
                return bindingList;
            }
        }

    }
}