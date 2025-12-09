using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.NodifyControl.Nodes;
using Pkn_HostSystem.NodifyControl.Operations.Core;
using Pkn_HostSystem.NodifyControl.Operations.Interface;
using Pkn_HostSystem.NodifyControl.Views.NodeOperation;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.Operations.MiddleOperation
{
    public class AddOperation : BaseOperation<AddOperationNode> 
    {

        public AddOperation(AddOperationNode _node):base(_node, new AddOperationUserControl()) { }


        protected override async Task OnExecute()
        {
            //获取参数
            var Params = node.InputParams;
            double sum = 0;
            double a = 0;
            foreach (OperationModels.Models.Core.OperationModel operationParam in Params)
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
            Log.Info($"计算结果:{sum}");
        }

        public override FrameworkElement GetConfigView()
        {
            view.DataContext = node;
            return view;
        }


    }
}