using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.Node.Connector
{
    public partial class Connector : ObservableObject
    {
        /// <summary>
        /// 定义一个测试连接器的名称
        /// </summary>
        public string ConnectorName { get; set; }

        /// <summary>
        /// 我们拖动节点时连接端子需要跟随移动，连接线也应随之改变，所以需要记录其位置锚点，在xaml中会将其绑定到节点的依赖属性Anchor上
        /// </summary>
        [ObservableProperty] private Point _anchor;

        [ObservableProperty] private bool _isConnected;

        /// <summary>
        /// 线大小
        /// </summary>
        [ObservableProperty] private Size _size;
    }
}