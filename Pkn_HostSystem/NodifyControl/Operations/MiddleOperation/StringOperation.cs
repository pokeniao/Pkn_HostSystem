using Pkn_HostSystem.NodifyControl.Nodes;
using Pkn_HostSystem.NodifyControl.OperationModels.Models.Core;
using Pkn_HostSystem.NodifyControl.Operations.Core;
using Pkn_HostSystem.NodifyControl.Views.NodeOperation;
using System.Text;

namespace Pkn_HostSystem.NodifyControl.Operations.MiddleOperation
{
    public class StringOperation(StringOperationNode node)
        : BaseOperation<StringOperationNode>(node, new StringOperationUserControl())
    {
        protected override async Task OnExecute(CancellationTokenSource cts)
        {
            string method = node.Model.Method;

            switch (method)
            {
                case "拼接":
                    //获取参数
                    var Params = node.InputParams;
                    string stringCombination = "";

                    foreach (OperationModel operationParam in Params)
                    {
                        string paramValue = GetParamValue(operationParam);
                        stringCombination += paramValue;
                    }

                    //添加到输出
                    var outputParams = node.OutputParams;
                    outputParams[0].ParamValue = stringCombination;
                    Log.Info($" 字符串进行拼接结果:{stringCombination}", $"{node.NodeName}:{node.Id}");
                    break;
                case "分割":
                    break;
                case "切割":
                    break;
            }
        }
    }
}