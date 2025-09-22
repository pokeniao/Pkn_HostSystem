using CommunityToolkit.Mvvm.DependencyInjection;
using DynamicData.Binding;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.NodifyControl.Connection;
using Pkn_HostSystem.NodifyControl.Editor;
using Pkn_HostSystem.NodifyControl.Node;
using Pkn_HostSystem.NodifyControl.Node.Connector;
using Pkn_HostSystem.NodifyControl.Node.DesignTreeNode;
using Pkn_HostSystem.ViewModels.Page;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.NodifyControl.LocalSave
{
    public static class LocalSaveNodifyMethod
    {
        public static void Save()
        {
            DesignViewModel designViewModel = Ioc.Default.GetRequiredService<DesignViewModel>();

            //获取到当前的EditorViewModel 
            List<DesignModel> designModels = designViewModel.ProjectModel.ProjectList.ToList();


            //遍历所有的DesignModel进行保存
            foreach (var designModel in designModels)
            {
                //当前显示的EVM
                EditorViewModel curEVM = designModel.EditorViewModel;
                //保存类
                LocalSaveNodify saveNodify = designModel.LocalSaveNodify;

                //保存节点
                saveNodify.Nodes = saveNodify.SaveNodes(curEVM.Nodes);
                saveNodify.Connections = saveNodify.SaveConnections(curEVM.Connectors);
            }
        }


        public static ProjectModel Load()
        {
            ProjectModel load = JsonTool<ProjectModel>.Load();

            //获取保存的ProjectList
            var projectList = load.ProjectList;
            //从ProjectList中取出designModel
            foreach (var designModel in projectList)
            {
                //获取本地保存内容
                LocalSaveNodify designModelLocalSaveNodify = designModel.LocalSaveNodify;
                //获取EVM
                EditorViewModel designModelEditorViewModel = designModel.EditorViewModel;

                //EVM中的Nodes
                ObservableCollection<MyNode> Nodes = designModelEditorViewModel.Nodes;
                //EVM中的Connectors
                ObservableCollection<ConnectorViewModel> Connectors = designModelEditorViewModel.Connectors;
                //转成EVM
                //先获得所有的Node
                List<LocalSaveNode> localSaveNodes = designModelLocalSaveNodify.Nodes;
                foreach (LocalSaveNode localSaveNode in localSaveNodes)
                {
                    MyNode myNode = DesignTreeNode.GetNode(localSaveNode.TreeNodes);
                    myNode.Id = localSaveNode.Id;
                    myNode.Location = localSaveNode.Location;
                    myNode.Input = designModelLocalSaveNodify.GetInputOrOutput(localSaveNode.Input);
                    myNode.Output = designModelLocalSaveNodify.GetInputOrOutput(localSaveNode.Output);

                    Nodes.Add(myNode);
                }
                //在获得线
                List<LocalSaveConnection> localSaveConnections = designModelLocalSaveNodify.Connections;

                foreach (LocalSaveConnection localSaveConnection in localSaveConnections)
                {
                    string sourceConectorId = localSaveConnection.SourceConectorId;
                    string targetConectorId = localSaveConnection.TargetConectorId;

                    MyConnector sourceMyConnector =null;
                    MyConnector targetMyConnector = null;
                    //找到对应端子的实例化对象
                    foreach (MyNode myNode in Nodes)
                    {
                        foreach (var connector in myNode.Output)
                        {
                            if (sourceConectorId == connector.Id)
                            {
                                sourceMyConnector = connector;
                            }
                        }

                        foreach (var connector in myNode.Input)
                        {
                            if (targetConectorId == connector.Id)
                            {
                                targetMyConnector = connector;
                            }
                        }
                    }
                    ConnectorViewModel connectorViewModel = new ConnectorViewModel(sourceMyConnector , targetMyConnector);

                    Connectors.Add(connectorViewModel);
                }
            }

            return load;
        }



    }
}