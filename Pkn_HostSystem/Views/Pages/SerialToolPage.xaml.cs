using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Base;
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
    /// SerialToolPage.xaml 的交互逻辑
    /// </summary>
    public partial class SerialToolPage : Page
    {
        public SerialToolViewModel ViewModel { get; set; }

        public SerialToolPage()
        {
            InitializeComponent();
            ViewModel = Ioc.Default.GetRequiredService<SerialToolViewModel>();
            DataContext = ViewModel;
            ViewModel.setSnackbarPresenter(SnackbarPresenter);
        }

        private void SerialToolPage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newHeight = Math.Max(50, e.NewSize.Height / 2 - 100); // 50为最小高度
            AcceptMessageTextBox.Height = newHeight;
            SendMessageTextBox.Height = newHeight;
        }

        private void ComboBox_OnDropDownOpened(object? sender, EventArgs e)
        {
            ViewModel.SerialToolModel.ComList = ScpiSerialTool.GetCOM();
        }
    }
}
