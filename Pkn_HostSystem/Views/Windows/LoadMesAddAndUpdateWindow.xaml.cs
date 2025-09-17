using DynamicData.Binding;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Service.LoadMes;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Windows;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Controls;
using LoadMesAddAndUpdateWindowModel = Pkn_HostSystem.Models.Windows.LoadMesAddAndUpdateWindowModel;

namespace Pkn_HostSystem.Views.Windows
{
    /// <summary>
    /// LoadMesAddWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoadMesAddWindow
    {
        public LoadMesAddAndUpdateWindowsViewModel viewModel { get; set; }


        public LoadMesAddWindow()
        {

            InitializeComponent(); 
          
        }

        //添加
        public LoadMesAddWindow(string title, ObservableCollectionExtended<LoadMesAddAndUpdateWindowModel> mesPojoList) : this()
        {
            DataContext = new LoadMesAddAndUpdateWindowsViewModel();
            Title.Text = title;
            viewModel = (LoadMesAddAndUpdateWindowsViewModel)DataContext;
            viewModel.setSnackbarService(SnackbarPresenter);
            viewModel.mesPojoList = mesPojoList;
        }

        //修改
        public LoadMesAddWindow(string title, LoadMesAddAndUpdateWindowModel item,
            ObservableCollectionExtended<LoadMesAddAndUpdateWindowModel> mesPojoList) : this()
        {
            DataContext = new LoadMesAddAndUpdateWindowsViewModel(item);
            Title.Text = title;
            viewModel = (LoadMesAddAndUpdateWindowsViewModel)DataContext;
            viewModel.setSnackbarService(SnackbarPresenter);
            viewModel.mesPojoList = mesPojoList;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            LoadMesCondition? item = ReqDataGrid.SelectedItem as LoadMesCondition;

            viewModel = (LoadMesAddAndUpdateWindowsViewModel)DataContext;

            ObservableCollection<LoadMesCondition>? items = viewModel?.LoadMesAddAndUpdateWindowModel.Condition;

            if (item?.Key != null)
            {
                //从集合中移除
                items?.Remove(item);
                viewModel?.Log.SuccessAndShowTask("删除一个条件", $"Mes请求{HTTP_Name.Text} ,{item.Key}条件被删掉");
            }
        }

        /// <summary>
        ///  下拉改变,显示循环文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox? comboBox = sender as ComboBox;

            viewModel.LoadMesAddAndUpdateWindowModel.ShowTriggerSet = false;
            viewModel.LoadMesAddAndUpdateWindowModel.ShowInteriorTriggerSet = false;
            switch (comboBox?.SelectedItem)
            {
                case "循环触发":
                    viewModel.LoadMesAddAndUpdateWindowModel.CycText = "循环时间(s)";
                    break;
                case "通讯触发":
                    viewModel.LoadMesAddAndUpdateWindowModel.CycText = "循环读取(ms)";
                    viewModel.LoadMesAddAndUpdateWindowModel.ShowTriggerSet = true;
                    break;
                case "内部触发":
                    viewModel.LoadMesAddAndUpdateWindowModel.CycText = "循环读取(ms)";
                    viewModel.LoadMesAddAndUpdateWindowModel.ShowInteriorTriggerSet = true;
                    break;
            }
            TabControlSelect();
        }

        private void DeleteColForDg(object sender, RoutedEventArgs e)
        {
            HttpHeader? item = HttpHeader.SelectedItem as HttpHeader;
            if (item.Key != null)
            {
                bool b = viewModel.LoadMesAddAndUpdateWindowModel.HttpHeaders.Remove(item);
                if (b)
                {
                    viewModel?.Log.SuccessAndShowTask("删除成功");
                }
                else
                {
                    viewModel?.Log.WarningAndShowTask("删除失败");
                }
            }
        }

        /// <summary>
        /// 页面打开时的加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            //判断是否HTTP请求
            viewModel.LoadMesAddAndUpdateWindowModel.ShowHttpSet = viewModel.LoadMesAddAndUpdateWindowModel.HttpNeed;
            //判断是否是通讯触发
            viewModel.LoadMesAddAndUpdateWindowModel.ShowTriggerSet =
                viewModel.LoadMesAddAndUpdateWindowModel.TriggerType == "通讯触发";
            TabControlSelect();

