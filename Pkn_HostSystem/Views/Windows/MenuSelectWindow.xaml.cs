using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.ViewModels.Page;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// MenuSelectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MenuSelectWindow
    {
        public MenuSelectModel Model { get; set; } = new MenuSelectModel();

        public MenuSelectWindow(ObservableCollection<string> projectList)
        {
            Model.ProjectList = projectList;
            InitializeComponent();
            DataContext = Model;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
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
        /// <summary>
        /// 确认按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
            string? s = ListBox.SelectedValue.ToString();

            if (!string.IsNullOrEmpty(s))
            {
                var designViewModel = Ioc.Default.GetRequiredService<DesignViewModel>();

                designViewModel.DesignModel.ProjectName = s;
                this.Close();
            }
        }
        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ButtonBase_OnClick3(object sender, RoutedEventArgs e)
        {

            bool? showDialog = new AddProjectWindow(Model.ProjectList).ShowDialog();

        }
    }
}
