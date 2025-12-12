using Pkn_HostSystem.NodifyControl.LocalSave.Pojo;
using Pkn_HostSystem.NodifyControl.Nodes.Connector;
using Pkn_HostSystem.NodifyControl.Nodes.Core;
using Pkn_HostSystem.NodifyControl.ViewModels.Connection;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.NodifyControl.LocalSave.Services
{
    public class LocalSaveNodify
    {
        public List<LocalSaveNode> Nodes { get; set; } = new();

        public List<LocalSaveConnection> Connections { get; set; } = new();


        public List<LocalSaveNode> SaveNodes(ObservableCollection<PknNode> Nodes)
        {
            List<LocalSaveNode> localSaveNodes = new();
            List<PknNode> myNodes = Nodes.ToList();
            foreach (PknNode myNode in myNodes)
            {
                LocalSaveNode localSaveNode = new()
                {
                    Id = myNode.Id,
                    Location = myNode.Location,
                    NodeType = myNode.NodeType,
                    Input = new List<LocalSaveConnector>(),
                    Output = new List<LocalSaveConnector>(),
                    InputParam = Enumerable.ToList(myNode.InputParams),
                    OutputParam = Enumerable.ToList(myNode.OutputParams),
                    model = myNode.IModel
                };
                //输入
                foreach (MyConnector input in myNode.Input)
                {
                    LocalSaveConnector localSaveConnector = new()
                    {
                        Id = input.Id,
                        NodeId = myNode.Id,
                        Anchor = input.Anchor,
                        ConnectorName = input.ConnectorName,
                        ConnectorType = input.ConnectorType,
                    };
                    localSaveNode.Input.Add(localSaveConnector);
                }
                //输出
                foreach (MyConnector output in myNode.Output)
                {
                    LocalSaveConnector localSaveConnector = new()
                    {
                        Id = output.Id,
                        NodeId = myNode.Id,
                        Anchor = output.Anchor,
                        ConnectorName = output.ConnectorName,
                        ConnectorType = output.ConnectorType,
                    };
                    localSaveNode.Output.Add(localSaveConnector);
                }
                localSaveNodes.Add(localSaveNode);
            }
            return localSaveNodes;
        }

        public List<LocalSaveConnection> SaveConnections(ObservableCollection<ConnectorViewModel> Connections)
        {
            List<LocalSaveConnection> localSaveConnections = new();
            List<ConnectorViewModel> myConnections = Connections.ToList();
            foreach (ConnectorViewModel myConnection in myConnections)
            {
                LocalSaveConnection localSaveConnection = new()
                {
                    SourceConectorId = myConnection.Source.Id,
                    TargetConectorId = myConnection.Target.Id
                };
                localSaveConnections.Add(localSaveConnection);
            }               
            return localSaveConnections;
        }


        public  void ResetInputOrOutput(List<LocalSaveConnector> InputOrOutput , ObservableCollection<MyConnector> oldMyConnectors , PknNode node)
        {
            if (InputOrOutput.Count != oldMyConnectors.Count)
            {
                throw new Exception("本地保存的节点与实际节点数量不同");
            }
            for (int i = 0; i < oldMyConnectors.Count; i++)
            {
                oldMyConnectors[i].Id = InputOrOutput[i].Id;
                oldMyConnectors[i].Anchor = InputOrOutput[i].Anchor;
                oldMyConnectors[i].NodeId = InputOrOutput[i].NodeId;
                oldMyConnectors[i].ConnectorName = InputOrOutput[i].ConnectorName;
                //不等于空表示是输出的接口
                if (node != null)
                {
                    oldMyConnectors[i].Value = node.OutputParams;
                }
            }
        }
    }
}