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
    /// S7ToolPage.xaml 的交互逻辑
    /// </summary>
    public partial class S7ToolPage : Page
    {

        public S7ToolViewModel ViewModel { get; set; } = new S7ToolViewModel();
        public S7ToolPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.setSnackbarPresenter(SnackbarPresenter);
        }

        private void ComboBox_DropDownOpened_1(object? sender, EventArgs e)
        {
            ViewModel.Ips = ModbusBase.GetIpAddress().ToList();
        }
    }
}
