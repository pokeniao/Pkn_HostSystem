using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Page;

namespace Pkn_HostSystem.Views.Pages
{
    /// <summary>
    /// LoadMesPage.xaml 的交互逻辑
    /// </summary>
    public partial class LoadMesPage : Page
    {

        public LoadMesPageViewModel LoadMesPageViewModel { get; set; }


        public LogControl<LoadMesPage> Log = new LogControl<LoadMesPage>();
        public LoadMesPage()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetRequiredService<LoadMesPageViewModel>();
            LoadMesPageViewModel = (LoadMesPageViewModel)DataContext;
            LoadMesPageViewModel.setSnackbarService(SnackbarPresenter);

        }

        /// <summary>
        /// 滑动到底部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void ClearLog(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// 页面大小发生改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void LoadMesPage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newHeight = Math.Max(50, e.NewSize.Height - 100); // 50为最小高度
            DataGrid.Height = newHeight;
        }
    }
}
