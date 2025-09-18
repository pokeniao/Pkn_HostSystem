using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using Newtonsoft.Json;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.Connection;
using Pkn_HostSystem.NodifyControl.Node;
using Pkn_HostSystem.NodifyControl.Node.Connector;
using Pkn_HostSystem.NodifyControl.Operation;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Pkn_HostSystem.NodifyControl.Editor
{
    public partial class EditorViewModel : ObservableRecipient
    {
        /// <summary>
        /// 节点集合
        /// </summary>
        public ObservableCollection<MyNode> Nodes { get; set; } = new ObservableCollection<MyNode>();

        /// <summary>
        /// 连接点集合
        /// </summary>
        public ObservableCollection<ConnectorViewModel> Connectors { get; set; } =
            new ObservableCollection<ConnectorViewModel>();

        /// <summary>
        /// 添加连接预处理
        /// </summary>
        [JsonIgnore]
        public PendingConnectionViewModel PendingConnection { get;  }

        /// <summary>
        /// 移除连接点预处理
        /// </summary>
        public ICommand DisconnectConnectorCommand { get; }

        /// <summary>
        /// 移除连接线
        /// </summary>
        public ICommand RemoveConnectionCommand { get; }

        /// <summary>
        /// 选中的
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<MyNode> SelectedConnectors { get; set; } =
            new ObservableCollection<MyNode>();

        public EditorViewModel()
        {
            PendingConnection = new PendingConnectionViewModel(this);


            RemoveConnectionCommand = new RelayCommand<ConnectorViewModel>(c =>
            {
                Connectors.Remove(c);
                var ic = Connectors.Count(con => con.Source == c.Source || con.Target == c.Source);
                var oc = Connectors.Count(con => con.Source == c.Target || con.Target == c.Target);
                if (ic == 0)
                {
                    c.Source.IsConnected = false;
                }

                if (oc == 0)
                {
                    c.Target.IsConnected = false;
                }
            });

            DisconnectConnectorCommand = new RelayCommand<MyConnector>(connector =>
            {
                var connections = Connectors.Where(c => c.Source == connector || c.Target == connector).ToList();
                connections.ForEach(c =>
                {
                    Connectors.Remove(c);
                    var ic = Connectors.Count(con => con.Source == c.Source || con.Target == c.Source);
                    var oc = Connectors.Count(con => con.Source == c.Target || con.Target == c.Target);
                    if (ic == 0)
                    {
                        c.Source.IsConnected = false;
                    }

                    if (oc == 0)
                    {
                        c.Target.IsConnected = false;
                    }
                });
            });
        }

        //编辑器中的端子连接方法
        public void Connect(MyConnector source, MyConnector target)
        {
            //检查是否已存在相同的连接
            var exists = Connectors.Any(c => c.Source == source && c.Target == target);
            if (!exists)
            {
                Connectors.Add(new ConnectorViewModel(source, target));
            }
        }

        [RelayCommand]
        public void DeleteSelection()
        {
            List<MyNode> l2 = new();
            foreach (MyNode selectedConnector in SelectedConnectors)
            {
                l2.Add(selectedConnector as MyNode);


                foreach (MyConnector myConnector in selectedConnector.Input)
                {
                    //匹配
                    List<ConnectorViewModel> myConnectors = Connectors.Where(c =>
                        c.Source == myConnector ||
                        c.Target == myConnector).ToList();
                    //移除线
                    Connectors.Remove(myConnectors);
                    //清理节点
                    foreach (ConnectorViewModel c in myConnectors)
                    {
                        var ic = Connectors.Count(con => con.Source == c.Source || con.Target == c.Source);
                        var oc = Connectors.Count(con => con.Source == c.Target || con.Target == c.Target);
                        if (ic == 0)
                        {
                            c.Source.IsConnected = false;
                        }

                        if (oc == 0)
                        {
                            c.Target.IsConnected = false;
                        }
                    }
                }

                foreach (MyConnector myConnector in selectedConnector.Output)
                {
                    //匹配
                    List<ConnectorViewModel> myConnectors = Connectors.Where(c =>
                        c.Source == myConnector ||
                        c.Target == myConnector).ToList();
                    //移除线
                    Connectors.Remove(myConnectors);
                    //清理节点
                    foreach (ConnectorViewModel c in myConnectors)
                    {
                        var ic = Connectors.Count(con => con.Source == c.Source || con.Target == c.Source);
                        var oc = Connectors.Count(con => con.Source == c.Target || con.Target == c.Target);
                        if (ic == 0)
                        {
                            c.Source.IsConnected = false;
                        }

                        if (oc == 0)
                        {
                            c.Target.IsConnected = false;
                        }
                    }
                }
            }

            Nodes.Remove(l2);
        }

        [RelayCommand]
        public void Run()
        {
            //1. 寻找到IStartOperation节点,作为起始节点

            MyNode startNode = Nodes.FirstOrDefault(n => n.Operation is IStartOperation);
            if (startNode == null)
            {
                MessageBox.Show("未找到起始节点,请添加一个IStartOperation节点");
                return;
            }

            //2. 执行起始节点的方法
            IStartOperation startOperation = startNode.Operation as IStartOperation;
            object[] results = startOperation.Execute();
            //3. 递归执行后续节点的方法
            ExecuteNextNodes(startNode, results);
        }

        private void ExecuteNextNodes(MyNode currentNode, object[] inputs)
        {
            //找到所有连接到当前节点输出端子的连接
            var outgoingConnections = Connectors.Where(c => currentNode.Output.Contains(c.Source)).ToList();
            //对于每个连接,找到连接的目标节点,并执行其方法
            foreach (var connection in outgoingConnections)
            {
                MyNode nextNode = Nodes.FirstOrDefault(n => n.Input.Contains(connection.Target));
                if (nextNode != null && nextNode.Operation != null)
                {
                    //执行下一个节点的方法
                    var operation = nextNode.Operation;
                    object[] results = operation.Execute(inputs);
                    //递归执行下一个节点
                    ExecuteNextNodes(nextNode, results);
                }
            }
        }
    }
}