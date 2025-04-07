using System.Windows.Controls;
using System.Windows.Input;
using WPF_NET.Models;
using WPF_NET.ViewModels.Windows;

namespace WPF_NET.Views.Windows
{
    /// <summary>
    /// LoadMesAddWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoadMesAddWindow
    {
        public LoadMesAddAndUpdateWindowsViewModel viewModel { get; set; }
        public LoadMesAddWindow()
        {
            InitializeComponent();
            
        }
        public LoadMesAddWindow(string title):this()
        {
            DataContext = new LoadMesAddAndUpdateWindowsViewModel();
            Title.Text = title;
            viewModel = (LoadMesAddAndUpdateWindowsViewModel)DataContext;
            viewModel.setSnackbarService(SnackbarPresenter);
        }

        public LoadMesAddWindow(string title, LoadMesAddAndUpdateWindowModel item):this()
        {
            DataContext = new LoadMesAddAndUpdateWindowsViewModel(item);
            Title.Text = title;
            LoadMesAddAndUpdateWindowsViewModel viewModel = (LoadMesAddAndUpdateWindowsViewModel)DataContext;
            viewModel.setSnackbarService(SnackbarPresenter);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
