using Nodify;
using Pkn_HostSystem.NodifyControl.Connection;
using Pkn_HostSystem.NodifyControl.Node;
using Pkn_HostSystem.NodifyControl.Node.Base;
using Pkn_HostSystem.NodifyControl.Node.Connector;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.NodifyControl.LocalSave
{
    public class LocalSaveNodify
    {
        public List<LocalSaveNode> Nodes { get; set; } = new List<LocalSaveNode>();

        public List<LocalSaveConnection> Connections { get; set; } = new List<LocalSaveConnection>();


        public List<LocalSaveNode> SaveNodes(ObservableCollection<PknNode> Nodes)
        {
            List<LocalSaveNode> localSaveNodes = new List<LocalSaveNode>();
            List<PknNode> myNodes = Nodes.ToList();
            foreach (PknNode myNode in myNodes)
            {
                LocalSaveNode localSaveNode = new LocalSaveNode()
                {
                    Id = myNode.Id,
                    Location = myNode.Location,
                    NodeType = myNode.NodeType,
                    Input = new List<LocalSaveConnector>(),
                    Output = new List<LocalSaveConnector>()
                };
                //输入
                foreach (MyConnector input in myNode.Input)
                {
                    LocalSaveConnector localSaveConnector = new LocalSaveConnector()
                    {
                        Id = input.Id,
                        NodeId = myNode.Id,
                        Anchor = input.Anchor,
                        ConnectorName = input.ConnectorName,
                        ConnectorType = input.ConnectorType,
                        ValueObservers = SaveValueObservers(input.ValueObservers)
                    };
                    localSaveNode.Input.Add(localSaveConnector);
                }
                //输出
                foreach (MyConnector output in myNode.Output)
                {
                    LocalSaveConnector localSaveConnector = new LocalSaveConnector()
                    {
                        Id = output.Id,
                        NodeId = myNode.Id,
                        Anchor = output.Anchor,
                        ConnectorName = output.ConnectorName,
                        ConnectorType = output.ConnectorType,
                        ValueObservers = SaveValueObservers(output.ValueObservers)
                    };
                    localSaveNode.Output.Add(localSaveConnector);
                }
                localSaveNodes.Add(localSaveNode);
            }

            return localSaveNodes;
        }
        /// <summary>
        /// 递归添加ValueObservers
        /// </summary>
        /// <param name="inputValueObservers"></param>
        /// <returns></returns>
        public List<LocalSaveConnector> SaveValueObservers(List<MyConnector> inputValueObservers)
        {
            List<LocalSaveConnector> localSaveConnectors = new List<LocalSaveConnector>();
            if (inputValueObservers == null )
            {
                return localSaveConnectors;
            }

            if (inputValueObservers.Count == 0)
            {
                return localSaveConnectors;
            }

            foreach (var inputValueObserver in inputValueObservers)
            {
                LocalSaveConnector localSaveConnectorObserver = new LocalSaveConnector()
                {
                    Id = inputValueObserver.Id,
                    NodeId = inputValueObserver.NodeId,
                    Anchor = inputValueObserver.Anchor,
                    ConnectorName = inputValueObserver.ConnectorName,
                    ConnectorType = inputValueObserver.ConnectorType,
                    ValueObservers = SaveValueObservers(inputValueObserver.ValueObservers)
                }; 
                localSaveConnectors.Add(localSaveConnectorObserver);
            }

            return localSaveConnectors;
        }




        public List<LocalSaveConnection> SaveConnections(ObservableCollection<ConnectorViewModel> Connections)
        {
            List<LocalSaveConnection> localSaveConnections = new List<LocalSaveConnection>();
            List<ConnectorViewModel> myConnections = Connections.ToList();



            foreach (ConnectorViewModel myConnection in myConnections)
            {
                LocalSaveConnection localSaveConnection = new LocalSaveConnection()
                {
                    SourceConectorId = myConnection.Source.Id,
                    TargetConectorId = myConnection.Target.Id
                };
                localSaveConnections.Add(localSaveConnection);
            }               
            return localSaveConnections;
        }


        public  ObservableCollection<MyConnector> GetInputOrOutput(List<LocalSaveConnector> InputOrOutput)
        {
            var observableCollection = new ObservableCollection<MyConnector>();

            foreach (var connector in InputOrOutput)
            {
                MyConnector myConnector = new MyConnector(connector.ConnectorName, connector.NodeId, connector.ConnectorType);

                myConnector.Id = connector.Id;
                myConnector.Anchor = connector.Anchor;
                myConnector.ValueObservers = GetValueObservers(connector.ValueObservers);
                observableCollection.Add(myConnector);
            }

            return observableCollection;

        }

        public List<MyConnector> GetValueObservers(List<LocalSaveConnector> LocalSaveConnectors)
        {
            List<MyConnector> myConnectors = new List<MyConnector>();

            foreach (var connector in LocalSaveConnectors)
            {
                MyConnector myConnector = new MyConnector(connector.ConnectorName, connector.NodeId, connector.ConnectorType);

                myConnector.Id = connector.Id;
                myConnector.Anchor = connector.Anchor;
                myConnector.ValueObservers = GetValueObservers(connector.ValueObservers);

                myConnectors.Add(myConnector);
            }

            return myConnectors;
        }

    }
}