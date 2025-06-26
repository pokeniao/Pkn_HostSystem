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
using System.Windows.Shapes;

namespace Pkn_HostSystem.Views.Windows
{
    /// <summary>
    /// SetLiveChartsParamWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetLiveChartsParamWindow 
    {
        public SetLiveChartsParamViewModel ViewModel { get; set; }

        public SetLiveChartsParamWindow()
        {
            InitializeComponent();
            ViewModel = new SetLiveChartsParamViewModel();
            DataContext = ViewModel;

            ViewModel.setSnackbarService(SnackbarPresenter);

        }
        /// <summary>
        /// 取消按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }
    }
}
