using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Models.Pojo;
using Pkn_HostSystem.Pojo.Page.HomePage;
using Pkn_HostSystem.Service.UserDefined;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Page;
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

        public LogBase<HomePage> Log { get; set; } = new LogBase<HomePage>();

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

        #region PLC连接卡片的播放动画

        private void CA_ConnectPLC_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //所有卡片消失
            CA_ConnectPLC.Visibility = Visibility.Collapsed;
            CA_bydOrderSelect.Visibility= Visibility.Collapsed;

            //展示一个页面,其他消失
            border.Visibility = Visibility.Visible;
            OrderBorder.Visibility= Visibility.Collapsed;
            Storyboard? storyboard = FindResource("OPENcontentPLC") as Storyboard;

            storyboard?.Begin();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Storyboard? storyboard = FindResource("CLOSEcontentPLC") as Storyboard;
            storyboard?.Begin();
            Task.Run(async () =>
                {
                    await Task.Delay(300);
                    Dispatcher.Invoke(new Action(() =>
                    {

                        border.Visibility = Visibility.Collapsed;

                        //显示所有卡片
                        CA_ConnectPLC.Visibility = Visibility.Visible;
                        CA_bydOrderSelect.Visibility = Visibility.Visible;

                    }));
                }
            );
        }

        #endregion

        #region 比亚迪工单连接卡片的播放动画
        private void CA_bydOrderSelect_OnClick(object sender, RoutedEventArgs e)
        {
            //所有卡片消失
            CA_ConnectPLC.Visibility = Visibility.Hidden;
            CA_bydOrderSelect.Visibility = Visibility.Collapsed;

            //展示一个页面,其他消失
            OrderBorder.Visibility = Visibility.Visible;
            border.Visibility = Visibility.Collapsed;
            Storyboard? storyboard = FindResource("OpenBydOrder") as Storyboard;
            storyboard?.Begin();
        }
        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
            Storyboard? storyboard = FindResource("CloseBydOrder") as Storyboard;
            storyboard?.Begin();
            Task.Run(async () =>
                {
                    await Task.Delay(300);
                    Dispatcher.Invoke(new Action(() =>
                    {

                        OrderBorder.Visibility = Visibility.Collapsed;

                        //显示所有卡片
                        CA_ConnectPLC.Visibility = Visibility.Visible;
                        CA_bydOrderSelect.Visibility = Visibility.Visible;

                    }));
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
            var (succeed,bydOrderLists) = await bydBase003OrderList.GetBydOrderLists(httpName,new CancellationTokenSource());

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
        }
    }
}