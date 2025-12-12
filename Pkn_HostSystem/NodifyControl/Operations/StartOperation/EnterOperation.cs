using Pkn_HostSystem.NodifyControl.Nodes;
using Pkn_HostSystem.NodifyControl.Operations.Core;
using Pkn_HostSystem.NodifyControl.Operations.Interface;
using Pkn_HostSystem.NodifyControl.Views.NodeOperation;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.Operations.StartOperation
{
    public class EnterOperation : BaseOperation<EnterOperationNode>, IStartOperation
    {

        public EnterOperation(EnterOperationNode _node):base(_node, new EnterOperationUserControl()) { }


        protected override async Task OnExecute()
        {
            
        }


    }
}