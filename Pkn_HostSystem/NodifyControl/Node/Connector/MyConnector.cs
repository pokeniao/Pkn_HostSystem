using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.NodifyControl.Connection;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.Node.Connector
{
    public partial class MyConnector : ObservableObject
    {
        /// <summary>
        /// 定义一个测试连接器的名称
        /// </summary>
        public string ConnectorName { get; set; }

        /// <summary>
        /// 接口值
        /// </summary>
        private object _value;

        public object Value
        {
            get => _value;
            set
            {
                SetProperty(ref _value, value);
                //将值传递给连接到该连接器的所有连接器
                ValueObservers.ForEach(o => o.Value = value);
            }

        }

        /// <summary>
        /// 用于存储所有连接到该连接器的连接器
        /// </summary>
        public List<MyConnector> ValueObservers { get; } = new List<MyConnector>();
        /// <summary>
        /// 我们拖动节点时连接端子需要跟随移动，连接线也应随之改变，所以需要记录其位置锚点，在xaml中会将其绑定到节点的依赖属性Anchor上
        /// </summary>
        [ObservableProperty] private Point _anchor;

        [ObservableProperty] private bool _isConnected;

        /// <summary>
        /// 线大小
        /// </summary>
        [ObservableProperty] private Size _size ;
    }
}