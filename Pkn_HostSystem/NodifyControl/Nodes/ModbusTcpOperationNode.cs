using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.NodifyControl.Nodes.Connector;
using Pkn_HostSystem.NodifyControl.Nodes.Core;
using Pkn_HostSystem.NodifyControl.OperationModels.Models;
using Pkn_HostSystem.NodifyControl.OperationModels.Models.Core;
using Pkn_HostSystem.NodifyControl.Operations.MiddleOperation;
using Pkn_HostSystem.Static;

namespace Pkn_HostSystem.NodifyControl.Nodes
{
    public partial class ModbusTcpOperationNode :PknNode
    {
        [ObservableProperty] private ModbusTcpOperationModel model = new();
        public ModbusTcpOperationNode(DesignModel designModel, Object model = null):base(designModel,"ModbusTcp通讯",NodeEnum.ModbusTcp)
        {
            //从本地读取
            if (model != null)
            {
                JObject? jObject = model as JObject;
                Model = jObject?.ToObject<ModbusTcpOperationModel>();
            }
            GlobalManager.NetWorkDictionary.Connect().Bind(Model.NetWorkTriggerModel.NetWorkList).Subscribe();
            IModel = Model;

            Operation = new ModbusTcpOperation(this);

            MyConnector connector = new("输入", Id, ConnectorTypeEnum.Input);
            Input.Add(connector);


            MyConnector myConnector = new("输出", Id, ConnectorTypeEnum.Output);
            myConnector.Value = OutputParams;
            Output.Add(myConnector);
        }
    }
}