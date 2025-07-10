using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.ViewModels.Windows;
using Pkn_HostSystem.Views.Pages;
using Pkn_HostSystem.Views.Pages.LoginWindowPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf.Ui;

namespace Pkn_HostSystem.Views.Windows
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow
    {
        public LoginViewModel ViewModel { get; set; }

        public LoginWindow()
        {
            InitializeComponent();
            Loaded += (_, _) =>
            {
                // //路由到Ioc的实例化对象
                // var navigation = Ioc.Default.GetRequiredService<INavigationService>();
                // navigation.SetNavigationControl(RootNavigation);
                // // 默认导航页面
                // navigation.Navigate(typeof(LoginWindowPage1));

                //不使用IOC的跟路径设置
                RootNavigation.Navigate(typeof(LoginWindowPage1));
            };

            ViewModel = Ioc.Default.GetRequiredService<LoginViewModel>();
            this.DataContext = ViewModel;
            ViewModel.setSnackbarPresenter(SnackbarPresenter);
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