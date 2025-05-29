using CommunityToolkit.Mvvm.DependencyInjection;
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var station = GlobalMannager.StationDictionary.Lookup("工位1").Value as EachStation<Station1>;
            station.Items.Add(new Station1() { CT = "1", ID = "1", 参数 = "1", 名字 = "1" });
            station.DevLog.InfoToRichTextBox("点击添加一行数据");
            station.UserLog.InfoToRichTextBox("添加一行数据");
            station.ErrorLog.ErrorToRichTextBox("点击后错误测试");
        }

        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
            var station = GlobalMannager.StationDictionary.Lookup("工位2").Value as EachStation<Station2>;
            station.Items.Add(new Station2() { CT = "2", ID = "2", 参数2 = "2", 名字2 = "2" });
            station.DevLog.InfoToRichTextBox("点击添加一行数据2");
            station.UserLog.InfoToRichTextBox("添加一行数据2");
            station.ErrorLog.ErrorToRichTextBox("点击后错误测试2");
        }
        private void UserLogRichTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            var rtb = sender as RichTextBox;
            if (rtb == null) return;

            IEachStation? selectedItem = LogTabControl.SelectedItem as IEachStation;
            rtb.Document = selectedItem.UserLog.flowDocument;
        }
        private void DevRichTextBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            var rtb = sender as RichTextBox;
            if (rtb == null) return;

            IEachStation? selectedItem = LogTabControl.SelectedItem as IEachStation;
            rtb.Document = selectedItem.DevLog.flowDocument;
        }

        private void ErrorRichTextBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            var rtb = sender as RichTextBox;
            if (rtb == null) return;

            IEachStation? selectedItem = LogTabControl.SelectedItem as IEachStation;
            rtb.Document = selectedItem.ErrorLog.flowDocument;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogTabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // 获取当前选中的 TabItem 内容
                var selectedItem = LogTabControl.SelectedItem;
                if (selectedItem == null) return;

                // 获取对应的 TabItem 容器（非 null 时表示已可视化）
                var tabItem = LogTabControl.ItemContainerGenerator.ContainerFromItem(selectedItem) as TabItem;
                if (tabItem == null) return;

                // 查找 ContentPresenter
                var contentPresenter = FindVisualChild<ContentPresenter>(tabItem);
                if (contentPresenter == null) return;

                // 强制应用模板
                contentPresenter.ApplyTemplate();

                // 尝试查找 RichTextBox 控件
                var userRtb = FindNamedChild<RichTextBox>(contentPresenter, "UserRichTextBox");
                var devRtb = FindNamedChild<RichTextBox>(contentPresenter, "DevRichTextBox");
                var errorRtb = FindNamedChild<RichTextBox>(contentPresenter, "ErrorRichTextBox");

                // 调用 Loaded 事件逻辑
                if (userRtb != null)
                    UserLogRichTextBox_Loaded(userRtb, new RoutedEventArgs());

                if (devRtb != null)
                    DevRichTextBox_OnLoaded(devRtb, new RoutedEventArgs());

                if (errorRtb != null)
                    ErrorRichTextBox_OnLoaded(errorRtb, new RoutedEventArgs());

            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }
        private T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                    return typedChild;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private T? FindNamedChild<T>(DependencyObject parent, string childName) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild && typedChild.Name == childName)
                    return typedChild;

                var result = FindNamedChild<T>(child, childName);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
