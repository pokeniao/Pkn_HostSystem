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
using WPF_NET.ViewModels.Windows;

namespace WPF_NET.Views.Windows
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
