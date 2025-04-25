using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Pojo.Page.HomePage;
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

        private void ClearLog(object sender, RoutedEventArgs e)
        {
            GlobalMannager.GlobalDictionary.TryGetValue("LogListBox", out var obj);
            ObservableCollection<string> list = (ObservableCollection<string>)obj;
            list.Clear();
        }
    }
}