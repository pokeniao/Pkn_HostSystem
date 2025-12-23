using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using Newtonsoft.Json;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.NodifyControl.Nodes.Connector;
using Pkn_HostSystem.NodifyControl.Nodes.Core;
using Pkn_HostSystem.NodifyControl.Operations.Interface;
using Pkn_HostSystem.NodifyControl.ViewModels.Connection;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Pkn_HostSystem.NodifyControl.ViewModels.Editor
{
    public partial class EditorViewModel : ObservableRecipient
    {
        public string ProjectName { get; set; }

        /// <summary>
        /// 节点集合
        /// </summary>
        public ObservableCollection<PknNode> Nodes { get; set; } = new();

        /// <summary>
        /// 连接点集合
        /// </summary>
        public ObservableCollection<ConnectorViewModel> Connectors { get; set; } = new();

        /// <summary>
        /// 添加连接预处理
        /// </summary>
        public PendingConnectionViewModel PendingConnection { get; }

        /// <summary>
        /// 移除连接点预处理
        /// </summary>
        public ICommand DisconnectConnectorCommand { get; }

        /// <summary>
        /// 移除连接线事件
        /// </summary>
        public ICommand RemoveConnectionCommand { get; }

        /// <summary>
        /// 选中的
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<PknNode> SelectedConnectors { get; set; } = new();

        public EditorViewModel()
        {
            //1.实例化预添加
            PendingConnection = new PendingConnectionViewModel(this);

            //2. 实例化移除 连接线 事件
            RemoveConnectionCommand = new RelayCommand<ConnectorViewModel>(c =>
            {
                //从观察中删除
                c?.Source.ValueObservers.Remove(c.Target);
                c?.Target.InputValue.Remove(c.Source.Value);
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
            //3. 实例化移除 连接点 事件
            DisconnectConnectorCommand = new RelayCommand<MyConnector>(connector =>
            {
                var connections = Connectors.Where(c => c.Source == connector || c.Target == connector).ToList();
                connections.ForEach(c =>
                {
                    //从观察中删除
                    c?.Source.ValueObservers.Remove(c.Target);
                    c?.Target.InputValue.Remove(c.Source.Value);
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

        /// <summary>
        /// 编辑器中的端子连接方法
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void Connect(MyConnector source, MyConnector target)
        {
            //检查是否已存在相同的连接
            var exists = Connectors.Any(c => c.Source == source && c.Target == target);
            if (!exists)
            {
                Connectors.Add(new ConnectorViewModel(source, target));
            }
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        [RelayCommand]
        public void DeleteSelection()
        {
            List<PknNode> l2 = new();
            foreach (PknNode selectedConnector in SelectedConnectors)
            {
                l2.Add(selectedConnector as PknNode);

                //获取到当前Node的Input
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
                        c?.Source.ValueObservers.Remove(c.Target);
                        c?.Target.InputValue.Remove(c.Source.Value);

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

                //获取到当前Node的OutPut
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
                        c?.Source.ValueObservers.Remove(c.Target);
                        c?.Target.InputValue.Remove(c.Source.Value);
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


        private CancellationTokenSource runCts;

        [RelayCommand]
        public async void Run()
        {
            if (runCts != null)
            {
                runCts.Cancel();
            }

            runCts = new CancellationTokenSource();
            TraceContext.Name = ProjectName;
            //1. 寻找到IStartOperation节点,作为起始节点
            PknNode startNode = Nodes.FirstOrDefault(n => n.Operation is IStartOperation);
            if (startNode == null)
            {
                MessageBox.Show("未找到起始节点,请添加一个IStartOperation节点");
                return;
            }

            //2. 执行起始节点的方法
            IStartOperation startOperation = startNode.Operation as IStartOperation;
            await startOperation.Execute(runCts);
            //3. 递归执行后续节点的方法
            await ExecuteNextNodes(startNode, runCts);
        }


        public async Task Run(CancellationTokenSource cts)
        {
            TraceContext.Name = ProjectName;
            //1. 寻找到IStartOperation节点,作为起始节点
            PknNode startNode = Nodes.FirstOrDefault(n => n.Operation is IStartOperation);
            if (startNode == null)
            {
                MessageBox.Show("未找到起始节点,请添加一个IStartOperation节点");
                return;
            }

            //2. 执行起始节点的方法
            IStartOperation startOperation = startNode.Operation as IStartOperation;
            await startOperation.Execute(cts);
            //3. 递归执行后续节点的方法
            await ExecuteNextNodes(startNode, cts);
        }


        private async Task ExecuteNextNodes(PknNode currentNode, CancellationTokenSource cts)
        {
            //找到所有连接到当前节点输出端子的连接
            var outgoingConnections = Connectors.Where(c => currentNode.Output.Contains(c.Source) && c.Source.Enabled)
                .ToList();
            //对于每个连接,找到连接的目标节点,并执行其方法
            foreach (var connection in outgoingConnections)
            {
                PknNode nextNode = Nodes.FirstOrDefault(n => n.Input.Contains(connection.Target));
                if (nextNode != null && nextNode.Operation != null)
                {
                    //执行下一个节点的方法
                    var operation = nextNode.Operation;
                    await operation.Execute(cts);
                    //递归执行下一个节点
                    await ExecuteNextNodes(nextNode, cts);
                }
            }
        }
    }
}