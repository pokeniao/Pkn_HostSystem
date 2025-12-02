using CommunityToolkit.Mvvm.DependencyInjection;
using DynamicData.Binding;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.NodifyControl.Node;
using Pkn_HostSystem.NodifyControl.Operation.Base;
using Pkn_HostSystem.NodifyControl.Operation.Interface;
using Pkn_HostSystem.NodifyControl.ParamOperationModel;
using Pkn_HostSystem.ViewModels.Page;
using System.Windows;
using System.Windows.Documents;

namespace Pkn_HostSystem.NodifyControl.Operation.MiddleOperation
{
    public class AddOperation : BaseOperation ,IOperation
    {
        private readonly Action _func;


        private AddOperationNode node;

        //创建一个Log ,
        private LogControl<DesignModel> Log ;

        public AddOperation(AddOperationNode _node)
        {
            node = _node;
            _func = Func;
            Log = _node.DesignModel.Log;
        }


        private void Func()
        {
        
            //获取参数
            var Params = node.InputParams;
            double sum = 0;
            double a = 0;
            foreach (OperationParamModel operationParam in Params)
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

      



        public void Execute() => _func.Invoke();

        public FrameworkElement GetConfigView()
        {
            var view = new Views.UserControls.NodeOperation.AddOperationUserControl();
            view.DataContext = node;
            return view;
        }
    }
}