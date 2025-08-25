using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.ViewModels.Page;
using System.Windows.Controls;

namespace Pkn_HostSystem.Views.Pages
{
    /// <summary>
    /// DesignPage.xaml 的交互逻辑
    /// </summary>
    public partial class DesignPage : Page
    {
        public DesignViewModel ViewModel { get; set; }
        public DesignPage()
        {
            InitializeComponent();
            ViewModel = Ioc.Default.GetRequiredService<DesignViewModel>();
            DataContext = ViewModel;
            ViewModel.setSnackbarPresenter(SnackbarPresenter);
            ViewModel.setHSmartWindowControl(HSmartWindowControlWPF);
        }
    }
}
