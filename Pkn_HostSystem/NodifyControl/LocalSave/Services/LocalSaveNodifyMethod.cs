using CommunityToolkit.Mvvm.DependencyInjection;
using DynamicData.Binding;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.NodifyControl.LocalSave.Pojo;
using Pkn_HostSystem.NodifyControl.Nodes.Connector;
using Pkn_HostSystem.NodifyControl.Nodes.Core;
using Pkn_HostSystem.NodifyControl.OperationModels.Models.Core;
using Pkn_HostSystem.NodifyControl.ViewModels.Connection;
using Pkn_HostSystem.NodifyControl.ViewModels.Editor;
using Pkn_HostSystem.ViewModels.Page;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.NodifyControl.LocalSave.Services
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

            if (load == null)
            {
                return null;
            }


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
                ObservableCollection<PknNode> Nodes = designModelEditorViewModel.Nodes;
                //EVM中的Connectors
                ObservableCollection<ConnectorViewModel> Connectors = designModelEditorViewModel.Connectors;
                //转成EVM
                //先获得所有的Node
                List<LocalSaveNode> localSaveNodes = designModelLocalSaveNodify.Nodes;
                foreach (LocalSaveNode localSaveNode in localSaveNodes)
                {
                    //Node节点和端子重新创建
                    PknNode pknNode = DesignTreeNode.CreateNode(localSaveNode.NodeType,designModel,localSaveNode.model);
                    pknNode.Id = localSaveNode.Id;
                    pknNode.Location = localSaveNode.Location;
                    //输入参数
                    pknNode.InputParams = new ObservableCollectionExtended<OperationModel>(localSaveNode.InputParam);
                    //输出参数
                    pknNode.OutputParams = new ObservableCollectionExtended<OperationModel>(localSaveNode.OutputParam);


                    designModelLocalSaveNodify.ResetInputOrOutput(localSaveNode.Input, pknNode.Input, null);
                    designModelLocalSaveNodify.ResetInputOrOutput(localSaveNode.Output, pknNode.Output, pknNode);
               
                    Nodes.Add(pknNode);
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
                    foreach (PknNode myNode in Nodes)
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