using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.Node.Base;
using Pkn_HostSystem.NodifyControl.Node.Connector;
using Pkn_HostSystem.NodifyControl.Operation.StartOperation;
using Pkn_HostSystem.NodifyControl.ParamOperationModel;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.NodifyControl.Node
{
    public partial class EnterOperationNode : PknNode
    {
        [ObservableProperty]
        private ObservableCollectionExtended<OperationParamModel> outputParams = new ObservableCollectionExtended<OperationParamModel>();

        [ObservableProperty]
        private EnterParamOperationModel model = new EnterParamOperationModel();

        public EnterOperationNode()
        {
            NodeName = "Start节点";
            NodeType = NodeEnum.Enter;
            Operation = new EnterOperation(this);
            MyConnector myConnector = new MyConnector("输出", Id, ConnectorTypeEnum.Output);
            myConnector.Value = outputParams;

            Output.Add(myConnector);
            
        }
    }
}