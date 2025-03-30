using System.Windows.Controls;
using WPF_NET.ViewModels;

namespace WPF_NET.Views.Pages
{
    /// <summary>
    /// LoadMesPage.xaml 的交互逻辑
    /// </summary>
    public partial class LoadMesPage : Page
    {

        public LoadMesPageViewModel LoadMesPageViewModel { get; set; }
        public LoadMesPage()
        {
            InitializeComponent();
            LoadMesPageViewModel = (LoadMesPageViewModel)this.DataContext;
            LoadMesPageViewModel.setSnackbarService(SnackbarPresenter);
        }
        
    }
}
