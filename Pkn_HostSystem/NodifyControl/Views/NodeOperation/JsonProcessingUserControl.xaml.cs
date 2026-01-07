using Pkn_HostSystem.NodifyControl.Nodes;
using Pkn_HostSystem.NodifyControl.OperationModels.Models.Core;
using System.Windows.Controls;

namespace Pkn_HostSystem.NodifyControl.Views.NodeOperation
{
    /// <summary>
    /// JsonProcessingUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class JsonProcessingUserControl : UserControl
    {
        public JsonProcessingUserControl()
        {
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            JsonOperationNode? node = DataContext as JsonOperationNode;


            switch (node.Model.JsonMethod)
            {
                case "路径解析":
                    if (node.Model.OldJsonMethod == node.Model.JsonMethod)
                    {
                        break;
                    }
                    node.InputParams.Clear();
                    node.OutputParams.Clear();

                    node.InputParams.Add(new OperationModel()
                    {
                        Name = "传入Json字符串",
                        NameReadOnly = true,
                    });

                    node.InputParams.Add(new OperationModel()
                    {
                        Name = "路径解析",
                        NoDelete = true,
                        NameReadOnly = true,
                    });


                    node.OutputParams.Add(new OperationModel()
                    {
                        Name = "解析结果",
                        NoDelete = true,
                        NameReadOnly = true,
                        MethodReadOnly = true,
                        ValueReadOnly = true
                    });
                    node.OutputParams.Add(new OperationModel()
                    {
                        Name = "解析内容返回",
                        NoDelete = true,
                        NameReadOnly = true,
                        MethodReadOnly = true,
                        ValueReadOnly = true
                    });
                    node.Model.OldJsonMethod = "路径解析";
                    break;
                case "解析数组":
                    if (node.Model.OldJsonMethod == node.Model.JsonMethod)
                    {
                        break;
                    }
                    node.InputParams.Clear();
                    node.OutputParams.Clear();

                    node.InputParams.Add(new OperationModel()
                    {
                        Name = "传入Json字符串",
                        NameReadOnly = true,
                    });

                    node.InputParams.Add(new OperationModel()
                    {
                        Name = "解析数组",
                        NoDelete = true,
                        NameReadOnly = true,
                    });


                    node.OutputParams.Add(new OperationModel()
                    {
                        Name = "解析结果",
                        NoDelete = true,
                        NameReadOnly = true,
                        MethodReadOnly = true,
                        ValueReadOnly = true
                    });
                    node.OutputParams.Add(new OperationModel()
                    {
                        Name = "解析数组数量",
                        NoDelete = true,
                        NameReadOnly = true,
                        MethodReadOnly = true,
                        ValueReadOnly = true
                    });
                    node.Model.OldJsonMethod = "解析数组";
                    break;
            }
        }
    }
}