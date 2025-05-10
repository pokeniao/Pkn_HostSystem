using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.ViewModels.Page;
using System.Windows;
using System.Windows.Controls;

namespace Pkn_HostSystem.Views.Pages
{
    /// <summary>
    /// ProductivePage.xaml 的交互逻辑
    /// </summary>
    public partial class ProductivePage : Page
    {
        public ProductiveViewModel ProductiveViewModel { get; set; }

        public ProductivePage()
        {
            InitializeComponent();
            ProductiveViewModel = Ioc.Default.GetRequiredService<ProductiveViewModel>();
            DataContext = ProductiveViewModel;
            ProductiveViewModel.setSnackbarPresenter(SnackbarPresenter);
        }

        #region 添加和删除

        /// <summary>
        /// 删除一行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeletePruAndConsumerDg(object sender, RoutedEventArgs e)
        {
            Productive? item = PruAndConsumerDataGrid.SelectedValue as Productive;

            ProductiveViewModel.ProductiveModel.Productives.Remove(item);
        }

        /// <summary>
        /// 删除一行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DetailedDgDelete(object sender, RoutedEventArgs e)
        {
            //获取当前选中行
            ProductiveDetailed? detailed = DetailedDg.SelectedValue as ProductiveDetailed;
            //获得当前的集合

            //从集合中删除一行
            ProductiveViewModel.ProductiveModel.ShowDgDetailed.Remove(detailed);
        }

        #endregion

        /// <summary>
        /// 点击设置 一条消息绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SettingOneMessage(object sender, RoutedEventArgs e)
        {
            //获取点击当前行
            Productive? item = PruAndConsumerDataGrid.SelectedItem as Productive;
            ProductiveViewModel.ProductiveModel.ShowDgDetailed = item.MessageList;
        }
        /// <summary>
        /// 页面窗口改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProductivePage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
           
        }
    }
}