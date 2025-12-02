using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using Newtonsoft.Json;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.NodifyControl.Node.Connector;
using Pkn_HostSystem.NodifyControl.Operation.Interface;
using Pkn_HostSystem.NodifyControl.ParamOperationModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.Node.Base
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
        [ObservableProperty] private ObservableCollectionExtended<OperationParamModel> inputParams = new ObservableCollectionExtended<OperationParamModel>();


        /// <summary>
        /// 输入节点
        /// </summary>
        public ObservableCollection<MyConnector> Input { get; set; } = new ObservableCollection<MyConnector>();

        /// <summary>
        /// 输出节点
        /// </summary>
        public ObservableCollection<MyConnector> Output { get; set; } = new ObservableCollection<MyConnector>();
    }
}