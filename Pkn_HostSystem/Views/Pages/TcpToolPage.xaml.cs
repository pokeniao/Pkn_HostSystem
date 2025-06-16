using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.ViewModels.Page;
using System.Windows;
using System.Windows.Controls;

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

        /// <summary>
        /// 页面大小改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void TcpToolPage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double MaxHeight = (e.NewSize.Height - e.NewSize.Height * 0.4) / 2;
            ClientAcceptMessageTextBox.MaxHeight = MaxHeight;
            ClientSendMessageTextBox.MaxHeight = MaxHeight;
            ServerAcceptMessageTextBox.MaxHeight = MaxHeight;
            ServerSendMessageTextBox.MaxHeight = MaxHeight;
           
            ConnectClientListBox.MaxHeight = (e.NewSize.Height - e.NewSize.Height * 0.4);
        }
    }
}
