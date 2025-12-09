using Pkn_HostSystem.Base;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.NodifyControl.Nodes;
using Pkn_HostSystem.NodifyControl.Operations.Core;
using Pkn_HostSystem.NodifyControl.Views.NodeOperation;
using Pkn_HostSystem.Static;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.Operations.MiddleOperation
{
    public class ModbusTcpOperation(ModbusTcpOperationNode node) : BaseOperation<ModbusTcpOperationNode>(node, new ModbusTcpOperationUserControl())
    {


        protected override async Task OnExecute()
        {

            await SendModbus();


        }

        public async Task SendModbus()
        {
            // { "01读线圈", "02读输入状态", "03读保持寄存器", "04读输入寄存器", "05写单线圈", "06写单寄存器", "0F写多线圈", "10写多寄存器" };
            NetWorkTriggerModel modelNetWorkTriggerModel = node.Model.NetWorkTriggerModel;
            NetWork netWork = GlobalManager.GetNetWork(node.Model.NetWorkTriggerModel.NetworkName);

            ModbusBase ModbusBase = netWork.ModbusBase;

            switch (modelNetWorkTriggerModel.NetMethodName)
            {
                case "01读线圈":
                    bool[] coils01 = null;
                    try
                    {
                        coils01 = await ModbusBase.ReadCoils_01(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress), 
                            ushort.Parse(node.Model.NetWorkTriggerModel.Count)
                            );
                    }
                    catch (Exception exception)
                    {
                        break;
                    }

                    if (coils01 != null) readDGV(coils01);
                    break;
                case "02读输入状态":
                    bool[] inputs02 = null;
                    try
                    {
                        inputs02 = await ModbusBase.ReadInputs_02(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.Count)
                            );
                    }
                    catch (Exception exception)
                    {
                        break;
                    }

                    if (inputs02 != null) readDGV(inputs02);
                    break;
                case "03读保持寄存器":
                    ushort[] holdingRegisters03 = null;
                    try
                    {
                        holdingRegisters03 = await ModbusBase.ReadHoldingRegisters_03(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.Count)
                            );
                    }
                    catch (Exception exception)
                    {
                        break;
                    }

                    if (holdingRegisters03 != null) readDGV(holdingRegisters03);
                    break;
                case "04读输入寄存器":
                    ushort[] readInputRegisters04 = null;
                    try
                    {
                        readInputRegisters04 = await ModbusBase.ReadInputRegisters_04(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.Count)
                            );
                    }
                    catch (Exception exception)
                    {
                        break;
                    }

                    if (readInputRegisters04 != null) readDGV(readInputRegisters04);
                    break;
                case "05写单线圈":
                    try
                    {
                        await ModbusBase.WriteCoil_05(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            (bool)node.Model.NetWorkTriggerModel.WriteDvgList[0].Value
                            );
                    }
                    catch (Exception exception)
                    {
                    }
                    break;
                case "06写单寄存器":
                    try
                    {
                        await ModbusBase.WriteRegister_06(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress), 
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            ushort.Parse((string)node.Model.NetWorkTriggerModel.WriteDvgList[0].Value.ToString()));
                    }
                    catch (Exception exception)
                    {
                    }
                    break;
                case "0F写多线圈":
                    try
                    {
                        var coils = new List<bool>();
                        foreach (var modbusPojo in Enumerable.ToArray<ModbusToolPojo<object>>(node.Model.NetWorkTriggerModel.WriteDvgList))
                            coils.Add((bool)modbusPojo.Value);

                        await ModbusBase.WriteCoils_0F(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            coils.ToArray()
                        );
                    }
                    catch (Exception exception)
                    {
                    }
                    break;
                case "10写多寄存器":
                    var registers = new List<ushort>();
                    try
                    {
                        foreach (ModbusToolPojo<object> modbusPojo in Enumerable.ToArray<ModbusToolPojo<object>>(node.Model.NetWorkTriggerModel.WriteDvgList))
                            registers.Add(ushort.Parse(modbusPojo.Value.ToString()));

                        await ModbusBase.WriteRegisters_10(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            registers.ToArray()
                        );
                    }
                    catch (Exception exception)
                    {
                    }
                    break;
            }
        }

        #region 显示读DGV

        public void readDGV<T>(T[] value)
        {
            var address = ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress);
            var modbusPojos = value.Select((b, index) => new ModbusToolPojo<object>
            { Address = address++, Value = b }).ToList();
            node.Model.NetWorkTriggerModel.ReadDvgList = modbusPojos;
        }

        #endregion

        public override FrameworkElement GetConfigView()
        {
            view.DataContext = node;
            return view;
        }
    }
}