using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.ViewModels.Page;
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

namespace Pkn_HostSystem.Views.Pages
{
    /// <summary>
    /// TcpToolPage.xaml 的交互逻辑
    /// </summary>
    public partial class TcpToolPage : Page
    {

        public TcpToolViewModel ViewModel { get; set; }
        public TcpToolPage()
        {
            InitializeComponent();
            ViewModel = Ioc.Default.GetRequiredService<TcpToolViewModel>();
            DataContext = ViewModel;
           ViewModel.setSnackbarPresenter(SnackbarPresenter);
        }
    }
}
