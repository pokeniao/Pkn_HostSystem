using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.NodifyControl.Nodes;
using Pkn_HostSystem.NodifyControl.Nodes.Connector;
using Pkn_HostSystem.NodifyControl.OperationModels.Models.Core;
using Pkn_HostSystem.NodifyControl.ViewModels.Editor;
using Pkn_HostSystem.ViewModels.Page;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Pkn_HostSystem.NodifyControl.Views.NodeOperation
{
    /// <summary>
    /// SwitchOperationUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class SwitchOperationUserControl : UserControl
    {
        public SwitchOperationUserControl()
        {
            InitializeComponent();
        }


        private void TextBoxBase_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            SwitchOperationNode? switchOperationNode = DataContext as SwitchOperationNode;

            int count = switchOperationNode.Model.SwitchCount + 1;
            ArrayList list = new();

            //计算不能删除的
            int cur = 0;
            int inputParamsCount = switchOperationNode.InputParams.Count;
            for (int i = 0; i < inputParamsCount; i++)
            {
                if (switchOperationNode.InputParams[i].NoDelete == true)
                {
                    cur++;
                }
                else
                {
                    //将获取到的输入添加
                    list.Add(switchOperationNode.InputParams[i]);
                    switchOperationNode.InputParams.RemoveAt(i);
                    i--;
                    inputParamsCount--;
                }
            }
            //判断是否需要添加

            int cur2 = 0;
            if (cur > count)
            {
                //大于需要不添加,反而需要删除

                for (int i = cur; i > count; i--)
                {
                    switchOperationNode.InputParams.RemoveAt(count);
                    cur2--;
                }
            }
            else if (cur < count)
            {
                //小于需要添加
                //添加数量
                for (int i = cur; i < count; i++)
                {
                    switchOperationNode.InputParams.Add(new OperationModel()
                    {
                        Name = (i).ToString(),
                        NameReadOnly = true,
                        NoDelete = true,
                    });
                    cur2++;
                }
            }
            foreach (OperationModel operationModel in list)
            {
                switchOperationNode.InputParams.Add(operationModel);
            }

            int cur3 = cur2 + cur;
            //更具不能删除的节点创建 接口
            //判断有多少节点了,需不需要添加 , 因为默认有一个了,所以应该-1
            int outputCount = switchOperationNode.Output.Count - 1;


            int cur4 = cur3 - 1;
            if (outputCount <= cur4)
            {
                //小于,需要添加节点
                for (int i = 0; i < cur4; i++)
                {

                    if (i <= outputCount - 1)
                    {
                        //仅做更新
                        switch (switchOperationNode.InputParams[i + 1].ParamMethod)
                        {
                            case "常量":
                                switchOperationNode.Output[i + 1].ConnectorName = switchOperationNode.InputParams[i + 1].ParamValue;
                                break;
                            case "动态获取":
                                switchOperationNode.Output[i + 1].ConnectorName = "\"" + switchOperationNode.InputParams[i + 1].DynName + "\"";
                                break;
                        }
                    }
                    else
                    {
                        string connectorName = "";
                        switch (switchOperationNode.InputParams[i + 1].ParamMethod)
                        {
                            case "常量":
                                connectorName = switchOperationNode.InputParams[i + 1].ParamValue;
                                break;
                            case "动态获取":
                                connectorName = "\"" + switchOperationNode.InputParams[i + 1].DynName + "\"";
                                break;
                        }
                        //添加节点
                        MyConnector myConnector = new(connectorName, switchOperationNode.Id, ConnectorTypeEnum.Output)
                        {
                            Value = switchOperationNode.OutputParams
                        };
                        switchOperationNode.Output.Add(myConnector);
                    }
                }
            }
            else if (outputCount > cur4)
            {
                //大于,不需要添加节点,并且需要删除

                for (int i = outputCount; i > cur4; i--)
                {
                    //移除节点前先断开所有连接
                    MyConnector myConnector = switchOperationNode.Output[cur4 + 1];

                    EditorViewModel editorViewModel = Ioc.Default.GetRequiredService<DesignViewModel>().DesignModel.EditorViewModel;
                    editorViewModel.DisconnectConnectorCommand.Execute(myConnector);


                    switchOperationNode.Output.RemoveAt(cur4 + 1);
                }

            }




            //过多移除
            // MyConnector myConnector = new($"输出{i}", switchOperationNode.Id, ConnectorTypeEnum.Output)
            // {
            //     Value = switchOperationNode.OutputParams
            // };
            // switchOperationNode.Output.Add(myConnector);
        }
    }
}