using Azure;
using CommunityToolkit.Mvvm.DependencyInjection;
using Nodify;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.Editor;
using Pkn_HostSystem.NodifyControl.Node;
using Pkn_HostSystem.NodifyControl.Node.Connector;
using Pkn_HostSystem.NodifyControl.Node.DesignTreeNode;
using Pkn_HostSystem.ViewModels.Page;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using DataObject = System.Windows.DataObject;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using RichTextBox = System.Windows.Controls.RichTextBox;

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
            
        private void NodifyEditor_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Source is NodifyEditor editor && editor.DataContext is EditorViewModel editorViewModel
                                                && e.Data.GetData(typeof(TreeNodes)) is TreeNodes treeNodes)
            {
                //创建一个Node
                MyNode myNode = DesignTreeNode.GetNode(treeNodes);
                myNode.Location = editor.GetLocationInsideEditor(e);
                editorViewModel.Nodes.Add(myNode);
                
                e.Handled = true;
            }
        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed )
            {
                // 找到 TreeViewItem
                var treeViewItem = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);

                if (treeViewItem?.DataContext is TreeNodes treeNode && treeNode.IsLeaf)
                {
                    var data = new DataObject(typeof(TreeNodes), treeNode);
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

    }
}
