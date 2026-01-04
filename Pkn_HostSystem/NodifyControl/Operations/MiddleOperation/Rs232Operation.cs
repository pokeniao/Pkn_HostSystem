using DynamicData.Binding;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.Nodes;
using Pkn_HostSystem.NodifyControl.OperationModels.Models.Core;
using Pkn_HostSystem.NodifyControl.Operations.Core;
using Pkn_HostSystem.NodifyControl.Views.NodeOperation;
using Pkn_HostSystem.Static;
using System.Text.RegularExpressions;

namespace Pkn_HostSystem.NodifyControl.Operations.MiddleOperation
{
    public class Rs232Operation(Rs232OperationNode node)
        : BaseOperation<Rs232OperationNode>(node, new Rs232OperationUserControl())
    {
        protected override async Task OnExecute(CancellationTokenSource cts)
        {
            NetWork netWork = GlobalManager.GetNetWork(node.Model.Rs232Model.NetworkName);
            if (netWork == null)
            {
                Log.Error("通讯未连接,未找到netWork", $"{node.NodeName}:{node.Id}");
                return;
            }

            ScpiSerialTool scpiSerialTool = netWork.ScpiSerialTool;
            bool succeed = false;
            string response = null;

            if (string.IsNullOrEmpty(node.Model.Rs232Model.NetMethodName))
            {
                Log.Error("通讯方式为NULL", $"{node.NodeName}:{node.Id}");
                return;
            }

            switch (node.Model.Rs232Model.NetMethodName)
            {
                case "发送并等待读取":
                    string dynMessage = DynMessage(node.Model.Rs232Model.SendMessage);
                    (succeed, response) = await scpiSerialTool.WriteLineAndWaitResponse(dynMessage);
                    break;
                case "读取":
                    (succeed, response) = await scpiSerialTool.ReadLine();
                    break;
                case "发送(携带结束符)":
                    (succeed, response) = await scpiSerialTool.WriteLine(response);
                    break;
                case "发送(不带结束符)":
                    (succeed, response) = await scpiSerialTool.Write(response);
                    break;
            }

            if (!succeed)
            {
                node.OutputParams[0].ParamValue = "False";
                node.OutputParams[1].ParamValue = response;
                Log.Error($"串口WriteLineAndWaitResponse执行返回失败.错误信息: {response}", $"{node.NodeName}:{node.Id}");
                return;
            }

            node.OutputParams[0].ParamValue = "True";
            node.OutputParams[1].ParamValue = response;
        }


        private string DynMessage(string message)
        {
            if (message == null)
            {
                return "";
            }

            //获取输入
            ObservableCollectionExtended<OperationModel> inputParams = node.InputParams;
            //通过正则表达式匹配对应数量的[]格式的字符 , 为了让顺序属于按[]出现的顺序来处理
            MatchCollection matches = Regex.Matches(message, @"\[.*?\]");
            foreach (Match match in matches)
            {
                //获取到匹配的内容
                foreach (var operationModel in inputParams)
                {
                    var itemKey = operationModel.Name;
                    //检查是否存在
                    var i = match.Value.IndexOf($"[{itemKey}]");

                    if (i == -1)
                    {
                        continue;
                    }

                    message = StaticMessage(message, itemKey, GetParamValue(operationModel));
                }
            }

            return message;
        }


        private string StaticMessage(string request, string itemKey, string itemValue)
        {
            var i = request.IndexOf($"[{itemKey}]");

            string messageBefore = request;
            if (i != -1)
            {
                var keyLen = itemKey.Length;
                var requestA = request.Substring(0, i);
                var requestB = request.Substring(i + keyLen + 2);
                request = requestA + itemValue + requestB;
            }

            //防止堆栈溢出,重复嵌套调用
            if (request == messageBefore)
            {
                Log.Error($"[{TraceContext.Name}]--进行嵌入后,前后一样,避免循环嵌套堆栈溢出,退出嵌套");
                return request;
            }

            return request.IndexOf($"[{itemKey}]") != -1 ? StaticMessage(request, itemKey, itemValue) : request;
        }
    }
}