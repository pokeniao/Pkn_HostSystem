using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.NodifyControl.Nodes.Connector;
using Pkn_HostSystem.NodifyControl.Nodes.Core;
using Pkn_HostSystem.NodifyControl.OperationModels.Models;
using Pkn_HostSystem.NodifyControl.OperationModels.Models.Core;
using Pkn_HostSystem.NodifyControl.Operations.MiddleOperation;

namespace Pkn_HostSystem.NodifyControl.Nodes
{
    public partial class SwitchOperationNode: PknNode
    {

        [ObservableProperty] private SwitchOperationModel model = new();
        public SwitchOperationNode(DesignModel designModel, Object model = null) : base(designModel, "Switch", NodeEnum.Switch)
        {

            //从本地读取
            if (model != null)
            {
                JObject? jObject = model as JObject;
                Model = jObject?.ToObject<SwitchOperationModel>();
            }

            IModel = Model;
            Operation = new SwitchOperation(this);

            MyConnector connector = new("输入", Id, ConnectorTypeEnum.Input);
            Input.Add(connector);


            InputParams.Add(new  OperationModel()
            {
                Name="swtich字符串",
                NoDelete = true,
                NameReadOnly = true,
            });
            MyConnector connector2 = new("default", Id, ConnectorTypeEnum.Output);
            Output.Add(connector2);
        }
    }
}