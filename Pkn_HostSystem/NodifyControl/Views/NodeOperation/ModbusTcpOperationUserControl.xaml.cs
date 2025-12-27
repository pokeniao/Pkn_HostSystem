using DynamicData.Binding;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.Nodes;
using Pkn_HostSystem.NodifyControl.Nodes.Core;
using Pkn_HostSystem.NodifyControl.OperationModels.Models.Core;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.Views.UserControls.OperationDataGrid;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;

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
                        FormatComboBox.Visibility = Visibility.Collapsed;
                        ReadDvgView();
                        break;
                    case "02读输入状态":
                        NumberBox.IsEnabled = true;
                        FormatComboBox.Visibility = Visibility.Collapsed;
                        ReadDvgView();
                        break;
                    case "03读保持寄存器":
                        NumberBox.IsEnabled = true;
                        FormatComboBox.Visibility = Visibility.Visible;
                        ReadDvgView();
                        break;
                    case "04读输入寄存器":
                        NumberBox.IsEnabled = true;
                        FormatComboBox.Visibility = Visibility.Visible;
                        ReadDvgView();
                        break;
                    case "05写单线圈":
                        model.NetWorkTriggerModel.Count = "1";
                        FormatComboBox.Visibility = Visibility.Collapsed;
                        NumberBox.IsEnabled = false;
                        WriteDvgView1<bool>();
                        break;
                    case "06写单寄存器":
                        model.NetWorkTriggerModel.Count = "1";
                        FormatComboBox.Visibility = Visibility.Collapsed;
                        NumberBox.IsEnabled = false;
                        WriteDvgView1<ushort>();
                        break;
                    case "0F写多线圈":
                        NumberBox.IsEnabled = true;
                        FormatComboBox.Visibility = Visibility.Collapsed;
                        WriteDvgView1<bool>();
                        break;
                    case "10写多寄存器":
                        NumberBox.IsEnabled = true;
                        FormatComboBox.Visibility = Visibility.Visible;
                        WriteDvgView<ushort>();
                        break;
                }
        }

        private void WriteDvgView<A>()
        {
            ModbusTcpOperationNode modbusTcpOperationNode = (ModbusTcpOperationNode)DataContext;
            var model = modbusTcpOperationNode?.Model;

            if (model.NetWorkTriggerModel.Format == "单寄存器(无符号)" || model.NetWorkTriggerModel.Format == "单寄存器(有符号)")
            {
                WriteDvgView1<A>();
            }

            else if (model.NetWorkTriggerModel.Format == "双寄存器;无符号;BigEndian" ||
                     model.NetWorkTriggerModel.Format == "双寄存器;无符号;LittleEndian" ||
                     model.NetWorkTriggerModel.Format == "双寄存器;无符号;BigEndianByteSwap" ||
                     model.NetWorkTriggerModel.Format == "双寄存器;无符号;LittleEndianByteSwap" ||
                     model.NetWorkTriggerModel.Format == "双寄存器;有符号;BigEndian" ||
                     model.NetWorkTriggerModel.Format == "双寄存器;有符号;LittleEndian" ||
                     model.NetWorkTriggerModel.Format == "双寄存器;有符号;BigEndianByteSwap" ||
                     model.NetWorkTriggerModel.Format == "双寄存器;有符号;LittleEndianByteSwap" ||
                     model.NetWorkTriggerModel.Format == "32位浮点数;BigEndian" ||
                     model.NetWorkTriggerModel.Format == "32位浮点数;LittleEndian" ||
                     model.NetWorkTriggerModel.Format == "32位浮点数;BigEndianByteSwap" ||
                     model.NetWorkTriggerModel.Format == "32位浮点数;LittleEndianByteSwap"
                    )
            {
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
                for (int i = 0; i < int.Parse(model.NetWorkTriggerModel.Count); i++)
                {
                    if (i >= model.NetWorkTriggerModel.WriteDvgList.Count)
                    {
                        model.NetWorkTriggerModel.WriteDvgList.Add(
                            new OperationModel() { Name = (startAddress + i * 2).ToString(), ParamValue = "0" }
                        );
                    }
                    else
                    {
                        model.NetWorkTriggerModel.WriteDvgList[i].Name = (startAddress + i * 2).ToString();
                        if (!int.TryParse(model.NetWorkTriggerModel.WriteDvgList[i].ParamValue, out _))
                        {
                            model.NetWorkTriggerModel.WriteDvgList[i].ParamValue = "0";
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
            else if (model.NetWorkTriggerModel.Format == "ASCII字符串(高低位)" ||
                     model.NetWorkTriggerModel.Format == "ASCII字符串(低高位)"
                    )
            {
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
                //写也清除只留一下
                for (int i = model.NetWorkTriggerModel.WriteDvgList.Count; i > 1; i--)
                {
                    model.NetWorkTriggerModel.WriteDvgList.RemoveAt(model.NetWorkTriggerModel.WriteDvgList.Count - 1);
                }

                for (int i = 0; i < 1; i++)
                {
                    if (i >= model.NetWorkTriggerModel.WriteDvgList.Count)
                    {
                        model.NetWorkTriggerModel.WriteDvgList.Add(
                            new OperationModel() { Name = (startAddress).ToString(), ParamValue = "0" }
                        );
                    }
                    else
                    {
                        model.NetWorkTriggerModel.WriteDvgList[i].Name = (startAddress).ToString();
                        if (!int.TryParse(model.NetWorkTriggerModel.WriteDvgList[i].ParamValue, out _))
                        {
                            model.NetWorkTriggerModel.WriteDvgList[i].ParamValue = "0";
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
        }

        private void WriteDvgView1<A>()
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
                            new OperationModel() { Name = (startAddress + i).ToString(), ParamValue = "0" }
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
            if (FormatComboBox.Visibility == Visibility.Visible)
            {
                if (model.NetWorkTriggerModel.Format == "单寄存器(无符号)" || model.NetWorkTriggerModel.Format == "单寄存器(有符号)")
                {
                    ReadDvgView1();
                }

                else if (model.NetWorkTriggerModel.Format == "双寄存器;无符号;BigEndian" ||
                         model.NetWorkTriggerModel.Format == "双寄存器;无符号;LittleEndian" ||
                         model.NetWorkTriggerModel.Format == "双寄存器;无符号;BigEndianByteSwap" ||
                         model.NetWorkTriggerModel.Format == "双寄存器;无符号;LittleEndianByteSwap" ||
                         model.NetWorkTriggerModel.Format == "双寄存器;有符号;BigEndian" ||
                         model.NetWorkTriggerModel.Format == "双寄存器;有符号;LittleEndian" ||
                         model.NetWorkTriggerModel.Format == "双寄存器;有符号;BigEndianByteSwap" ||
                         model.NetWorkTriggerModel.Format == "双寄存器;有符号;LittleEndianByteSwap" ||
                         model.NetWorkTriggerModel.Format == "32位浮点数;BigEndian" ||
                         model.NetWorkTriggerModel.Format == "32位浮点数;LittleEndian" ||
                         model.NetWorkTriggerModel.Format == "32位浮点数;BigEndianByteSwap" ||
                         model.NetWorkTriggerModel.Format == "32位浮点数;LittleEndianByteSwap"
                        )
                {
                    ReadDvgView2();
                }
                else if (model.NetWorkTriggerModel.Format == "ASCII字符串(高低位)" ||
                         model.NetWorkTriggerModel.Format == "ASCII字符串(低高位)"
                        )
                {
                    ReadDvgView3();
                }
            }
            else
            {
                ReadDvgView1();
            }
        }

        private void ReadDvgView1()
        {
            ModbusTcpOperationNode modbusTcpOperationNode = (ModbusTcpOperationNode)DataContext;
            var model = modbusTcpOperationNode?.Model;

            if (model.NetWorkTriggerModel.LastFormat != "1")
            {
                var observableCollectionExtended = modbusTcpOperationNode.OutputParams;
                for (int i = 0; i < observableCollectionExtended.Count; i++)
                {
                    //判断,输入不可删除的,全部删除
                    if (observableCollectionExtended[i].NoDelete)
                    {
                        observableCollectionExtended.RemoveAt(i);
                        i--;
                    }
                }
            }

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
                        new OperationModel() { Name = (startAddress + i).ToString(), ReadOnly = true, NoDelete = true }
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
                        pendingOutputParamList.Add(modbusTcpOperationNode.OutputParams[i]); //添加到预备处理中
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


            model.NetWorkTriggerModel.LastFormat = "1";
        }

        private void ReadDvgView2()
        {
            ModbusTcpOperationNode modbusTcpOperationNode = (ModbusTcpOperationNode)DataContext;
            var model = modbusTcpOperationNode?.Model;

            if (model.NetWorkTriggerModel.LastFormat != "2")
            {
                var observableCollectionExtended = modbusTcpOperationNode.OutputParams;
                for (int i = 0; i < observableCollectionExtended.Count; i++)
                {
                    //判断,输入不可删除的,全部删除
                    if (observableCollectionExtended[i].NoDelete)
                    {
                        observableCollectionExtended.RemoveAt(i);
                        i--;
                    }
                }
            }

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
                        new OperationModel()
                        {
                            Name = (startAddress + i * 2).ToString(),
                            ReadOnly = true,
                            NoDelete = true
                        }
                    );
                }
                else
                {
                    //判断,输入不可删除的话

                    if (modbusTcpOperationNode.OutputParams[i].NoDelete)
                    {
                        modbusTcpOperationNode.OutputParams[i].Name = (startAddress + (i * 2)).ToString();
                    }
                    else
                    {
                        pendingOutputParamList.Add(modbusTcpOperationNode.OutputParams[i]); //添加到预备处理中
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


            model.NetWorkTriggerModel.LastFormat = "2";
        }

        private void ReadDvgView3()
        {
            ModbusTcpOperationNode modbusTcpOperationNode = (ModbusTcpOperationNode)DataContext;
            var model = modbusTcpOperationNode?.Model;

            if (model.NetWorkTriggerModel.LastFormat != "3")
            {
                var observableCollectionExtended = modbusTcpOperationNode.OutputParams;
                for (int i = 0; i < observableCollectionExtended.Count; i++)
                {
                    //判断,输入不可删除的,全部删除
                    if (observableCollectionExtended[i].NoDelete)
                    {
                        observableCollectionExtended.RemoveAt(i);
                        i--;
                    }
                }
            }

            int startAddress = int.Parse(model.NetWorkTriggerModel.StartAddress);
            List<OperationModel> pendingOutputParamList = new();
            //计数读多少
            int cur = 0;
            for (int i = 0; i < 1; i++)
            {
                if (i >= modbusTcpOperationNode.OutputParams.Count)
                {
                    //添加
                    modbusTcpOperationNode.OutputParams.Add(
                        new OperationModel() { Name = (startAddress).ToString(), ReadOnly = true, NoDelete = true }
                    );
                }
                else
                {
                    //判断,输入不可删除的话

                    if (modbusTcpOperationNode.OutputParams[i].NoDelete)
                    {
                        modbusTcpOperationNode.OutputParams[i].Name = (startAddress).ToString();
                    }
                    else
                    {
                        pendingOutputParamList.Add(modbusTcpOperationNode.OutputParams[i]); //添加到预备处理中
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


            model.NetWorkTriggerModel.LastFormat = "3";
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

        //通讯地址改变的时候
        private async void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NetNameComboBox.SelectedIndex == -1)
            {
                return;
            }

            if (NetNameComboBox.SelectedValue == null)
            {
                return;
            }

            string? NetName = NetNameComboBox.SelectedValue.ToString();
            NetWork netWork = GlobalManager.GetNetWork(NetName);

            if (netWork == null)
            {
                NetNameComboBox.SelectedIndex = -1;
                await new MessageBox() { Content = "选中通讯不为ModbusTcp或ModbusRtu,或通讯处于关闭" }.ShowDialogAsync();
                return;
            }

            if (!(netWork.NetworkDetailed.NetMethod == "ModbusTcp" || netWork.NetworkDetailed.NetMethod == "ModbusRtu"))
            {
                NetNameComboBox.SelectedIndex = -1;
                await new MessageBox() { Content = "选中通讯不为ModbusTcp或ModbusRtu" }.ShowDialogAsync();
                return;
            }
        }
    }
}