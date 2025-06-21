using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Base.Log;
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
using Wpf.Ui;

namespace Pkn_HostSystem.Views.Pages
{
    /// <summary>
    /// LiveChartsTestPage.xaml 的交互逻辑
    /// </summary>
    public partial class LiveChartsTestPage : Page
    {

        public LiveChartsTestViewModel ViewModel { get; set; }


        public LogBase<LiveChartsTestPage> Log = new LogBase<LiveChartsTestPage>();
        public LiveChartsTestPage()
        {
            InitializeComponent();
            ViewModel = Ioc.Default.GetRequiredService<LiveChartsTestViewModel>();
            DataContext = ViewModel;
            ViewModel.setSnackbarService(SnackbarPresenter);
        }
    }
}
