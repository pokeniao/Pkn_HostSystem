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
        /// <summary>
        /// 更具TreeNodes创建一个MyNode
        /// </summary>
        /// <param name="treeNodes"></param>
        /// <returns></returns>
        public static MyNode GetNode(TreeNodes treeNodes)
        {
            switch (treeNodes.NodeType)
            {
                case NodeEnum.Add:
                    MyNode myNode = new MyNode()
                    {
                        NodeName = treeNodes.Name,
                        Operation = new AddOperation()
                    };
                    myNode.Input.Add(new MyConnector("输入" , myNode.Id , ConnectorTypeEnum.Input));
                    myNode.Output.Add(new MyConnector("输出", myNode.Id, ConnectorTypeEnum.Output));
                    return myNode;
                case NodeEnum.DebugEnter:
                     myNode = new MyNode()
                    {
                        NodeName = treeNodes.Name,
                        Operation = new TestOperation()
                    };
                    myNode.Output.Add(new MyConnector("输出", myNode.Id, ConnectorTypeEnum.Output));
                    return myNode;
                default:
                    return new MyNode()
                    {

                    };
            }
        }
    }
}