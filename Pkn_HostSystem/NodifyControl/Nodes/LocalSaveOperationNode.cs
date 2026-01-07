using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.NodifyControl.Nodes.Connector;
using Pkn_HostSystem.NodifyControl.Nodes.Core;
using Pkn_HostSystem.NodifyControl.OperationModels.Models;
using Pkn_HostSystem.NodifyControl.Operations.MiddleOperation;

namespace Pkn_HostSystem.NodifyControl.Nodes
{
    public partial class LocalSaveOperationNode:PknNode
    {

        [ObservableProperty] private LocalSaveOperationModel model;
        public LocalSaveOperationNode(DesignModel designModel, Object model = null) : base(designModel, "本地保存", NodeEnum.LocalSave)
        {
            //从本地读取
            if (model != null)
            {
                JObject? jObject = model as JObject;
                Model = jObject?.ToObject<LocalSaveOperationModel>();
            }

            IModel = Model;
            Operation = new LocalSaveOperation(this);

            MyConnector connector = new("输入", Id, ConnectorTypeEnum.Input);
            Input.Add(connector);

            MyConnector myConnector = new("输出", Id, ConnectorTypeEnum.Output)
            {
                Value = OutputParams
            };
            Output.Add(myConnector);
        }
    }
}