using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using Newtonsoft.Json;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.NodifyControl.Nodes.Connector;
using Pkn_HostSystem.NodifyControl.OperationModels.Interface;
using Pkn_HostSystem.NodifyControl.OperationModels.Models.Core;
using Pkn_HostSystem.NodifyControl.Operations.Interface;
using Pkn_HostSystem.Static;
using System.Collections.ObjectModel;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.Nodes.Core
{
    public partial class PknNode : ObservableObject
    {

        public string Id { get; set; } = GlobalManager.SnowflakeId.GetId().ToString();

        public string NodeName { get; set; }

        /// <summary>
        /// Node的类型
        /// </summary>
        public NodeEnum NodeType { get; set; }
        /// <summary>
        /// 当前节点的Model
        /// </summary>
        public IOperationModel IModel { get; set; }
        /// <summary>
        /// 运行的方法
        /// </summary>
        private IOperation? _operation;
        [JsonIgnore]
        public IOperation? Operation
        {
            get => _operation;
            set => SetProperty(ref _operation, value);

        }
        /// <summary>
        /// 位置
        /// </summary>
        [ObservableProperty] private Point _location;

        /// <summary>
        /// 节点所在设计页面的Model
        /// </summary>
        public DesignModel DesignModel { get; set; }

        /// <summary>
        /// 输入参数
        /// </summary>
        [ObservableProperty] private ObservableCollectionExtended<OperationModel> inputParams = new();

        [ObservableProperty] private ObservableCollectionExtended<OperationModel> outputParams = new();

        /// <summary>
        /// 输入节点
        /// </summary>
        public ObservableCollection<MyConnector> Input { get; set; } = new();

        /// <summary>
        /// 输出节点
        /// </summary>
        public ObservableCollection<MyConnector> Output { get; set; } = new();

        public PknNode(DesignModel _designModel, string _nodeName, NodeEnum _nodeType)
        {
            DesignModel = _designModel;
            NodeName = _nodeName; 
            NodeType = _nodeType;
        }
    }
}