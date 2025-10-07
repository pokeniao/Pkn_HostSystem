using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using Nodify;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.Node.Base;
using Pkn_HostSystem.NodifyControl.Node.Connector;
using Pkn_HostSystem.NodifyControl.Operation.MiddleOperation;

namespace Pkn_HostSystem.NodifyControl.Node
{
    public partial class AddOperationNode :PknNode
    {
        /// <summary>
        /// 输出
        /// </summary>
        [ObservableProperty] private ObservableCollectionExtended<OperationParam> outputParams = new ObservableCollectionExtended<OperationParam>();

        public AddOperationNode()
        {
            NodeName = "加法节点";
            NodeType = NodeEnum.Add;
            Operation = new AddOperation(this);

            MyConnector connector = new("输入", Id, ConnectorTypeEnum.Input);
            Input.Add(connector);


            MyConnector myConnector = new("输出", Id, ConnectorTypeEnum.Output);
            myConnector.Value = outputParams;
            Output.Add(myConnector);

            OperationParam operationParam = new()
            {
                Name = new SnowflakeIdGenerator(1,1).GetId().ToString(),
                ParamMethod = "常量",
                IsEnable = false,
                NoDelete = true
            };

            //固定输出
            OutputParams.Add(operationParam);
        }
    }
}