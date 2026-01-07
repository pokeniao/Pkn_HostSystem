using CommunityToolkit.Mvvm.ComponentModel;
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
    public partial class HttpOperationNode : PknNode
    {
        [ObservableProperty] private HttpOperationModel model = new HttpOperationModel();
        public HttpOperationNode(DesignModel _designModel, Object model) : base(_designModel, "Http请求", NodeEnum.Http)
        {
            //从本地读取
            if (model != null)
            {
                JObject? jObject = model as JObject;
                Model = jObject?.ToObject<HttpOperationModel>();
            }

            IModel = Model;
            Operation = new HttpOperation(this);
            MyConnector connector = new("输入", Id, ConnectorTypeEnum.Input);
            Input.Add(connector);

            MyConnector myConnector = new("输出", Id, ConnectorTypeEnum.Output)
            {
                Value = OutputParams
            };
            Output.Add(myConnector);


            OperationModel operationModel = new()
            {
                Name = "Http请求结果"+GlobalManager.SnowflakeId.GetId().ToString(),
                ParamMethod = "常量",
                NameReadOnly = true,
                ValueReadOnly = true,
                MethodReadOnly = true,
                NoDelete = true
            };
            //固定输出
            OutputParams.Add(operationModel);

            OperationModel operationModel2 = new()
            {
                Name = "请求返回结果" + GlobalManager.SnowflakeId.GetId().ToString(),
                ParamMethod = "常量",
                NameReadOnly = true,
                ValueReadOnly = true,
                MethodReadOnly = true,
                NoDelete = true
            };
            //固定输出
            OutputParams.Add(operationModel2);
        }
    }
}