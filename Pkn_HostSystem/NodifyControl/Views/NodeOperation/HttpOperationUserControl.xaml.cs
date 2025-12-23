using DynamicData;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.Nodes;
using System.Windows;
using System.Windows.Controls;

namespace Pkn_HostSystem.NodifyControl.Views
{
    /// <summary>
    /// HttpOperationUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class HttpOperationUserControl : UserControl
    {
        public HttpOperationUserControl()
        {
            InitializeComponent();
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            HttpItem? HttpItem = HttpHeaderDataGrid.SelectedValue as HttpItem;
            if (HttpItem == null) return;

            string name = HttpItem.Key;
            if (name != null)
            {
                HttpOperationNode? httpOperationNode = DataContext as HttpOperationNode;
                bool remove = httpOperationNode.Model.HttpHeaders.Remove(HttpItem);
            }
        }
        private void MenuItem_OnClick2(object sender, RoutedEventArgs e)
        {
            HttpItem? HttpItem = FormDataGrid.SelectedValue as HttpItem;
            if (HttpItem == null) return;

            string name = HttpItem.Key;
            if (name != null)
            {
                HttpOperationNode? httpOperationNode = DataContext as HttpOperationNode;
                bool remove = httpOperationNode.Model.FromBodys.Remove(HttpItem);
            }
        }
    }
}
