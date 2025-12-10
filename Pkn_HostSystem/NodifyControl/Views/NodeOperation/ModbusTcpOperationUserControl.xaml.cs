using DynamicData;
using DynamicData.Binding;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.Nodes;
using Pkn_HostSystem.NodifyControl.Nodes.Core;
using Pkn_HostSystem.NodifyControl.OperationModels.Interface;
using Pkn_HostSystem.NodifyControl.OperationModels.Models.Core;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.Views.UserControls.OperationDataGrid;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.NodifyControl.Views.NodeOperation
{
    /// <summary>
    /// ModbusTcpOperationUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ModbusTcpOperationUserControl : UserControl
    {
        public ModbusTcpOperationUserControl()
        {
            InitializeComponent();
        }


        #region 数据改变需要刷新WriteDgv
        //数量改变
        private void NumberBox_ValueChanged(object sender, NumberBoxValueChangedEventArgs args)
        {

            refreshWriteDgv();
        }
        #endregion

        //功能码的改变
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ModbusTcpOperationNode modbusTcpOperationNode = (ModbusTcpOperationNode)DataContext;
            var model = modbusTcpOperationNode?.Model;
            NetWork netWork = GlobalManager.GetNetWork(model.NetWorkTriggerModel.NetworkName);
            switch (netWork?.NetworkDetailed.NetMethod)
            {
                case "ModbusTcp":
                    refreshWriteDgv();
                    break;
                case "ModbusRtu":
                    refreshWriteDgv();
                    break;
            }
        }


        //刷新WriteDgv
        private void refreshWriteDgv()
        {
            ModbusTcpOperationNode modbusTcpOperationNode = (ModbusTcpOperationNode)DataContext;
            var model = modbusTcpOperationNode?.Model;
            if (model != null)
                switch (model.NetWorkTriggerModel.NetMethodName)
                {
                    case "01读线圈":
                        NumberBox.IsEnabled = true;
                        ReadDvgView();
                        break;
                    case "02读输入状态":
                        NumberBox.IsEnabled = true;
                        ReadDvgView();
                        break;
                    case "03读保持寄存器":
                        NumberBox.IsEnabled = true;
                        ReadDvgView();
                        break;
                    case "04读输入寄存器":
                        NumberBox.IsEnabled = true;
                        ReadDvgView();
                        break;
                    case "05写单线圈":
                        model.NetWorkTriggerModel.Count = "1";
                        NumberBox.IsEnabled = false;
                        WriteDvgView<bool>();
                        break;
                    case "06写单寄存器":
                        model.NetWorkTriggerModel.Count = "1";
                        NumberBox.IsEnabled = false;
                        WriteDvgView<ushort>();
                        break;
                    case "0F写多线圈":
                        NumberBox.IsEnabled = true;
                        WriteDvgView<bool>();
                        break;
                    case "10写多寄存器":
                        NumberBox.IsEnabled = true;
                        WriteDvgView<ushort>();
                        break;
                }
        }

        private void WriteDvgView<A>()
        {
            ModbusTcpOperationNode modbusTcpOperationNode = (ModbusTcpOperationNode)DataContext;
            var model = modbusTcpOperationNode?.Model;
            int startAddress = int.Parse(model.NetWorkTriggerModel.StartAddress);

            //切换到写,先清除一下输出

            for (int i = 0; i < modbusTcpOperationNode.OutputParams.Count; i++)
            {
                if (modbusTcpOperationNode.OutputParams[i].NoDelete == true)
                {
                    modbusTcpOperationNode.OutputParams.RemoveAt(i);
                    i--;
                }
            }
  



            if (typeof(A) == typeof(bool))
            {
                for (int i = 0; i < int.Parse(model.NetWorkTriggerModel.Count); i++)
                {
                    if (i >= model.NetWorkTriggerModel.WriteDvgList.Count)
                    {
                        model.NetWorkTriggerModel.WriteDvgList.Add(

                            new OperationModel() { Name = (startAddress + i).ToString(), ParamValue = "False" });
                    }
                    else
                    {
                        model.NetWorkTriggerModel.WriteDvgList[i].Name = (startAddress + i).ToString();
                        if (!bool.TryParse(model.NetWorkTriggerModel.WriteDvgList[i].ParamValue, out _))
                        {
                            model.NetWorkTriggerModel.WriteDvgList[i].ParamValue = "False";
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < int.Parse(model.NetWorkTriggerModel.Count); i++)
                {
                    if (i >= model.NetWorkTriggerModel.WriteDvgList.Count)
                    {
                        model.NetWorkTriggerModel.WriteDvgList.Add(
                            new OperationModel(){ Name = (startAddress + i).ToString(), ParamValue = "0" }
                            );
                    }
                    else
                    {
                        model.NetWorkTriggerModel.WriteDvgList[i].Name = (startAddress + i).ToString();

                        if (!int.TryParse(model.NetWorkTriggerModel.WriteDvgList[i].ParamValue, out _))
                        {
                            model.NetWorkTriggerModel.WriteDvgList[i].ParamValue = "0";
                        }
                    }

                }
            }
            if (model.NetWorkTriggerModel.WriteDvgList.Count > int.Parse(model.NetWorkTriggerModel.Count))
            {
                int count = model.NetWorkTriggerModel.WriteDvgList.Count - 1;
                for (int i = int.Parse(model.NetWorkTriggerModel.Count) - 1; i < count; i++)
                {
                    model.NetWorkTriggerModel.WriteDvgList.RemoveAt(int.Parse(model.NetWorkTriggerModel.Count));
                }
            }
        }


        private void ReadDvgView()
        {
            ModbusTcpOperationNode modbusTcpOperationNode = (ModbusTcpOperationNode)DataContext;
            var model = modbusTcpOperationNode?.Model;
            int startAddress = int.Parse(model.NetWorkTriggerModel.StartAddress);
            List<OperationModel> pendingOutputParamList = new();
            //计数读多少
            int cur = 0;
            for (int i = 0; i < int.Parse(model.NetWorkTriggerModel.Count); i++)
            {
                if (i >= modbusTcpOperationNode.OutputParams.Count)
                {
                    //添加
                    modbusTcpOperationNode.OutputParams.Add(
                        new OperationModel() { Name = (startAddress + i).ToString(),IsEnable = false , NoDelete = true}
                        );
                }
                else
                {
                    //判断,输入不可删除的话
                   
                    if (modbusTcpOperationNode.OutputParams[i].NoDelete)
                    {
                        modbusTcpOperationNode.OutputParams[i].Name = (startAddress + i).ToString();
                    }
                    else
                    {
                        pendingOutputParamList.Add(modbusTcpOperationNode.OutputParams[i]);//添加到预备处理中
                        modbusTcpOperationNode.OutputParams.RemoveAt(i);
                        i--;
                    }
                }
            }
            //计数有多少数组
            for (int i = 0; i < modbusTcpOperationNode.OutputParams.Count; i++)
            {
                if (modbusTcpOperationNode.OutputParams[i].NoDelete)
                {
                    cur++;
                }
            }

            if (cur > int.Parse(model.NetWorkTriggerModel.Count))
            {
                int count = cur - int.Parse(model.NetWorkTriggerModel.Count);
                for (int i = 0; i < count; i++)
                {
                    modbusTcpOperationNode.OutputParams.RemoveAt(int.Parse(model.NetWorkTriggerModel.Count));
                }
            }


            //将自己添加的放到最后面
            if (pendingOutputParamList.Count != 0)
            {
                modbusTcpOperationNode.OutputParams.AddRange(pendingOutputParamList);
            }

        }

        public ObservableCollectionExtended<OperationModel> InputParams2
        {
            get => (ObservableCollectionExtended<OperationModel>)GetValue(InputParams2Property);
            set => SetValue(InputParams2Property, value);
        }

        public static readonly DependencyProperty InputParams2Property =
            DependencyProperty.Register(
                nameof(InputParams2),
                typeof(ObservableCollectionExtended<OperationModel>),
                typeof(PknOperationDataGrid),
                new FrameworkPropertyMetadata(new ObservableCollectionExtended<OperationModel>()));

        private void ComboBox_OnDropDownOpened(object? sender, EventArgs e)
        {
            //获取全部接入
            PknNode? Node = DataContext as PknNode;
            var myConnectors = Node.Input;
            InputParams2.Clear();
            if (Node.Input == null)
            {
                InputParams2.Clear();
                return;
            }
            foreach (var connector in myConnectors)
            {
                if (connector == null)
                {
                    continue;
                }
                List<ObservableCollection<OperationModel>> myConnectorInputValue = connector.InputValue;
                foreach (var observableCollection in myConnectorInputValue)
                {
                    if (observableCollection == null)
                    {
                        return;
                    }

                    InputParams2.AddRange(observableCollection);
                }
            }
        }

    }
}
