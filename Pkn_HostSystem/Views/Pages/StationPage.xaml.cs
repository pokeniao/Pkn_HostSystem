using CommunityToolkit.Mvvm.DependencyInjection;
using DynamicData.Kernel;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.Stations;
using Pkn_HostSystem.ViewModels.Page;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        }

        #region 富文本加载事件
        private void UserLogRichTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            var rtb = sender as RichTextBox;
            if (rtb == null) return;

            IEachStation? selectedItem = LogTabControl.SelectedItem as IEachStation;
            selectedItem.UserLog.richTextBox = rtb;
            rtb.Document = selectedItem.UserLog.flowDocument;
        }
        private void DevRichTextBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            var rtb = sender as RichTextBox;
            if (rtb == null) return;

            IEachStation? selectedItem = LogTabControl.SelectedItem as IEachStation;
            selectedItem.DevLog.richTextBox = rtb;
            rtb.Document = selectedItem.DevLog.flowDocument;
        }

        private void ErrorRichTextBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            var rtb = sender as RichTextBox;
            if (rtb == null) return;

            IEachStation? selectedItem = LogTabControl.SelectedItem as IEachStation;
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
            var value = GlobalMannager.StationDictionary.Lookup("工位1").Value as EachStation<Station1>;

            value.AddItem(new Station1() { CT = "1", ID = "1", 参数 = "1", 名字 = "1" });
            value.UserLog.InfoToRichTextBox("添加一行");
        }
    }
}
