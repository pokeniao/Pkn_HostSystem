using CommunityToolkit.Mvvm.Input;
using Pkn_HostSystem.NodifyControl.Connection;
using Pkn_HostSystem.NodifyControl.Node;
using Pkn_HostSystem.NodifyControl.Node.Connector;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Pkn_HostSystem.NodifyControl.Editor
{
    
    public class EditorViewModel
    {
        /// <summary>
        /// 节点集合
        /// </summary>
        public ObservableCollection<PNode> Nodes { get; set; } = new ObservableCollection<PNode>();
        /// <summary>
        /// 连接点集合
        /// </summary>
        public ObservableCollection<ConnectorViewModel> Connectors { get;  } = new ObservableCollection<ConnectorViewModel>();
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

            DisconnectConnectorCommand = new RelayCommand<Connector>(connector =>
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




            PNode pNode = new PNode()
            {
                NodeName = "HelloWorld",
                Input = new ObservableCollection<Connector>() { new Connector() { ConnectorName = "输入" } },
                Output = new ObservableCollection<Connector>(){new Connector(){ ConnectorName = "输出"}},
              
            };

            PNode pNode2 = new PNode()
            {
                NodeName = "HelloWorld2",
                Input = new ObservableCollection<Connector>() { new Connector() { ConnectorName = "输入" } },
                Location = new Point(50, 100)
            };

            PNode pNode3 = new PNode()
            {
                NodeName = "HelloWorld3",
                Input = new ObservableCollection<Connector>() { new Connector() { ConnectorName = "输入" } },
                Location = new Point(100, 100)
            };
            Nodes.Add(pNode);
            Nodes.Add(pNode2);
            Nodes.Add(pNode3);
            Connectors.Add(new ConnectorViewModel( pNode.Output[0], pNode2.Input[0]));
        }

        //编辑器中的端子连接方法
        public void Connect(Connector source, Connector target)
        {
            var newConnection = new ConnectorViewModel(source, target);
            //检查是否已存在相同的连接
            if (!Connectors.Contains(newConnection))
            {
                Connectors.Add(newConnection);
            }
        }

    }
}