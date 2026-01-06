using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.Nodes;
using Pkn_HostSystem.NodifyControl.OperationModels.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pkn_HostSystem.NodifyControl.Views.NodeOperation
{
    /// <summary>
    /// StringOperationUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class StringOperationUserControl : UserControl
    {
        public StringOperationUserControl()
        {
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StringOperationNode? node = DataContext as StringOperationNode;

            switch (node.Model.Method)      
            {
                case "拼接":

                    if ((node.Model.OldMethod == "拼接"))
                    {
                        break;
                    }
                    //输入
                    node.InputParams.Clear();
                    //输出
                    node.OutputParams.Clear();
                    node.OutputParams.Add(new OperationModel()
                    {
                        Name = "拼接结果",
                        NoDelete = true,
                        NameReadOnly = true,
                        MethodReadOnly = true,
                        ValueReadOnly = true,
                    });

                    node.Model.OldMethod = "拼接";
                    break;
                case "分割":
                    if ((node.Model.OldMethod == "分割"))
                    {
                        break;
                    }
                    //输入
                    node.InputParams.Clear();
                    node.InputParams.Add(new OperationModel()
                    {
                        Name = "分割String",
                        NoDelete = true,
                        NameReadOnly = true
                    });
                    node.InputParams.Add(new OperationModel()
                    {
                        Name = "分隔符",
                        NoDelete = true,
                        NameReadOnly = true
                    });
                    //输出
                    node.OutputParams.Clear();
                    //预留100个
                    node.OutputParams.AddRange(Enumerable.Range(0, 100)
                        .Select(index => new OperationModel()
                        {
                            Name = "分割"+index,
                            NoDelete = true,
                            NameReadOnly = true,
                            ValueReadOnly = true,
                            MethodReadOnly = true
                        }));

                    node.Model.OldMethod = "分割";
                    break;
                case "切割":
                    if ((node.Model.OldMethod == "切割"))
                    {
                        break;
                    }
                    //输入
                    node.InputParams.Clear();
                    node.InputParams.Add(new OperationModel()
                    {
                        Name = "切割String",
                        NoDelete = true,
                        NameReadOnly = true
                    });

                    node.InputParams.Add(new OperationModel()
                    {
                        Name = "切割字符串",
                        NoDelete = true,
                        NameReadOnly = true
                    });
                    node.InputParams.Add(new OperationModel()
                    {
                        Name = "切割数量",
                        NoDelete = true,
                        NameReadOnly = true
                    });

                    //输出
                    node.OutputParams.Clear();
                    node.OutputParams.Add(new OperationModel()
                    {
                        Name = "切割结果",
                        NoDelete = true,
                        NameReadOnly = true,
                        ValueReadOnly = true,
                        MethodReadOnly = true
                    });

                    node.Model.OldMethod = "切割";
                    break;

                case "索引":
                    if ((node.Model.OldMethod == "索引"))
                    {
                        break;
                    }
                    //输入
                    node.InputParams.Clear();
                    node.InputParams.Add(new OperationModel()
                    {
                        Name = "索引String",
                        NoDelete = true,
                        NameReadOnly = true
                    });

                    node.InputParams.Add(new OperationModel()
                    {
                        Name = "索引字符",
                        NoDelete = true,
                        NameReadOnly = true
                    });

                    //输出
                    node.OutputParams.Clear();
                    node.OutputParams.Add(new OperationModel()
                    {
                        Name = "索引地址",
                        NoDelete = true,
                        NameReadOnly = true,
                        ValueReadOnly = true,
                        MethodReadOnly = true
                    });

                    node.Model.OldMethod = "索引";
                    break;
                case "索引(倒序)":
                    if ((node.Model.OldMethod == "索引(倒序)"))
                    {
                        break;
                    }
                    //输入
                    node.InputParams.Clear();
                    node.InputParams.Add(new OperationModel()
                    {
                        Name = "索引String",
                        NoDelete = true,
                        NameReadOnly = true
                    });

                    node.InputParams.Add(new OperationModel()
                    {
                        Name = "索引字符",
                        NoDelete = true,
                        NameReadOnly = true
                    });

                    //输出
                    node.OutputParams.Clear();
                    node.OutputParams.Add(new OperationModel()
                    {
                        Name = "索引地址",
                        NoDelete = true,
                        NameReadOnly = true,
                        ValueReadOnly = true,
                        MethodReadOnly = true
                    });

                    node.Model.OldMethod = "索引(倒序)";
                    break;
            }
        }
    }
}