            //判断是什么形式的通讯模式
            NetWork netWork = GlobalManager.GetNetWork(viewModel.LoadMesAddAndUpdateWindowModel.TriggerConnectName);
            if (netWork ==null)
            {
                return;
            }
            switch (netWork.NetworkDetailed.NetMethod)
            {
                case "ModbusTcp":
                    CloseParamShow();
                    viewModel.LoadMesAddAndUpdateWindowModel.ShowModbusTriggerParam = true;
                    break;
                case "ModbusRtu":
                    CloseParamShow();
                    viewModel.LoadMesAddAndUpdateWindowModel.ShowModbusTriggerParam = true;
                    break;
                case "Tcp客户端":
                    CloseParamShow();
                    viewModel.LoadMesAddAndUpdateWindowModel.ShowTcpTriggerParam = true;
                    break;
                case "Tcp服务器":
                    CloseParamShow();
                    viewModel.LoadMesAddAndUpdateWindowModel.ShowTcpTriggerParam = true;
                    break;
                case "基恩士上位链路通讯":
                    CloseParamShow();
                    viewModel.LoadMesAddAndUpdateWindowModel.ShowKeyenceHostLinkParam = true;
                    break;
                case "串口232/485":
                    CloseParamShow();
                    viewModel.LoadMesAddAndUpdateWindowModel.ShowSerialParam = true;
                    break;
            }
            
        }

        /// <summary>
        /// 触发对象选着时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Selector_OnSelectionChanged2(object sender, SelectionChangedEventArgs e)
        {
            //判断是什么形式的通讯模式
            NetWork netWork = GlobalManager.GetNetWork(viewModel.LoadMesAddAndUpdateWindowModel.TriggerConnectName);
            if (netWork == null)
            {
                return;
            }
            switch (netWork.NetworkDetailed.NetMethod)
            {
                case "ModbusTcp":
                    CloseParamShow();
                    viewModel.LoadMesAddAndUpdateWindowModel.ShowModbusTriggerParam = true;
                    break;
                case "ModbusRtu":
                    CloseParamShow();
                    viewModel.LoadMesAddAndUpdateWindowModel.ShowModbusTriggerParam = true;
                    break;
                case "Tcp客户端":
                    CloseParamShow();
                    viewModel.LoadMesAddAndUpdateWindowModel.ShowTcpTriggerParam = true;
                    break;
                case "Tcp服务器":
                    CloseParamShow();
                    viewModel.LoadMesAddAndUpdateWindowModel.ShowTcpTriggerParam = true;
                    break;
                case "基恩士上位链路通讯":
                    CloseParamShow();
                    viewModel.LoadMesAddAndUpdateWindowModel.ShowKeyenceHostLinkParam = true;
                    break;
                case "串口232/485":
                    CloseParamShow();
                    viewModel.LoadMesAddAndUpdateWindowModel.ShowSerialParam = true;
                    break;
            }
        }
        /// <summary>
        /// 关闭所有参数显示
        /// </summary>
        private void CloseParamShow()
        {
            viewModel.LoadMesAddAndUpdateWindowModel.ShowKeyenceHostLinkParam =false;
            viewModel.LoadMesAddAndUpdateWindowModel.ShowModbusTriggerParam =false;
            viewModel.LoadMesAddAndUpdateWindowModel.ShowSerialParam =false;
            viewModel.LoadMesAddAndUpdateWindowModel.ShowTcpTriggerParam =false;
          
        }

        /// <summary>
        /// 发送HTTP请求按钮切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            //判断是否HTTP请求
            viewModel.LoadMesAddAndUpdateWindowModel.ShowHttpSet = viewModel.LoadMesAddAndUpdateWindowModel.HttpNeed;
            TabControlSelect();
        }

        private void LocalSaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            viewModel.LoadMesAddAndUpdateWindowModel.ShowLocalSave = viewModel.LoadMesAddAndUpdateWindowModel.LocalSave;
            TabControlSelect();
        }

        /// <summary>
        /// Tab页面自动选择
        /// </summary>
        private void TabControlSelect()
        {
            //如果当前选中的是没有关闭的,就不需要进行改变
            int cur = SetTabControl.SelectedIndex;
            switch (cur)
            {
                case 0:
                    if (viewModel.LoadMesAddAndUpdateWindowModel.ShowHttpSet)
                    {
                        return;
                    }

                    break;
                case 1:
                    if (viewModel.LoadMesAddAndUpdateWindowModel.ShowTriggerSet)
                    {
                        return;
                    }
                    break;
                case 2:
                    if (viewModel.LoadMesAddAndUpdateWindowModel.ShowInteriorTriggerSet)
                    {
                        return;
                    }
                    break;
                case 3:
                    if (viewModel.LoadMesAddAndUpdateWindowModel.ShowLocalSave)
                    {
                        return;
                    }
                    break;
            }

            //判断那个是显示的就选中那个
            if (viewModel.LoadMesAddAndUpdateWindowModel.ShowHttpSet)
            {
                SetTabControl.SelectedIndex = 0;
                
            } else if (viewModel.LoadMesAddAndUpdateWindowModel.ShowTriggerSet)
            {
                SetTabControl.SelectedIndex = 1;
            } else if (viewModel.LoadMesAddAndUpdateWindowModel.ShowInteriorTriggerSet)
            {
                SetTabControl.SelectedIndex = 2;
            }
            else if (viewModel.LoadMesAddAndUpdateWindowModel.ShowLocalSave)
            {
                SetTabControl.SelectedIndex = 3;
            }
        }


    }
}