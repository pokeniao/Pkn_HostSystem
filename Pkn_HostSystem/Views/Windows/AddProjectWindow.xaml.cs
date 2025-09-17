using DynamicData;
using DynamicData.Kernel;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Static;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Pkn_HostSystem.Views.Windows
{
    /// <summary>
    /// AddProjectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddProjectWindow
    {
        public string ProjectName { get; set; }

        public AddProjectWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(ProjectName))
            {
                return;
            }

            bool hasValue = GlobalManager.ProjectDictionary.Lookup(ProjectName).HasValue;
            if (!hasValue)
            {
                DesignModel designModel = new DesignModel { ProjectName = ProjectName };
                GlobalManager.ProjectDictionary.AddOrUpdate(designModel);
            }
            this.Close();
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
