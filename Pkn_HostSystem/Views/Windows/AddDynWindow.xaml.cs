using System.Windows;
using Pkn_HostSystem.ViewModels.Windows;

namespace Pkn_HostSystem.Views.Windows
{
    /// <summary>
    /// AddDynWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddDynWindow 
    {
        public AddDynWindowViewModel viewModel { get; set; } = new AddDynWindowViewModel();
        public AddDynWindow()
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.setSnackbarPresenter(SnackbarPresenter);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
