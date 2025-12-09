using DynamicData;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.Nodes;
using Pkn_HostSystem.NodifyControl.OperationModels.Models;
using Pkn_HostSystem.Static;
using System.Collections.ObjectModel;
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
        private void NumberBox_ValueChanged(object sender, Wpf.Ui.Controls.NumberBoxValueChangedEventArgs args)
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
            if (typeof(A) == typeof(bool))
            {
                // model.NetWorkTriggerModel.WriteDvgList.Clear();
              
                for (int i = 0; i < int.Parse(model.NetWorkTriggerModel.Count); i++)
                {
                    if (i>= model.NetWorkTriggerModel.WriteDvgList.Count)
                    {
                        model.NetWorkTriggerModel.WriteDvgList.Add(new ModbusToolPojo<object>() { Address = startAddress + i, Value = false, valueIsBool = true });
                    }
                    else
                    {
                        model.NetWorkTriggerModel.WriteDvgList[i].Address = startAddress + i;
                    }
                }

            }
            else
            {
                // model.NetWorkTriggerModel.WriteDvgList.Clear();
                for (int i = 0; i < int.Parse(model.NetWorkTriggerModel.Count); i++)
                {
                    if (i >= model.NetWorkTriggerModel.WriteDvgList.Count)
                    {
                        model.NetWorkTriggerModel.WriteDvgList.Add(new ModbusToolPojo<object>() { Address = startAddress + i, Value = (A)(object)(ushort)0, valueIsBool = false });
                    }
                    else
                    {
                        model.NetWorkTriggerModel.WriteDvgList[i].Address = startAddress + i;
                    }

                }
            }
            if (model.NetWorkTriggerModel.WriteDvgList.Count > int.Parse(model.NetWorkTriggerModel.Count))
            {
                int count = model.NetWorkTriggerModel.WriteDvgList.Count-1;
                for (int i = int.Parse(model.NetWorkTriggerModel.Count)-1; i < count; i++)
                {
                    model.NetWorkTriggerModel.WriteDvgList.RemoveAt(int.Parse(model.NetWorkTriggerModel.Count));
                }
            }
        }


        // private ObservableCollection<ModbusToolPojo<object>> WriteView<A>()
        // {
        //     ModbusTcpOperationNode modbusTcpOperationNode = (ModbusTcpOperationNode)DataContext;
        //     var model = modbusTcpOperationNode?.Model;
        //     ObservableCollection<ModbusToolPojo<object>> bindingList = new ObservableCollection<ModbusToolPojo<object>>();
        //     if (typeof(A) == typeof(bool))
        //     {
        //         int key = int.Parse(model.NetWorkTriggerModel.StartAddress);
        //         for (int i = 0; i < int.Parse(model.NetWorkTriggerModel.Count); i++)
        //         {
        //             bindingList.Add(new ModbusToolPojo<object>() { address = key++, value = (object)false, valueIsBool = true });
        //         }
        //
        //         return bindingList;
        //     }
        //     else
        //     {
        //         int currentAddress = int.Parse(model.NetWorkTriggerModel.StartAddress);
        //         for (int i = 0; i < int.Parse(model.NetWorkTriggerModel.Count); i++)
        //         {
        //             bindingList.Add(new ModbusToolPojo<object>()
        //             { address = currentAddress, value = (A)(object)(ushort)0, valueIsBool = false });
        //             currentAddress++;
        //         }
        //         return bindingList;
        //     }
        // }
    }
}
