using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.NodifyControl.Nodes.Connector;
using Pkn_HostSystem.NodifyControl.Nodes.Core;
using Pkn_HostSystem.NodifyControl.OperationModels.Models;
using Pkn_HostSystem.NodifyControl.Operations.StartOperation;
using Pkn_HostSystem.Static;

namespace Pkn_HostSystem.NodifyControl.Nodes
{
    public partial class EnterOperationNode : PknNode
    {
        [ObservableProperty] private EnterOperationModel model = new EnterOperationModel();

        public EnterOperationNode(DesignModel designModel, Object model = null) : base(designModel, "Start节点", NodeEnum.Enter)
        {
            //从本地读取
            if (model != null)
            {
                JObject? jObject = model as JObject;
                Model = jObject?.ToObject<EnterOperationModel>();
            }

            GlobalManager.NetWorkDictionary.Connect().Bind(Model.NetWorkTriggerModel.NetWorkList).Subscribe();
            IModel = Model;

            Operation = new EnterOperation(this);

            MyConnector myConnector = new("输出", Id, ConnectorTypeEnum.Output)
            {
                Value = OutputParams
            };
            Output.Add(myConnector);
        }

    }
}