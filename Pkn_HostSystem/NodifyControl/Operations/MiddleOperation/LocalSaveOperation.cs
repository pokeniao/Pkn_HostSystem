using Pkn_HostSystem.NodifyControl.Nodes;
using Pkn_HostSystem.NodifyControl.Operations.Core;
using Pkn_HostSystem.NodifyControl.Views.NodeOperation;

namespace Pkn_HostSystem.NodifyControl.Operations.MiddleOperation
{
    public class LocalSaveOperation(LocalSaveOperationNode node)
        : BaseOperation<LocalSaveOperationNode>(node, new LocalSaveOperationUserControl())
    {
        protected override async Task OnExecute(CancellationTokenSource cts)
        {
            //
        }
    }
}