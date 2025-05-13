using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Page;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace Pkn_HostSystem.Views.Pages
{
    /// <summary>
    /// SettingsPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPageViewModel ViewModel { get; set; }
        public SettingsPage()
        {
            InitializeComponent();
            ViewModel = Ioc.Default.GetRequiredService<SettingsPageViewModel>();
            DataContext = ViewModel;
            ViewModel.setSnackbarPresenter(SnackbarPresenter);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {


            if (System.IO.Directory.Exists(GlobalMannager.AppFolder))
            {
                Process.Start("explorer.exe", GlobalMannager.AppFolder);
            }
            else
            {
                MessageBox.Show("文件夹不存在！");
            }
        }
    }
}