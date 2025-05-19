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
using System.Windows.Shapes;

namespace Pkn_HostSystem.Views.Pages
{
    /// <summary>
    /// VisionPageViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class VisionPage : Page
    {

        public VisionPageViewModel ViewModel { get; set; }
        public VisionPage()
        {
            InitializeComponent();
            ViewModel = Ioc.Default.GetRequiredService<VisionPageViewModel>();
            DataContext = ViewModel;
            ViewModel.setSnackbarPresenter(SnackbarPresenter);
            ViewModel.setHSmartWindowControl(HalconControl);
        }
    }
}
