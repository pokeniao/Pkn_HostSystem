using DynamicData;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.Node.Connector;
using Pkn_HostSystem.NodifyControl.Operation;
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
                    new TreeNodes(name: "手动触发进入", nodeType: NodeEnum.DebugEnter)
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

        public static MyNode GetNode(TreeNodes treeNodes)
        {
            switch (treeNodes.NodeType)
            {
                case NodeEnum.Add:
                    return new MyNode()
                    {
                        NodeName = treeNodes.Name,
                        Input = [new() { ConnectorName = "输入" }],
                        Output = [new() { ConnectorName = "输出" }],
                        Operation = new AddOperation()
                    };
                case NodeEnum.DebugEnter:
                    return new MyNode()
                    {
                        NodeName = treeNodes.Name,
                        Output = [new() { ConnectorName = "输出" }],
                        Operation = new TestOperation()
                    };
                default:
                    return new MyNode()
                    {

                    };
            }
        }
    }
}