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
using System.Xml.Linq;
using WPF_NET.Pojo.Page.MESTcp;
using WPF_NET.ViewModels;

namespace WPF_NET.Views.Pages
{
    /// <summary>
    /// MesTcpPage.xaml 的交互逻辑
    /// </summary>
    public partial class MesTcpPage : Page
    {
        public MesTcpViewModel viewModel { get; set; } =new MesTcpViewModel();
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
