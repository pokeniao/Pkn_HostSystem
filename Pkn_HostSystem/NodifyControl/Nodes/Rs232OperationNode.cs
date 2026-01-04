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
    public partial class Rs232OperationNode : PknNode
    {

        [ObservableProperty] private Rs232OperationModel model = new();
        public Rs232OperationNode(DesignModel designModel, Object model = null) : base(designModel, $"串口通讯Rs232", NodeEnum.Rs232)
        {
            //从本地读取
            if (model != null)
            {
                JObject? jObject = model as JObject;
                Model = jObject?.ToObject<Rs232OperationModel>();
            }
            GlobalManager.NetWorkDictionary.Connect().Bind(Model.Rs232Model.NetWorkList).Subscribe();
            IModel = Model;

            Operation = new Rs232Operation(this);
            MyConnector connector = new("输入", Id, ConnectorTypeEnum.Input);
            Input.Add(connector);

            MyConnector myConnector = new("输出", Id, ConnectorTypeEnum.Output)
            {
                Value = OutputParams
            };
            Output.Add(myConnector);


            OperationModel operationModel = new()
            {
                Name = "串口返回结果" + new SnowflakeIdGenerator(1, 1).GetId().ToString(),
                ParamMethod = "常量",
                ReadOnly = true,
                NoDelete = true
            };
            //固定输出
            OutputParams.Add(operationModel);

            OperationModel operationModel1 = new()
            {
                Name = "串口返回信息" + new SnowflakeIdGenerator(1, 1).GetId().ToString(),
                ParamMethod = "常量",
                ReadOnly = true,
                NoDelete = true
            };
            //固定输出
            OutputParams.Add(operationModel1);
        }
    }
}