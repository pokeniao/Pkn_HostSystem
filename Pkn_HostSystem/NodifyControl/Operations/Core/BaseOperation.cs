using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.NodifyControl.Nodes.Core;
using Pkn_HostSystem.NodifyControl.OperationModels.Models.Core;
using Pkn_HostSystem.NodifyControl.Operations.Interface;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.Operations.Core
{
    public abstract class BaseOperation<TNode>(TNode node, FrameworkElement view) : IOperation
    {
        protected readonly LogControl<DesignModel> Log = (node as PknNode)?.DesignModel.Log ?? throw new ArgumentException("Node 必须是 PknNode 派生类");
        public TNode node { get; } = node;


        public FrameworkElement view { get; } = view;
        /// <summary>
        /// 动态遍历 更具动态名 获取动态值
        /// </summary>
        public string GetParamValue(OperationModel operationModel)
        {
            if (operationModel == null)
            {
                return "";
            }

            if (operationModel.ParamMethod.Equals("动态获取"))
            {
                return GetParamValue(operationModel.Dyn);
            }
            else
            {
                return operationModel.ParamValue;
            }
        }

        public async Task Execute()
        {
            await OnExecute();
        }

        protected abstract  Task OnExecute();

        public FrameworkElement GetConfigView()
        {
            view.DataContext = node;
            return view;
        }
    }
}