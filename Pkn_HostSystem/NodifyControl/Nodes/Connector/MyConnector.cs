using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.NodifyControl.OperationModels.Models.Core;
using System.Collections.ObjectModel;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.Nodes.Connector
{
    public partial class MyConnector : ObservableObject
    {
        public MyConnector(string Name, string nodeId, ConnectorTypeEnum connectorType)
        {
            ConnectorName = Name;
            NodeId = nodeId;
            ConnectorType = connectorType;


            
        }
        public string Id = new SnowflakeIdGenerator(1, 1).GetId().ToString();

        /// <summary>
        /// 定义一个测试连接器的名称
        /// </summary>
        [ObservableProperty] private string connectorName;

        /// <summary>
        /// 当前连接器所属的节点ID
        /// </summary>
        public string NodeId { get; set; }

        public ConnectorTypeEnum ConnectorType { get; set; }


        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 节点的所有输入
        /// </summary>
        public List<ObservableCollection<OperationModel>> _inputValue = new();

        public List<ObservableCollection<OperationModel>> InputValue
        {
            get => _inputValue;
            set
            {
                SetProperty(ref _inputValue, value);
            }
        }

        /// <summary>
        /// 节点的输出
        /// </summary>
        private ObservableCollectionExtended<OperationModel> _value;

        public ObservableCollectionExtended<OperationModel> Value
        {
            get => _value;
            set
            {
                SetProperty(ref _value, value);
            }
        }


        /// <summary>
        /// 用于存储所有连接到该连接器的连接器
        /// </summary>
        public List<MyConnector> ValueObservers { get; set; } = new List<MyConnector>();

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