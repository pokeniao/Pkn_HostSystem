using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Page;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.NodifyControl.Nodes.Core
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
            },
            new ()
            {
                Name = "通讯",
                Children =
                    [
                        new TreeNodes(name: "ModbusTcp", nodeType: NodeEnum.ModbusTcp),
                        new TreeNodes("Http",NodeEnum.Http)
                    ]
            },
            new ()
            {
                Name = "逻辑类",
                Children =
                    [
                    new TreeNodes(name:"If",nodeType:NodeEnum.If)
                    ]
            }
        };

        /// <summary>
        /// 更具TreeNodes创建一个MyNode
        /// </summary>
        /// <param name="treeNodes"></param>
        /// <returns></returns>
        public static PknNode CreateNode(NodeEnum NodeType, DesignModel designModel , Object model = null)
        {
            PknNode pknNode = null;

            switch (NodeType)
            {
                case NodeEnum.Add:
                    pknNode = new AddOperationNode(designModel,model);
                    break;
                case NodeEnum.Enter:
                    pknNode = new EnterOperationNode(designModel,model);
                    break;

                case NodeEnum.ModbusTcp:
                    pknNode = new ModbusTcpOperationNode(designModel,model);
                    break;
                case NodeEnum.If:
                    pknNode = new IfOperationNode(designModel, model);
                    break;
                case NodeEnum.Http:
                    pknNode = new HttpOperationNode(designModel, model);
                    break;
            }
            return pknNode;
        }
    }
}