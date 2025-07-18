using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Models.Core.Interface;
using Pkn_HostSystem.Models.Mapper;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Page;
using System.Windows;
using System.Windows.Controls;
using Station1 = Pkn_HostSystem.Service.Stations.Station1;

namespace Pkn_HostSystem.Views.Pages
{
    /// <summary>
    /// StationPage.xaml 的交互逻辑
    /// </summary>
    public partial class StationPage : Page
    {
        public StationViewModel ViewModel { get; set; }

        public StationPage()
        {
            InitializeComponent();
            ViewModel = Ioc.Default.GetRequiredService<StationViewModel>();
            DataContext = ViewModel;
            ViewModel.setSnackbarPresenter(SnackbarPresenter);
        }

        #region 富文本加载事件
        private void UserLogRichTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            var rtb = sender as RichTextBox;
            if (rtb == null) return;

            IEachStation selectedItem = LogTabControl.SelectedItem as IEachStation;
            if (selectedItem == null)
            {return; }

            selectedItem.UserLog.richTextBox = rtb;
            rtb.Document = selectedItem.UserLog.flowDocument;
        }
        private void DevRichTextBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            var rtb = sender as RichTextBox;
            if (rtb == null) return;

            IEachStation selectedItem = LogTabControl.SelectedItem as IEachStation;
            if (selectedItem == null)
            { return; }
            selectedItem.DevLog.richTextBox = rtb;
            rtb.Document = selectedItem.DevLog.flowDocument;
        }

        private void ErrorRichTextBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            var rtb = sender as RichTextBox;
            if (rtb == null) return;

            IEachStation? selectedItem = LogTabControl.SelectedItem as IEachStation;
            if (selectedItem == null)
            { return; }
            selectedItem.ErrorLog.richTextBox = rtb;
            rtb.Document = selectedItem.ErrorLog.flowDocument;
        }
        private void LogTabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserLogRichTextBox_Loaded(UserRichTextBox, new RoutedEventArgs());
            DevRichTextBox_OnLoaded(DevRichTextBox, new RoutedEventArgs());
            ErrorRichTextBox_OnLoaded(ErrorRichTextBox, new RoutedEventArgs());
        }
        #endregion
        /// <summary>
        /// 清除当前日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearLog_OnClick(object sender, RoutedEventArgs e)
        {
            IEachStation? selectedItem = LogTabControl.SelectedItem as IEachStation;
            if (selectedItem == null)
            { return; }
            int selectedIndex = TabControl2.SelectedIndex;
            switch (selectedIndex)
            {
                case 0:
                    selectedItem.UserLog.flowDocument.Blocks.Clear();
                    break;
                case 1:
                    selectedItem.ErrorLog.flowDocument.Blocks.Clear();
                    break;
                case 2:
                    selectedItem.DevLog.flowDocument.Blocks.Clear();
                    break;
            }
        }
        /// <summary>
        /// 滑动到底部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollDown_OnClick(object sender, RoutedEventArgs e)
        {
            IEachStation? selectedItem = LogTabControl.SelectedItem as IEachStation;
            if (selectedItem == null)
            { return; }
            int selectedIndex = TabControl2.SelectedIndex;
            switch (selectedIndex)
            {
                case 0:
                    selectedItem.UserLog.richTextBox.ScrollToEnd();
                    break;
                case 1:
                    selectedItem.ErrorLog.richTextBox.ScrollToEnd();
                    break;
                case 2:
                    selectedItem.DevLog.richTextBox.ScrollToEnd();
                    break;
            }
        }

        private void ClearLog_OnClick1(object sender, RoutedEventArgs e)
        {
            // ScanQR scanQr = new ScanQR();
            // scanQr.id = new SnowflakeIdGenerator(1, 1).GetId();
            // scanQr.qr_code = "12345";
            // scanQr.orderCode = "123";
            // scanQr.materialCode = "123";
            // scanQr.CT = "12";
            // scanQr.Pass = 'Y';
            //
            // int insert = scanQr.insert(scanQr);
            // if (insert != 0)
            // {
            //     ViewModel.Log.SuccessAndShow($"添加成功{insert}");
            // }



            // var value = GlobalManager.StationDictionary.Lookup("生产信息上传").Value as EachStation<Station2>;
            // dynamic value = GlobalManager.StationDictionary.Lookup("扫码过站").Value;
            //
            // value.AddItem(new Station1() { CT = "1" });
            // var observableCollection = value.Items[2];
            // observableCollection.合格 = "123";
            // value.Items[2] = observableCollection;
            //
            // value.UserLog.InfoToRichTextBox("添加一行");

            // ⬇️ 触发一条测试日志，看能否写成功
            Console.WriteLine("测试");
        }
    }
}
