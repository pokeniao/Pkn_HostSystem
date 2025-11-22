using CommunityToolkit.Mvvm.DependencyInjection;
using DynamicData;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Models.Pojo;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Page;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
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

        public LogControl<HomePage> Log { get; set; } = new();

        public HomePage()
        {
            InitializeComponent();

            DataContext = Ioc.Default.GetRequiredService<HomePageViewModel>();
            HomePageViewModel = (HomePageViewModel)DataContext;
            HomePageViewModel.setSnackbarPresenter(SnackbarPresenter);
            // ViewModel.setSnackbarPresenter(SnackbarPresenter);
            
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
        ///  工单连接卡片的展开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CA_pppOrderSelect_OnClick(object sender, RoutedEventArgs e)
        {
            showOpenCard(OrderBorder, "OpenPppOrder");
        }

        /// <summary>
        ///  工单连接卡片的收起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
            showCloseCard(OrderBorder, "ClosePppOrder");
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

        /// <summary>
        /// 展开数据库卡片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JDBC_OnClick(object sender, RoutedEventArgs e)
        {
            showOpenCard(JDBC_Border, "JDBC_Open_Storyboard");
        }

        /// <summary>
        /// 收起数据库卡片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JDBC_Close_OnClick(object sender, RoutedEventArgs e)
        {
            showCloseCard(JDBC_Border, "JDBC_Close_Storyboard");
        }

        /// <summary>
        /// 展开内部寄存器卡片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Register_OnClick(object sender, RoutedEventArgs e)
        {
            showOpenCard(Register_Border, "Register_Open");
        }


        /// <summary>
        /// 收起数据库卡片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Register_Close_OnClick(object sender, RoutedEventArgs e)
        {
            showCloseCard(Register_Border, "Register_Close");
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
            CA_pppOrderSelect.Visibility = Visibility.Hidden;
            CA_CameraSelect.Visibility = Visibility.Hidden;
            JDBC_CardAction.Visibility = Visibility.Hidden;
            Register_CardAction.Visibility = Visibility.Hidden;
            //二. 所有border消失,重新选择
            ConnectPLCBorder.Visibility = Visibility.Collapsed;
            OrderBorder.Visibility = Visibility.Collapsed;
            CameraBorder.Visibility = Visibility.Collapsed;
            JDBC_Border.Visibility = Visibility.Collapsed;
            Register_Border.Visibility = Visibility.Collapsed;
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
                        CA_pppOrderSelect.Visibility = Visibility.Visible;
                        CA_CameraSelect.Visibility = Visibility.Visible;
                        JDBC_CardAction.Visibility = Visibility.Visible;
                        Register_CardAction.Visibility = Visibility.Visible;
                    });
                }
            );
        }

        #endregion

        #region 连接PLC下拉Combobox

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            HomePageViewModel.HomeSetConnectModel.Coms = ModbusBase.GetCOM().ToList();
        }

        private void ComboBox_DropDownOpened_1(object sender, EventArgs e)
        {
            HomePageViewModel.HomeSetConnectModel.Ips = ModbusBase.GetIpAddress().ToList();
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
        }

        #endregion

        #region  工单Combobox

        /// <summary>
        /// 下拉Combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // private async void SelectPppOrderDown(object? sender, EventArgs e)
        // {
        //     //进行一次查询
        //     string httpName = HomePageViewModel.HomePageModel.HttpName;
        //
        //     var pppBase003OrderList = new PppBase003OrderList();
        //     var (succeed, pppOrderLists) =
        //         await pppBase003OrderList.GetPppOrderLists(httpName, new CancellationTokenSource());
        //
        //     if (succeed)
        //     {
        //         //返回结果,显示到页面Combobox提供选择
        //         HomePageViewModel.HomePageModel.PppOrderLists = pppOrderLists;
        //
        //         Log.Info("获取工单成功");
        //     }
        //     else
        //     {
        //         Log.Info("获取工单失败--pppBase003OrderList.GetPppOrderLists返回false");
        //     }
        // }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectPppOrderButton(object sender, RoutedEventArgs e)
        {
            HomePageModel homePageModel = HomePageViewModel.HomePageModel;
            VOCPojo vocPojo = homePageModel.VocPojo;

            vocPojo.TestTime = vocPojo.SetValueTestTime;
            vocPojo.TriggerMax = vocPojo.SetValueTriggerMax;

            vocPojo.MachineId = vocPojo.SetMachineId;
            vocPojo.GroupCode = vocPojo.SetGroupCode;
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
        }
        /// <summary>
        /// 删除MenuItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            DesignModel? model = HomeProjectDataGrid.SelectedValue as DesignModel;
            if (model == null)
            {
                return;
            }

            var selectedValue = model.ProjectName;

            if (selectedValue != null)
            {
                GlobalManager.ProjectDictionary.Remove(selectedValue);
                DesignViewModel designViewModel = Ioc.Default.GetRequiredService<DesignViewModel>();
                designViewModel.DesignModel = null;
                designViewModel.ProjectName = null;
            }
        }
    }
}