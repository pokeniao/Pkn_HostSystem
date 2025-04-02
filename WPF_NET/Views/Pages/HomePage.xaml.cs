using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using WPF_NET.ViewModels;


namespace WPF_NET.Views.Pages
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage : Page
    {
        private HomePageViewModel HomePageViewModel { get; set; }

        public HomePage()
        {
            InitializeComponent();
            HomePageViewModel = (HomePageViewModel)DataContext;
            HomePageViewModel.setSnackbarPresenter(SnackbarPresenter);

            //添加LogListBox监听
            HomePageViewModel.HomePageModel.LogListBox.CollectionChanged += Item_CollectionChanged;
        }

        private void Item_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
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
            CA_ConnectPLC.Visibility = Visibility.Collapsed;
            border.Visibility = Visibility.Visible;
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
                        CA_ConnectPLC.Visibility = Visibility.Visible;
                    }));
                }
            );
        }

        #endregion

        #region 下拉Combobox

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            HomePageViewModel.ModbusToolModel.ModbusRtu_COM = HomePageViewModel.ModbusBase.getCOM().ToList();
        }

        private void ComboBox_DropDownOpened_1(object sender, EventArgs e)
        {
            HomePageViewModel.ModbusToolModel.ModbusTcp_Ip = HomePageViewModel.ModbusBase.getIpAddress().ToList();
        }
        #endregion


    }
}