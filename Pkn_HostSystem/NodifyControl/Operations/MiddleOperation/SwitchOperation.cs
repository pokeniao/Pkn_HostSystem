using Pkn_HostSystem.NodifyControl.Nodes;
using Pkn_HostSystem.NodifyControl.Operations.Core;
using Pkn_HostSystem.NodifyControl.Views.NodeOperation;

namespace Pkn_HostSystem.NodifyControl.Operations.MiddleOperation
{
    public class SwitchOperation(SwitchOperationNode node)
        : BaseOperation<SwitchOperationNode>(node, new SwitchOperationUserControl())
    {
        protected override async Task OnExecute(CancellationTokenSource cts)
        {

            bool temp = false;
            node.Output[0].Enabled = false;
            for (int i = 0; i < node.Model.SwitchCount; i++)
            {
                if (GetParamValue(node.InputParams[i + 1]).Equals(GetParamValue(node.InputParams[0])))
                {
                    temp = true;
                    node.Output[i+1].Enabled = true;
                }
                else
                {
                    node.Output[i + 1].Enabled = false;
                }
            }

            if (!temp)
            {
                node.Output[0].Enabled = true;
            }
       
        }
    }
}