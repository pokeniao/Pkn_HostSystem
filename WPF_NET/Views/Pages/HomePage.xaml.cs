using System.Collections.Specialized;
using System.Windows.Controls;
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
            //添加LogListBox监听
            HomePageViewModel.HomePageModel.LogListBox.CollectionChanged += Item_CollectionChanged;
        }

        private void Item_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (LogListBox != null && LogListBox.Items.Count > 0)
                {
                    //  LogListBox.SelectedIndex = LogListBox.Items.Count - 1;
                    //  LogListBox.SelectedItem = LogListBox.Items[^1];
                    LogListBox.ScrollIntoView(LogListBox.Items[^1]);

                }
            });

        }

    }
}
