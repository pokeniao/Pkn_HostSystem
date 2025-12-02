using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.NodifyControl.LocalSave;
using Pkn_HostSystem.NodifyControl.Node.Base;
using Pkn_HostSystem.NodifyControl.Node.Connector;
using Pkn_HostSystem.NodifyControl.Operation.StartOperation;
using Pkn_HostSystem.NodifyControl.ParamOperationModel;

namespace Pkn_HostSystem.NodifyControl.Node
{
    public partial class EnterOperationNode : PknNode
    {
        [ObservableProperty] private EnterParamOperationModel model = new EnterParamOperationModel();

        public EnterOperationNode(DesignModel designModel,Object model = null)
        {
            //从本地读取
            if (model!=null)
            {
                JObject? jObject = model as JObject;
                model = jObject?.ToObject<EnterParamOperationModel>();
            }

            IModel = Model;
            DesignModel = designModel;
            NodeName = "Start节点";
            NodeType = NodeEnum.Enter;
            Operation = new EnterOperation(this);
            MyConnector myConnector = new MyConnector("输出", Id, ConnectorTypeEnum.Output);
            myConnector.Value = OutputParams;

            Output.Add(myConnector);
        }

    }
}