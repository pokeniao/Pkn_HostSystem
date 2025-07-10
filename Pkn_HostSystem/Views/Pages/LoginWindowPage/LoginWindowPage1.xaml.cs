using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.ViewModels.Windows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pkn_HostSystem.Views.Pages.LoginWindowPage
{
    /// <summary>
    /// LoginWindowPage1.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindowPage1 : Page
    {

        public LoginViewModel ViewModel { get; set; }
        public LoginWindowPage1()
        {
            InitializeComponent();
            ViewModel = Ioc.Default.GetRequiredService<LoginViewModel>();
            DataContext = ViewModel;
            ViewModel.setSnackbarPresenter(SnackbarPresenter);
        }
    }
}
