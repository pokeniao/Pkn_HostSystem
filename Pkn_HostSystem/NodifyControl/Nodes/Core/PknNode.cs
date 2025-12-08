using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.NodifyControl.Nodes.Connector;
using Pkn_HostSystem.NodifyControl.OperationModels.Interface;
using Pkn_HostSystem.NodifyControl.OperationModels.Models;
using Pkn_HostSystem.NodifyControl.Operations.Interface;
using System.Collections.ObjectModel;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.Nodes.Core
{
    public partial class PknNode : ObservableObject
    {

        public string Id { get; set; } = new SnowflakeIdGenerator(1, 1).GetId().ToString();

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
        [ObservableProperty] private ObservableCollectionExtended<OperationModels.Models.Core.OperationModel> inputParams = new ObservableCollectionExtended<OperationModels.Models.Core.OperationModel>();

        [ObservableProperty] private ObservableCollectionExtended<OperationModels.Models.Core.OperationModel> outputParams = new ObservableCollectionExtended<OperationModels.Models.Core.OperationModel>();

        /// <summary>
        /// 输入节点
        /// </summary>
        public ObservableCollection<MyConnector> Input { get; set; } = new ObservableCollection<MyConnector>();

        /// <summary>
        /// 输出节点
        /// </summary>
        public ObservableCollection<MyConnector> Output { get; set; } = new ObservableCollection<MyConnector>();

        public PknNode(DesignModel _designModel , string _nodeName , NodeEnum _nodeType)
        {

            DesignModel = _designModel; 
            NodeName = _nodeName;
            NodeType = _nodeType;
        }
    }
}