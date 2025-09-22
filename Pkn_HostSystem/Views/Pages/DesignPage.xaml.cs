using CommunityToolkit.Mvvm.DependencyInjection;
using Nodify;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.Editor;
using Pkn_HostSystem.NodifyControl.Node;
using Pkn_HostSystem.NodifyControl.Node.DesignTreeNode;
using Pkn_HostSystem.ViewModels.Page;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Clipboard = System.Windows.Clipboard;
using DataObject = System.Windows.DataObject;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Pkn_HostSystem.Views.Pages
{
    /// <summary>
    /// DesignPage.xaml 的交互逻辑
    /// </summary>
    public partial class DesignPage : Page
    {
        public DesignViewModel ViewModel { get; set; }
        public DesignPage()
        {
            InitializeComponent();
            ViewModel = Ioc.Default.GetRequiredService<DesignViewModel>();
            DataContext = ViewModel;
            ViewModel.setSnackbarPresenter(SnackbarPresenter);
            ViewModel.setHSmartWindowControl(HSmartWindowControlWPF);
            ViewModel.SetLogRichTextBox(LogRichTextBox);
        }
        /// <summary>
        /// 放下拖拽添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodifyEditor_OnDrop(object sender, DragEventArgs e)
        {
            //如果拖拽到源是NodifyEditor,并且DataContext是EditorViewModel,并且拖拽的数据是TreeNodes类型
            if (e.Source is NodifyEditor editor && editor.DataContext is EditorViewModel editorViewModel
                                                && e.Data.GetData(typeof(TreeNodes)) is TreeNodes treeNodes)
            {
                //创建一个Node
                MyNode myNode = DesignTreeNode.GetNode(treeNodes);
                //赋值TreeNodes方便后续节点的实例化
                myNode.TreeNodes= treeNodes;
                //设置位置
                myNode.Location = editor.GetLocationInsideEditor(e);
                editorViewModel.Nodes.Add(myNode);

                e.Handled = true;
            }
        }
        /// <summary>
        /// 按住拖住
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // 找到 TreeViewItem
                var treeViewItem = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);
                // 确保找到了 TreeViewItem 并且它的数据上下文是 TreeNodes 类型,然后是叶子节点
                if (treeViewItem?.DataContext is TreeNodes treeNode && treeNode.IsLeaf)
                {
                    var data = new DataObject(typeof(TreeNodes), treeNode);

                    // 开始拖放操作, 这里使用 Copy 效果,
                    DragDrop.DoDragDrop(treeViewItem, data, DragDropEffects.Copy);
                }
            }
        }

        /// <summary>
        /// 向上查找指定类型的父元素  一个控件由多层包裹, 需要DependencyObject,控件源网上递归找到TreeViewItem
        /// </summary>
        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T target)
                    return target;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        private void DesignPage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            LogRichTextBox.MaxHeight = (e.NewSize.Height / 3) + 2;
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            LogRichTextBox.Document.Blocks.Clear();
        }

        private void CopySelected_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(LogRichTextBox.Selection.Text))
            {
                Clipboard.SetText(LogRichTextBox.Selection.Text);
            }
        }

        private void ContextMenu_OnOpened(object sender, RoutedEventArgs e)
        {
            // 判断是否有选中的文本
            CopyMenuItem.Visibility = string.IsNullOrEmpty(LogRichTextBox.Selection.Text)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }
}
