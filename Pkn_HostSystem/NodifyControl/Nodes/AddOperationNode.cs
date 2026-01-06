
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.NodifyControl.Nodes.Connector;
using Pkn_HostSystem.NodifyControl.Nodes.Core;
using Pkn_HostSystem.NodifyControl.OperationModels.Models.Core;
using Pkn_HostSystem.NodifyControl.Operations.MiddleOperation;

namespace Pkn_HostSystem.NodifyControl.Nodes
{
    public partial class AddOperationNode : PknNode
    {
        public AddOperationNode(DesignModel designModel,Object model = null) : base(designModel, "加法节点", NodeEnum.Add)
        {
            Operation = new AddOperation(this);

            MyConnector connector = new("输入", Id, ConnectorTypeEnum.Input);
            Input.Add(connector);

            MyConnector myConnector = new("输出", Id, ConnectorTypeEnum.Output)
            {
                Value = OutputParams
            };
            Output.Add(myConnector);
            OperationModel operationModel = new()
            {
                Name = "加法结果"+new SnowflakeIdGenerator(1, 1).GetId().ToString(),
                ParamMethod = "常量",
                NameReadOnly = true,
                ValueReadOnly = true,
                MethodReadOnly = true,
                NoDelete = true
            };
            //固定输出
            OutputParams.Add(operationModel);
        }
    }
}