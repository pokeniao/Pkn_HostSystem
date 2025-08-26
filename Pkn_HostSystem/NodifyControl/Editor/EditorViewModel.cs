using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.Connection;
using Pkn_HostSystem.NodifyControl.Node;
using Pkn_HostSystem.NodifyControl.Node.Connector;
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
        public ObservableCollection<ConnectorViewModel> Connectors { get; } =
            new ObservableCollection<ConnectorViewModel>();

        /// <summary>
        /// 添加连接预处理
        /// </summary>
        public PendingConnectionViewModel PendingConnection { get; }

        /// <summary>
        /// 移除连接点预处理
        /// </summary>
        public ICommand DisconnectConnectorCommand { get; }

        /// <summary>
        /// 移除连接线
        /// </summary>
        public ICommand RemoveConnectionCommand { get; }


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
            var newConnection = new ConnectorViewModel(source, target);
            //检查是否已存在相同的连接
            if (!Connectors.Contains(newConnection))
            {
                Connectors.Add(newConnection);
            }
        }

        [RelayCommand]
        public void DeleteSelection()
        {
            List<MyNode> l2 = new List<MyNode>();
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

        public static MyNode GetNode(TreeNodes treeNodes)
        {
            switch (treeNodes.NodeType)
            {
                case NodeEnum.Add:
                    return new MyNode()
                    {
                        NodeName = "Add",
                        Input = new ObservableCollection<MyConnector>() { new MyConnector() { ConnectorName = "输入" } },
                        Output = new ObservableCollection<MyConnector>() { new MyConnector() { ConnectorName = "输出" } },
                    };
                case NodeEnum.Subtract:
                    return new MyNode()
                    {
                        NodeName = "Sub",
                        Input = new ObservableCollection<MyConnector>() { new MyConnector() { ConnectorName = "输入" } },
                        Output = new ObservableCollection<MyConnector>() { new MyConnector() { ConnectorName = "输出" } },
                    };

                default:
                    return new MyNode() { };
            }
        }
    }
}