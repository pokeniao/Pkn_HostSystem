using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.NodifyControl.Nodes;
using Pkn_HostSystem.NodifyControl.Operations.Core;
using Pkn_HostSystem.NodifyControl.Views.NodeOperation;

namespace Pkn_HostSystem.NodifyControl.Operations.MiddleOperation
{
    public class JsonProcessingOperation(JsonOperationNode node)
        : BaseOperation<JsonOperationNode>(node, new JsonProcessingUserControl())
    {

        protected override async Task OnExecute(CancellationTokenSource cts)
        {

            try
            {
                string jsonValue = GetParamValue(node.InputParams[0]);
                string paramValue = GetParamValue(node.InputParams[1]);
                string tryFormatJson = JsonTool<Object>.TryFormatJson(jsonValue, out bool isJson);
                if (!isJson)
                {
                    node.OutputParams[0].ParamValue = "false";
                    Log.Error("传入不为json格式", $"{node.NodeName}:{node.Id}");
                    return;
                }
                JObject obj = JObject.Parse(tryFormatJson);
                switch (node.Model.JsonMethod)
                {
                    case "路径解析":
                        var jToken = obj.SelectToken(paramValue);
                        node.OutputParams[0].ParamValue = "true";
                        node.OutputParams[1].ParamValue = jToken.ToString();
                        Log.Info($"解析返回:{jToken}", $"{node.NodeName}:{node.Id}");
                        break;
                    case "解析数组":
                        JArray? jArray = obj[paramValue] as JArray;
                        node.OutputParams[0].ParamValue = "true";
                        string count = jArray.Count.ToString();
                        node.OutputParams[1].ParamValue = count;

                        Log.Info($"解析数组数量返回:{count}", $"{node.NodeName}:{node.Id}");

                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, $"{node.NodeName}:{node.Id}");
                node.OutputParams[0].ParamValue = "false";
                return;
            }

        }
    }
}