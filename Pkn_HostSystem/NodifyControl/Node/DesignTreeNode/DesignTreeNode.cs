
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.Node.Base;
using Pkn_HostSystem.NodifyControl.Node.Connector;
using Pkn_HostSystem.NodifyControl.Operation;
using Pkn_HostSystem.NodifyControl.Operation.StartOperation;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.NodifyControl.Node.DesignTreeNode
{
    public static class DesignTreeNode
    {
        public static ObservableCollection<TreeNodes> TreeNodesList = new ObservableCollection<TreeNodes>()
        {
            new()
            {
                Name = "程序入口",
                Children =
                [
                    new TreeNodes(name: "入口", nodeType: NodeEnum.Enter)
                ]
            },
            new()
            {
                Name = "计算",
                Children =
                [
                    new TreeNodes(name: "加法", nodeType: NodeEnum.Add)
                ]
            }
        };
        /// <summary>
        /// 更具TreeNodes创建一个MyNode
        /// </summary>
        /// <param name="treeNodes"></param>
        /// <returns></returns>
        public static PknNode GetNode(NodeEnum NodeType)
        {
            switch (NodeType)
            {
                case NodeEnum.Add:
                    return new AddOperationNode();
                case NodeEnum.Enter:
                    return new EnterOperationNode();
                default:
                    return new Base.PknNode()
                    {

                    };
            }
        }
    }
}