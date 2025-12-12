using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.NodifyControl.Nodes.Connector;
using Pkn_HostSystem.NodifyControl.Nodes.Core;
using Pkn_HostSystem.NodifyControl.OperationModels.Models;
using Pkn_HostSystem.NodifyControl.Operations.MiddleOperation;

namespace Pkn_HostSystem.NodifyControl.Nodes
{
    public partial class IfOperationNode : PknNode
    {

        [ObservableProperty] private IfOperationModel model = new();
        public IfOperationNode(DesignModel _designModel, Object model = null) : base(_designModel, "If", NodeEnum.If)
        {
            Operation = new IfOperation(this);
            MyConnector connector = new("输入", Id, ConnectorTypeEnum.Input);
            Input.Add(connector);

            MyConnector myConnector = new("True", Id, ConnectorTypeEnum.Output )
            {
                Value = OutputParams
            };
            Output.Add(myConnector);

            MyConnector myConnector2 = new("False", Id, ConnectorTypeEnum.Output)
            {
                Value = OutputParams
            };
            Output.Add(myConnector2);
        }
    }
}