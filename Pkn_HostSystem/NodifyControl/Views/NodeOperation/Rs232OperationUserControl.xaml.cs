using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Static;
using System.Windows;
using System.Windows.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;
namespace Pkn_HostSystem.NodifyControl.Views.NodeOperation
{
    /// <summary>
    /// Rs232OperationUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class Rs232OperationUserControl : UserControl
    {
        public Rs232OperationUserControl()
        {
            InitializeComponent();
        }

        private async void NetNameComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NetNameComboBox.SelectedIndex == -1)
            {
                return;
            }

            if (NetNameComboBox.SelectedValue == null)
            {
                return;
            }

            string? NetName = NetNameComboBox.SelectedValue.ToString();
            NetWork netWork = GlobalManager.GetNetWork(NetName);

            if (netWork == null)
            {
                NetNameComboBox.SelectedIndex = -1;
                await new MessageBox() { Content = "选中通讯不为串口232/485,或通讯处于关闭" }.ShowDialogAsync();
                return;
            }

            if (!(netWork.NetworkDetailed.NetMethod == "串口232/485"))
            {
                NetNameComboBox.SelectedIndex = -1;
                await new MessageBox() { Content = "选中通讯不为串口232/485" }.ShowDialogAsync();
                return;
            }
        }
    }
}
