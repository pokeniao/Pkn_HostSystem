using Pkn_HostSystem.NodifyControl.Nodes;
using Pkn_HostSystem.NodifyControl.OperationModels.Models.Core;
using Pkn_HostSystem.NodifyControl.Operations.Core;
using Pkn_HostSystem.NodifyControl.Views.NodeOperation;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.Operations.MiddleOperation
{
    public class AddOperation : BaseOperation<AddOperationNode>
    {

        public AddOperation(AddOperationNode _node) : base(_node, new AddOperationUserControl()) { }


        protected override async Task OnExecute(CancellationTokenSource cts)
        {
            //获取参数
            var Params = node.InputParams;
            double sum = 0;
            double a = 0;
            foreach (OperationModel operationParam in Params)
            {

                bool tryParse = double.TryParse(GetParamValue(operationParam), out a);
                if (!tryParse)
                {
                    a = 0;
                }
                sum += a;
            }

            //添加到输出
            var outputParams = node.OutputParams;
            outputParams[0].ParamValue = sum.ToString();
            Log.Info($" 计算结果:{sum}",$"{node.NodeName}:{node.Id}");
        }
    }
}