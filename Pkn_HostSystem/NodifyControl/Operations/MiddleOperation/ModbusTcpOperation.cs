using Pkn_HostSystem.Base;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.Nodes;
using Pkn_HostSystem.NodifyControl.OperationModels.Models.Core;
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
                        Log.Info("执行01读线圈");
                        coils01 = await ModbusBase.ReadCoils_01(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.Count)
                            );

                        Log.Info("执行01线圈完成");
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"执行01读线圈错误:{exception}");
                        break;
                    }

                    if (coils01 != null) ReadDvg(coils01);
                    break;
                case "02读输入状态":
                    bool[] inputs02 = null;
                    try
                    {
                        Log.Info("执行02读输入状态");
                        inputs02 = await ModbusBase.ReadInputs_02(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.Count)
                            );
                        Log.Info("执行02读输入状态完成");
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"执行02读输入状态错误:{exception}");
                        break;
                    }

                    if (inputs02 != null) ReadDvg(inputs02);
                    break;
                case "03读保持寄存器":
                    ushort[] holdingRegisters03 = null;
                    try
                    {
                        Log.Info("执行03读保持寄存器");
                        holdingRegisters03 = await ModbusBase.ReadHoldingRegisters_03(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.Count)
                            );
                        Log.Info("执行03读保持寄存器完成");
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"执行03读保持寄存器错误:{exception}");
                        break;
                    }

                    if (holdingRegisters03 != null) ReadDvg(holdingRegisters03);
                    break;
                case "04读输入寄存器":
                    ushort[] readInputRegisters04 = null;
                    try
                    {
                        Log.Info("执行04读输入寄存器");
                        readInputRegisters04 = await ModbusBase.ReadInputRegisters_04(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.Count)
                            );
                        Log.Info("执行04读输入寄存器完成");
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"执行04读输入寄存器错误:{exception}");
                        break;
                    }
                    if (readInputRegisters04 != null) ReadDvg(readInputRegisters04);
                    break;
                case "05写单线圈":
                    try
                    {
                        Log.Info("执行05写单线圈");
                        await ModbusBase.WriteCoil_05(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            bool.Parse(
                                GetParamValue(node.Model.NetWorkTriggerModel.WriteDvgList[0])
                               )
                            );
                        Log.Info("执行05写单线圈完成");
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"执行05写单线圈错误:{exception}");
                    }
                    break;
                case "06写单寄存器":
                    try
                    {
                        Log.Info("执行06写单寄存器");
                        await ModbusBase.WriteRegister_06(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            ushort.Parse(GetParamValue(node.Model.NetWorkTriggerModel.WriteDvgList[0])));
                        Log.Info("执行06写单寄存器完成");
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"执行06写单寄存器错误:{exception}");
                    }
                    break;
                case "0F写多线圈":
                    try
                    {
                        Log.Info("执行0F写多线圈");
                        var coils = new List<bool>();
                        foreach (var modbusPojo in node.Model.NetWorkTriggerModel.WriteDvgList)
                            coils.Add(bool.Parse(GetParamValue(modbusPojo)));

                        await ModbusBase.WriteCoils_0F(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            coils.ToArray()
                        );

                        Log.Info("执行0F写多线圈完成");
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"执行0F写多线圈错误:{exception}");
                    }
                    break;
                case "10写多寄存器":
                    var registers = new List<ushort>();
                    try
                    {
                        Log.Info("执行10写多寄存器");
                        foreach (var modbusPojo in node.Model.NetWorkTriggerModel.WriteDvgList)
                            registers.Add(ushort.Parse(GetParamValue(modbusPojo)));

                        await ModbusBase.WriteRegisters_10(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            registers.ToArray()
                        );

                        Log.Info("执行10写多寄存器完成");
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"执行10写多寄存器错误:{exception}");
                    }
                    break;
            }
        }

        #region 显示读DGV

        public void ReadDvg<T>(T[] value)
        {
            var StartAddress = ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress);

            ushort curAddress = StartAddress;
            foreach (OperationModel nodeOutputParam in node.OutputParams)
            {
                if (nodeOutputParam.Name.Equals(curAddress.ToString()))
                {
                    nodeOutputParam.ParamValue = value[curAddress - StartAddress].ToString();
                }
                curAddress++;
            }

        }

        #endregion

        public override FrameworkElement GetConfigView()
        {
            view.DataContext = node;
            return view;
        }
    }
}