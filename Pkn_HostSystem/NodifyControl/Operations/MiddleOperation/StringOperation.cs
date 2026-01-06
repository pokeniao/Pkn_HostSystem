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
            try
            {
                string method = node.Model.Method;
                string message;
                string strIndex;
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
                        message = GetParamValue(node.InputParams[0]);
                        string cut = GetParamValue(node.InputParams[1]);
                        if (string.IsNullOrEmpty(message))
                        {
                            Log.Error("分割字符message为null", $"{node.NodeName}:{node.Id}");
                            return;
                        }

                        if (string.IsNullOrEmpty(cut))
                        {
                            Log.Error("分割符Split为null", $"{node.NodeName}:{node.Id}");
                            return;
                        }

                        string[] strings = message.Split(cut);
                        for (int i = 0; i < strings.Length; i++)
                        {
                            node.OutputParams[i].ParamValue = strings[i];
                        }

                        Log.Info($" 字符串分割完成数组数量:{strings.Length}", $"{node.NodeName}:{node.Id}");
                        break;
                    case "切割":
                        message = GetParamValue(node.InputParams[0]);
                        if (int.TryParse(GetParamValue(node.InputParams[1]), out int cutStart) &&
                            int.TryParse(GetParamValue(node.InputParams[2]), out int cutNum))
                        {
                            string substring = message.Substring(cutStart,
                                cutNum);
                            //添加到输出
                            node.OutputParams[0].ParamValue = substring;
                            Log.Info($" 字符串进行切割结果:{substring}", $"{node.NodeName}:{node.Id}");
                        }
                        else
                        {
                            Log.Error("切割起始地址或数量不为Int类型", $"{node.NodeName}:{node.Id}");
                            break;
                        }

                        break;
                    case "索引":
                        message = GetParamValue(node.InputParams[0]);
                        strIndex = GetParamValue(node.InputParams[1]);
                        int indexOf = message.IndexOf(strIndex);
                        //添加到输出
                        node.OutputParams[0].ParamValue = indexOf.ToString();
                        break;
                    case "索引(倒序)":
                        message = GetParamValue(node.InputParams[0]);
                        strIndex = GetParamValue(node.InputParams[1]);
                        int lastIndexOf = message.LastIndexOf(strIndex);
                        //添加到输出
                        node.OutputParams[0].ParamValue = lastIndexOf.ToString();
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, $"{node.NodeName}:{node.Id}");
            }
        }
    }
}