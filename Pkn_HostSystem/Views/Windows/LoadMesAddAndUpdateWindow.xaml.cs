using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Pkn_HostSystem.Pojo.Windows.LoadMesAddAndUpdateWindow;
using Pkn_HostSystem.ViewModels.Windows;
using LoadMesAddAndUpdateWindowModel = Pkn_HostSystem.Models.Windows.LoadMesAddAndUpdateWindowModel;

namespace Pkn_HostSystem.Views.Windows
{
    /// <summary>
    /// LoadMesAddWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoadMesAddWindow
    {
        public LoadMesAddAndUpdateWindowsViewModel viewModel { get; set; }

      
        public LoadMesAddWindow()
        {
            InitializeComponent();
            
        }
        //添加
        public LoadMesAddWindow(string title, ObservableCollection<LoadMesAddAndUpdateWindowModel> mesPojoList) :this()
        {
            DataContext = new LoadMesAddAndUpdateWindowsViewModel();
            Title.Text = title;
            viewModel = (LoadMesAddAndUpdateWindowsViewModel)DataContext;
            viewModel.setSnackbarService(SnackbarPresenter);
            viewModel.mesPojoList = mesPojoList;
        }
        //修改
        public LoadMesAddWindow(string title, LoadMesAddAndUpdateWindowModel item, ObservableCollection<LoadMesAddAndUpdateWindowModel> mesPojoList) :this()
        {
            DataContext = new LoadMesAddAndUpdateWindowsViewModel(item);
            Title.Text = title;
            LoadMesAddAndUpdateWindowsViewModel viewModel = (LoadMesAddAndUpdateWindowsViewModel)DataContext;
            viewModel.setSnackbarService(SnackbarPresenter);
            viewModel.mesPojoList = mesPojoList;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            LoadMesCondition? item = ReqDataGrid.SelectedItem as LoadMesCondition;

            viewModel = (LoadMesAddAndUpdateWindowsViewModel)DataContext;

            ObservableCollection<LoadMesCondition>? items = viewModel?.LoadMesAddAndUpdateWindowModel.Condition;

            if (item?.Key != null)
            {
                //从集合中移除
                items?.Remove(item);
                viewModel?.Log.SuccessAndShow("删除一个条件",$"Mes请求{HTTP_Name.Text} ,{item.Key}条件被删掉");
            }
           
        }
    }
}
