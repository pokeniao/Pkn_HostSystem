using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Models.Pojo;
using Pkn_HostSystem.Service.UserDefined;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Page;
using System.Windows.Input;
using Wpf.Ui.Controls;
using MessageBox = Pkn_HostSystem.Views.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;


namespace Pkn_HostSystem.Views.Pages
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePageViewModel HomePageViewModel { get; set; }

        public LogBase<HomePage> Log { get; set; } = new();

        public HomePage()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetRequiredService<HomePageViewModel>();

            HomePageViewModel = (HomePageViewModel)DataContext;
            HomePageViewModel.setSnackbarPresenter(SnackbarPresenter);

            //添加LogListBox监听
            HomePageViewModel.HomePageModel.LogListBox.CollectionChanged += Item_CollectionChanged;
        }

        private void Item_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                //不在聚焦,自动下滑
                if (!LogListBox.IsKeyboardFocusWithin)
                {
                    if (LogListBox != null && LogListBox.Items.Count > 0)
                    {
                        LogListBox.ScrollIntoView(LogListBox.Items[^1]);
                    }
                }
            });
        }

        #region 播放动画

        /// <summary>
        /// PLC连接卡片的展开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CA_ConnectPLC_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            showOpenCard(ConnectPLCBorder, "OPENcontentPLC");
        }

        /// <summary>
        /// PLC连接卡片的收起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            showCloseCard(ConnectPLCBorder, "CLOSEcontentPLC");
        }

        /// <summary>
        /// 比亚迪工单连接卡片的展开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CA_bydOrderSelect_OnClick(object sender, RoutedEventArgs e)
        {
            showOpenCard(OrderBorder, "OpenBydOrder");
        }

        /// <summary>
        /// 比亚迪工单连接卡片的收起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
            showCloseCard(OrderBorder, "CloseBydOrder");
        }
        /// <summary>
        /// 展开照相卡片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CA_CameraSelect_OnClick(object sender, RoutedEventArgs e)
        {
            showOpenCard(CameraBorder, "OpenCameraSelect");
        }
        /// <summary>
        /// 收起照相卡片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick3(object sender, RoutedEventArgs e)
        {
            showCloseCard(CameraBorder, "CloseCameraSelect");
        }
        #endregion

        #region 卡片私有类

        /// <summary>
        /// 显示卡片
        /// </summary>
        /// <param name="border"></param>
        private void showOpenCard(Border border, string storyboardName)
        {
            //一. 所有卡片消失
            CA_ConnectPLC.Visibility = Visibility.Hidden;
            CA_bydOrderSelect.Visibility = Visibility.Hidden;
            CA_CameraSelect.Visibility = Visibility.Hidden;

            //二. 所有border消失,重新选择
            ConnectPLCBorder.Visibility = Visibility.Collapsed;
            OrderBorder.Visibility = Visibility.Collapsed;
            CameraBorder.Visibility = Visibility.Collapsed;
            //三. 展示当前点击的
            border.Visibility = Visibility.Visible;
            Storyboard? storyboard = FindResource(storyboardName) as Storyboard;
            storyboard?.Begin();
        }

        /// <summary>
        /// 关闭卡片内容显示
        /// </summary>
        /// <param name="border"></param>
        /// <param name="storyboardName"></param>
        private void showCloseCard(Border border, string storyboardName)
        {
            Storyboard? storyboard = FindResource(storyboardName) as Storyboard;
            storyboard?.Begin();
            Task.Run(async () =>
                {
                    await Task.Delay(300);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        border.Visibility = Visibility.Collapsed;
                        //显示所有卡片
                        CA_ConnectPLC.Visibility = Visibility.Visible;
                        CA_bydOrderSelect.Visibility = Visibility.Visible;
                        CA_CameraSelect.Visibility = Visibility.Visible;
                    });
                }
            );
        }

        #endregion

        #region 连接PLC下拉Combobox

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            HomePageViewModel.ModbusToolModel.ModbusRtu_COM = ModbusToolModel.modbusBase.getCOM().ToList();
        }

        private void ComboBox_DropDownOpened_1(object sender, EventArgs e)
        {
            HomePageViewModel.ModbusToolModel.ModbusTcp_Ip = ModbusToolModel.modbusBase.getIpAddress().ToList();
        }

        #endregion


        #region 连接编辑提交,检测

        private async void SetConnectDg_OnCellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
        {
            NetworkDetailed? item = setConnectDg.SelectedItem as NetworkDetailed;


            if (e.EditAction == DataGridEditAction.Commit)
            {
                var textBox = e.EditingElement as TextBox;
                if (string.IsNullOrWhiteSpace(textBox?.Text))
                {
                    MessageBox messageBox = new MessageBox("不能为null");
                    messageBox.ShowDialog();
                    e.Cancel = true; // ❌ 阻止编辑提交
                    return;
                }

                foreach (var connectPojo in HomePageViewModel.HomePageModel.SetConnectDg)
                {
                    if (item.Name == null && connectPojo.Name == textBox?.Text)
                    {
                        MessageBox messageBox = new MessageBox("名字已经存在,请修改");

                        messageBox.ShowDialog();
                        e.Cancel = true; // ❌ 阻止编辑提交
                        return;
                    }
                    else if (item.Name != null)
                    {
                        //当前名字已存在,不会提示报错自己已存在
                        string thisName = item.Name;
                        if (connectPojo.Name == textBox?.Text && thisName != textBox?.Text)
                        {
                            MessageBox messageBox = new MessageBox("名字已经存在, 请修改");

                            messageBox.ShowDialog();
                            e.Cancel = true; // ❌ 阻止编辑提交
                            return;
                        }
                    }
                }
            }
        }

        #endregion

        #region 清除日志

        private void ClearLog(object sender, RoutedEventArgs e)
        {
            GlobalMannager.GlobalDictionary.TryGetValue("LogListBox", out var obj);
            ObservableCollection<string> list = (ObservableCollection<string>)obj;
            list.Clear();
        }

        #endregion

        #region 比亚迪工单Combobox

        /// <summary>
        /// 下拉Combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SelectBydOrderDown(object? sender, EventArgs e)
        {
            //进行一次查询
            string httpName = HomePageViewModel.HomePageModel.HttpName;

            var bydBase003OrderList = new BydBase003OrderList();
            var (succeed, bydOrderLists) =
                await bydBase003OrderList.GetBydOrderLists(httpName, new CancellationTokenSource());

            if (succeed)
            {
                //返回结果,显示到页面Combobox提供选择
                HomePageViewModel.HomePageModel.BydOrderLists = bydOrderLists;
                Log.Info("获取工单成功");
            }
            else
            {
                Log.Info("获取工单失败--bydBase003OrderList.GetBydOrderLists返回false");
            }
        }

        private void SelectedBydOrder(object sender, SelectionChangedEventArgs e)
        {
            //选中结果赋值到显示区域
            ComboBox? comboBox = sender as ComboBox;
            var orderList = comboBox.SelectedValue as BydOrderList;
            HomePageViewModel.HomePageModel.CurrentSelectBydOrder = orderList;
        }

        #endregion

        /// <summary>
        /// 页面尺寸修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void HomePage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            LogListBorder.MaxHeight = e.NewSize.Height - 100;
            LogListBox.Height = e.NewSize.Height - 100;
        }


    }
}