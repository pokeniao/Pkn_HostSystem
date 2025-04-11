using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using WPF_NET.ViewModels;

namespace WPF_NET.Views.Pages
{
    /// <summary>
    /// LoadMesPage.xaml 的交互逻辑
    /// </summary>
    public partial class LoadMesPage : Page
    {

        public LoadMesPageViewModel LoadMesPageViewModel { get; set; }
        public LoadMesPage()
        {
            InitializeComponent();
            LoadMesPageViewModel = (LoadMesPageViewModel)this.DataContext;
            LoadMesPageViewModel.setSnackbarService(SnackbarPresenter);
            //添加为ReturnMessageList监听
            LoadMesPageViewModel.LoadMesPageModel.ReturnMessageList.CollectionChanged += Item_CollectionChanged;
        }

        private void Item_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                //不在聚焦,自动下滑
                if (!HttpLogListBox.IsKeyboardFocusWithin)
                {
                    if (HttpLogListBox != null && HttpLogListBox.Items.Count > 0)
                    {
                        HttpLogListBox.ScrollIntoView(HttpLogListBox.Items[^1]);
                    }
                }
            });
        }



        /// <summary>
        /// 滑动到底部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (HttpLogListBox.Items.Count == 0)
            {
                return;
            }
            HttpLogListBox.ScrollIntoView(HttpLogListBox.Items[^1]);
        }
    }
}
