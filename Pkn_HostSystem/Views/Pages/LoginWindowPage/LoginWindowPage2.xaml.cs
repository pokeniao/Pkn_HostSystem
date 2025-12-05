using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.ViewModels.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pkn_HostSystem.Views.Pages.LoginWindowPage
{
    /// <summary>
    /// LoginWindowPage2.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindowPage2 : Page
    {
        public Login2ViewModel ViewModel { get; set; }
        public LoginWindowPage2()
        {
            InitializeComponent();
            ViewModel = Ioc.Default.GetRequiredService<Login2ViewModel>();
            ViewModel.Page = this;
            DataContext = ViewModel;
            ViewModel.setSnackbarPresenter(SnackbarPresenter);
        }

        private void Button_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                e.Handled = true;   // 阻止回车触发按钮
        }
        public void Close()
        {
            var window = Window.GetWindow(this);
            window?.Close();

        }
    }
}
