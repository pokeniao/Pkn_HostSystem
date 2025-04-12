using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Pojo.Page.MESTcp;
using MesTcpViewModel = Pkn_HostSystem.ViewModels.Page.MesTcpViewModel;

namespace Pkn_HostSystem.Views.Pages
{
    /// <summary>
    /// MesTcpPage.xaml 的交互逻辑
    /// </summary>
    public partial class MesTcpPage : Page
    {
        public MesTcpViewModel viewModel { get; set; } = Ioc.Default.GetRequiredService<MesTcpViewModel>();
        public MesTcpPage()
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.setSnackbarPresenter(SnackbarPresenter);
        }


        /// <summary>
        /// 选择方式修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DynNameListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

              MesTcpPojo? mesTcpPojo = DynNameListBox.SelectedItem as MesTcpPojo;

            if (mesTcpPojo != null)
            {
                //绑定选中行的数据到页面
                viewModel.MesTcpModel.DynConditionItemList = mesTcpPojo.DynCondition;
                viewModel.MesTcpModel.Message = mesTcpPojo.Message;
            }
        }
        //每当文本发生修改
        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            MesTcpPojo? mesTcpPojo = DynNameListBox.SelectedItem as MesTcpPojo;

            if (mesTcpPojo != null)
            {
                mesTcpPojo.Message = viewModel.MesTcpModel.Message;
            }
        }

    }
}
