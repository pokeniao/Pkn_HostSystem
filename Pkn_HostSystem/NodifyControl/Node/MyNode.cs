using Azure;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.Node.Connector;
using Pkn_HostSystem.NodifyControl.Operation;
using System.Collections.ObjectModel;

using System.Windows;

namespace Pkn_HostSystem.NodifyControl.Node
{
    public partial class MyNode: ObservableObject
    {
        
        public string Id { get; set; } = new SnowflakeIdGenerator(1, 1).GetId().ToString();

        public string NodeName { get; set; }

        /// <summary>
        /// 实例化当前节点的TreeNodes
        /// </summary>
        public TreeNodes TreeNodes { get; set; }
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


        [ObservableProperty] private Point _location;

        public ObservableCollection<MyConnector> Input { get; set; } = new ObservableCollection<MyConnector>();
        public ObservableCollection<MyConnector> Output { get; set; } = new ObservableCollection<MyConnector>();
    }
}