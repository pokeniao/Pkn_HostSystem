using Azure;
using DynamicData;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using Nodify;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.Nodes;
using Pkn_HostSystem.NodifyControl.OperationModels.Models.Core;
using Pkn_HostSystem.NodifyControl.Operations.Core;
using Pkn_HostSystem.NodifyControl.Views.NodeOperation;
using Pkn_HostSystem.Static;
using System.Globalization;
using System.Text;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.Operations.MiddleOperation
{
    public class ModbusTcpOperation(ModbusTcpOperationNode node)
        : BaseOperation<ModbusTcpOperationNode>(node, new ModbusTcpOperationUserControl())
    {
        protected override async Task OnExecute(CancellationTokenSource cts)
        {
            await SendModbus();
        }

        public async Task SendModbus()
        {
            // { "01读线圈", "02读输入状态", "03读保持寄存器", "04读输入寄存器", "05写单线圈", "06写单寄存器", "0F写多线圈", "10写多寄存器" };
            NetWorkTriggerModel modelNetWorkTriggerModel = node.Model.NetWorkTriggerModel;
            NetWork netWork = GlobalManager.GetNetWork(node.Model.NetWorkTriggerModel.NetworkName);
            if (netWork == null)
            {
                Log.Error("通讯未连接,未找到netWork", $"{node.NodeName}:{node.Id}");
                return;
            }

            ModbusBase ModbusBase = netWork.ModbusBase;

            switch (modelNetWorkTriggerModel.NetMethodName)
            {
                case "01读线圈":
                    bool[] coils01 = null;
                    try
                    {
                        Log.Info("执行01读线圈", $"{node.NodeName}:{node.Id}");
                        coils01 = await ModbusBase.ReadCoils_01(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.Count)
                        );

                        Log.Info("执行01读线圈完成", $"{node.NodeName}:{node.Id}");
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"执行01读线圈错误:{exception}", $"{node.NodeName}:{node.Id}");
                        break;
                    }

                    if (coils01 != null) ReadDvg(coils01);
                    break;
                case "02读输入状态":
                    bool[] inputs02 = null;
                    try
                    {
                        Log.Info("执行02读输入状态", $"{node.NodeName}:{node.Id}");
                        inputs02 = await ModbusBase.ReadInputs_02(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.Count)
                        );
                        Log.Info("执行02读输入状态完成", $"{node.NodeName}:{node.Id}");
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"执行02读输入状态错误:{exception}", $"{node.NodeName}:{node.Id}");
                        break;
                    }

                    if (inputs02 != null) ReadDvg(inputs02);
                    break;
                case "03读保持寄存器":
                    await ReadReg();
                    break;
                case "04读输入寄存器":
                    await ReadReg();
                    break;
                case "05写单线圈":
                    try
                    {
                        Log.Info("执行05写单线圈", $"{node.NodeName}:{node.Id}");
                        await ModbusBase.WriteCoil_05(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            bool.Parse(
                                GetParamValue(node.Model.NetWorkTriggerModel.WriteDvgList[0])
                            )
                        );
                        Log.Info("执行05写单线圈完成", $"{node.NodeName}:{node.Id}");
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"执行05写单线圈错误:{exception}", $"{node.NodeName}:{node.Id}");
                    }

                    break;
                case "06写单寄存器":
                    try
                    {
                        Log.Info("执行06写单寄存器", $"{node.NodeName}:{node.Id}");
                        await ModbusBase.WriteRegister_06(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            ushort.Parse(GetParamValue(node.Model.NetWorkTriggerModel.WriteDvgList[0])));
                        Log.Info("执行06写单寄存器完成", $"{node.NodeName}:{node.Id}");
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"执行06写单寄存器错误:{exception}", $"{node.NodeName}:{node.Id}");
                    }

                    break;
                case "0F写多线圈":
                    try
                    {
                        Log.Info("执行0F写多线圈", $"{node.NodeName}:{node.Id}");
                        var coils = new List<bool>();
                        foreach (var modbusPojo in node.Model.NetWorkTriggerModel.WriteDvgList)
                            coils.Add(bool.Parse(GetParamValue(modbusPojo)));

                        await ModbusBase.WriteCoils_0F(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            coils.ToArray()
                        );

                        Log.Info("执行0F写多线圈完成", $"{node.NodeName}:{node.Id}");
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"执行0F写多线圈错误:{exception}", $"{node.NodeName}:{node.Id}");
                    }

                    break;
                case "10写多寄存器":
                    var registers = new List<ushort>();
                    try
                    {
                        Log.Info("执行10写多寄存器", $"{node.NodeName}:{node.Id}");

                        await WriteReg();
                        Log.Info("执行10写多寄存器完成", $"{node.NodeName}:{node.Id}");
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"执行10写多寄存器错误:{exception}", $"{node.NodeName}:{node.Id}");
                    }

                    break;
            }
        }

        public async Task ReadReg()
        {
            NetWorkTriggerModel modelNetWorkTriggerModel = node.Model.NetWorkTriggerModel;
            NetWork netWork = GlobalManager.GetNetWork(node.Model.NetWorkTriggerModel.NetworkName);
            if (netWork == null)
            {
                Log.Error("通讯未连接,未找到netWork", $"{node.NodeName}:{node.Id}");
                return;
            }

            ModbusBase modbusBase = netWork.ModbusBase;
            var result = "";
            try
            {
                //获得读寄存器值
                ushort[] readHoldingRegisters = new ushort[] { };
                if (modelNetWorkTriggerModel.NetMethodName == "03读保持寄存器")
                {
                    if (node.Model.NetWorkTriggerModel.Format == "双寄存器;无符号;BigEndian" ||
                        node.Model.NetWorkTriggerModel.Format == "双寄存器;无符号;LittleEndian" ||
                        node.Model.NetWorkTriggerModel.Format == "双寄存器;无符号;BigEndianByteSwap" ||
                        node.Model.NetWorkTriggerModel.Format == "双寄存器;无符号;LittleEndianByteSwap" ||
                        node.Model.NetWorkTriggerModel.Format == "双寄存器;有符号;BigEndian" ||
                        node.Model.NetWorkTriggerModel.Format == "双寄存器;有符号;LittleEndian" ||
                        node.Model.NetWorkTriggerModel.Format == "双寄存器;有符号;BigEndianByteSwap" ||
                        node.Model.NetWorkTriggerModel.Format == "双寄存器;有符号;LittleEndianByteSwap" ||
                        node.Model.NetWorkTriggerModel.Format == "32位浮点数;BigEndian" ||
                        node.Model.NetWorkTriggerModel.Format == "32位浮点数;LittleEndian" ||
                        node.Model.NetWorkTriggerModel.Format == "32位浮点数;BigEndianByteSwap" ||
                        node.Model.NetWorkTriggerModel.Format == "32位浮点数;LittleEndianByteSwap"
                       )
                    {
                        readHoldingRegisters = await modbusBase.ReadHoldingRegisters_03(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            (ushort)(ushort.Parse(node.Model.NetWorkTriggerModel.Count) * 2));
                    }
                    else
                    {
                        readHoldingRegisters = await modbusBase.ReadHoldingRegisters_03(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.Count));
                    }
                }
                else if (modelNetWorkTriggerModel.NetMethodName == "04读输入寄存器")
                {
                    readHoldingRegisters = await modbusBase.ReadInputRegisters_04(
                        byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                        ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                        ushort.Parse(node.Model.NetWorkTriggerModel.Count)
                    );
                }


                string[] strings;

                switch (node.Model.NetWorkTriggerModel.Format)
                {
                    case "单寄存器(无符号)":
                        //用逗号分割
                        ReadDvg(readHoldingRegisters);
                        break;
                    case "单寄存器(有符号)":
                        strings = Array.ConvertAll(readHoldingRegisters, p => $"{(short)p}");
                        ReadDvg(strings);
                        break;
                    case "双寄存器;无符号;BigEndian":
                        List<uint> uInt32List1 =
                            ModbusDoubleRegisterTool.ToUInt32List(readHoldingRegisters, ModbusEndian.BigEndian);
                        strings = Array.ConvertAll(uInt32List1.ToArray(), p => $"{p}");
                        ReadDvg(strings);
                        break;
                    case "双寄存器;无符号;LittleEndian":
                        List<uint> uInt32List2 =
                            ModbusDoubleRegisterTool.ToUInt32List(readHoldingRegisters, ModbusEndian.LittleEndian);
                        strings = Array.ConvertAll(uInt32List2.ToArray(), p => $"{p}");
                        ReadDvg(strings);
                        break;
                    case "双寄存器;无符号;BigEndianByteSwap":
                        List<uint> uInt32List3 =
                            ModbusDoubleRegisterTool.ToUInt32List(readHoldingRegisters, ModbusEndian.BigEndianByteSwap);
                        strings = Array.ConvertAll(uInt32List3.ToArray(), p => $"{p}");
                        ReadDvg(strings);
                        break;
                    case "双寄存器;无符号;LittleEndianByteSwap":
                        List<uint> uInt32List4 =
                            ModbusDoubleRegisterTool.ToUInt32List(readHoldingRegisters,
                                ModbusEndian.LittleEndianByteSwap);
                        strings = Array.ConvertAll(uInt32List4.ToArray(), p => $"{p}");
                        ReadDvg(strings);
                        break;
                    case "双寄存器;有符号;BigEndian":
                        List<int> int32List1 =
                            ModbusDoubleRegisterTool.ToInt32List(readHoldingRegisters, ModbusEndian.BigEndian);
                        strings = Array.ConvertAll(int32List1.ToArray(), p => $"{p}");
                        ReadDvg(strings);
                        break;
                    case "双寄存器;有符号;LittleEndian":
                        List<int> int32List2 =
                            ModbusDoubleRegisterTool.ToInt32List(readHoldingRegisters, ModbusEndian.LittleEndian);
                        strings = Array.ConvertAll(int32List2.ToArray(), p => $"{p}");
                        ReadDvg(strings);
                        break;
                    case "双寄存器;有符号;BigEndianByteSwap":
                        List<int> int32List3 =
                            ModbusDoubleRegisterTool.ToInt32List(readHoldingRegisters, ModbusEndian.BigEndianByteSwap);
                        strings = Array.ConvertAll(int32List3.ToArray(), p => $"{p}");
                        ReadDvg(strings);
                        break;
                    case "双寄存器;有符号;LittleEndianByteSwap":
                        List<int> int32List4 =
                            ModbusDoubleRegisterTool.ToInt32List(readHoldingRegisters,
                                ModbusEndian.LittleEndianByteSwap);
                        strings = Array.ConvertAll(int32List4.ToArray(), p => $"{p}");
                        ReadDvg(strings);
                        break;
                    case "32位浮点数;BigEndian":
                        List<float> floatList1 =
                            ModbusDoubleRegisterTool.ToFloatList(readHoldingRegisters, ModbusEndian.BigEndian);
                        strings = Array.ConvertAll(floatList1.ToArray(), p => $"{p}");
                        ReadDvg(strings);
                        break;
                    case "32位浮点数;LittleEndian":
                        List<float> floatList3 =
                            ModbusDoubleRegisterTool.ToFloatList(readHoldingRegisters, ModbusEndian.LittleEndian);
                        strings = Array.ConvertAll(floatList3.ToArray(), p => $"{p}");
                        ReadDvg(strings);
                        break;
                    case "32位浮点数;BigEndianByteSwap":
                        List<float> floatList4 =
                            ModbusDoubleRegisterTool.ToFloatList(readHoldingRegisters, ModbusEndian.BigEndianByteSwap);
                        strings = Array.ConvertAll(floatList4.ToArray(), p => $"{p}");
                        ReadDvg(strings);
                        break;
                    case "32位浮点数;LittleEndianByteSwap":
                        List<float> floatList2 =
                            ModbusDoubleRegisterTool.ToFloatList(readHoldingRegisters,
                                ModbusEndian.LittleEndianByteSwap);
                        strings = Array.ConvertAll(floatList2.ToArray(), p => $"{p}");
                        ReadDvg(strings);
                        break;
                    case "ASCII字符串(低高位)":
                        var result_3 = new List<byte>();
                        foreach (var itemUshort in readHoldingRegisters)
                        {
                            //转成16进制
                            var value = itemUshort.ToString("x4");
                            //从2索引截取到结尾
                            var low = value.Substring(2);
                            var high = value.Substring(0, 2);
                            var ByteLow = byte.Parse(low, NumberStyles.HexNumber);
                            var ByteHigh = byte.Parse(high, NumberStyles.HexNumber);

                            //低位在前
                            result_3.Add(ByteLow);
                            result_3.Add(ByteHigh);
                        }

                        //输出ASCII码转换后的结果
                        string trim = Encoding.ASCII.GetString(result_3.ToArray()).Trim('\0');
                        strings = new[] { trim };
                        ReadDvg(strings);
                        break;
                    case "ASCII字符串(高低位)":
                        var result_4 = new List<byte>();
                        foreach (var itemUshort in readHoldingRegisters)
                        {
                            //转成16进制
                            var value = itemUshort.ToString("x4");
                            //从2索引截取到结尾
                            var high = value.Substring(2);
                            var low = value.Substring(0, 2);
                            var ByteLow = byte.Parse(low, NumberStyles.HexNumber);
                            var ByteHigh = byte.Parse(high, NumberStyles.HexNumber);

                            //高位在前
                            result_4.Add(ByteLow);
                            result_4.Add(ByteHigh);
                        }

                        //输出ASCII码转换后的结果
                        string trim2 = Encoding.ASCII.GetString(result_4.ToArray()).Trim('\0');
                        strings = new[] { trim2 };
                        ReadDvg(strings);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error($"[{TraceContext.Name}]--执行Modbus读寄存器失败,错误:{e}");
            }
        }

        public async Task WriteReg()
        {
            NetWorkTriggerModel modelNetWorkTriggerModel = node.Model.NetWorkTriggerModel;
            NetWork netWork = GlobalManager.GetNetWork(node.Model.NetWorkTriggerModel.NetworkName);
            if (netWork == null)
            {
                Log.Error("通讯未连接,未找到netWork", $"{node.NodeName}:{node.Id}");
                return;
            }

            ModbusBase modbusBase = netWork.ModbusBase;
            var registers = new List<ushort>();
            try
            {
                Log.Info("执行10写多寄存器", $"{node.NodeName}:{node.Id}");
                switch (node.Model.NetWorkTriggerModel.Format)
                {
                    case "单寄存器(无符号)":
                        foreach (var modbusPojo in node.Model.NetWorkTriggerModel.WriteDvgList)
                            registers.Add(ushort.Parse(GetParamValue(modbusPojo)));

                        await modbusBase.WriteRegisters_10(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            registers.ToArray()
                        );
                        break;
                    case "单寄存器(有符号)":
                        foreach (var modbusPojo in node.Model.NetWorkTriggerModel.WriteDvgList)
                        {
                            registers.Add((ushort)int.Parse(GetParamValue(modbusPojo)));
                        }

                        await modbusBase.WriteRegisters_10(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            registers.ToArray()
                        );
                        break;
                    case "双寄存器;无符号;BigEndian":
                        foreach (var modbusPojo in node.Model.NetWorkTriggerModel.WriteDvgList)
                        {
                            uint i = uint.Parse(GetParamValue(modbusPojo));
                            byte[] bytes = BitConverter.GetBytes(i);

                            var newBytes = new byte[] { bytes[2], bytes[3], bytes[0], bytes[1] };
                            registers.Add(BitConverter.ToUInt16(newBytes, 0));
                            registers.Add(BitConverter.ToUInt16(newBytes, 2));
                        }

                        await modbusBase.WriteRegisters_10(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            registers.ToArray()
                        );
                        break;
                    case "双寄存器;无符号;LittleEndian":
                        foreach (var modbusPojo in node.Model.NetWorkTriggerModel.WriteDvgList)
                        {
                            uint i = uint.Parse(GetParamValue(modbusPojo));
                            byte[] bytes = BitConverter.GetBytes(i);

                            var newBytes = new byte[] { bytes[1], bytes[0], bytes[3], bytes[2] };
                            registers.Add(BitConverter.ToUInt16(newBytes, 0));
                            registers.Add(BitConverter.ToUInt16(newBytes, 2));
                        }

                        await modbusBase.WriteRegisters_10(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            registers.ToArray()
                        );
                        break;
                    case "双寄存器;无符号;BigEndianByteSwap":
                        foreach (var modbusPojo in node.Model.NetWorkTriggerModel.WriteDvgList)
                        {
                            uint i = uint.Parse(GetParamValue(modbusPojo));
                            byte[] bytes = BitConverter.GetBytes(i);

                            var newBytes = new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] };
                            registers.Add(BitConverter.ToUInt16(newBytes, 0));
                            registers.Add(BitConverter.ToUInt16(newBytes, 2));
                        }

                        await modbusBase.WriteRegisters_10(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            registers.ToArray()
                        );
                        break;
                    case "双寄存器;无符号;LittleEndianByteSwap":
                        foreach (var modbusPojo in node.Model.NetWorkTriggerModel.WriteDvgList)
                        {
                            uint i = uint.Parse(GetParamValue(modbusPojo));
                            byte[] bytes = BitConverter.GetBytes(i);

                            var newBytes = new byte[] { bytes[0], bytes[1], bytes[2], bytes[3] };
                            registers.Add(BitConverter.ToUInt16(newBytes, 0));
                            registers.Add(BitConverter.ToUInt16(newBytes, 2));
                        }

                        await modbusBase.WriteRegisters_10(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            registers.ToArray()
                        );
                        break;
                    case "双寄存器;有符号;BigEndian":
                        foreach (var modbusPojo in node.Model.NetWorkTriggerModel.WriteDvgList)
                        {
                            int i = int.Parse(GetParamValue(modbusPojo));
                            byte[] bytes = BitConverter.GetBytes(i);

                            var newBytes = new byte[] { bytes[2], bytes[3], bytes[0], bytes[1] };
                            registers.Add(BitConverter.ToUInt16(newBytes, 0));
                            registers.Add(BitConverter.ToUInt16(newBytes, 2));
                        }

                        await modbusBase.WriteRegisters_10(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            registers.ToArray()
                        );
                        break;
                    case "双寄存器;有符号;LittleEndian":
                        foreach (var modbusPojo in node.Model.NetWorkTriggerModel.WriteDvgList)
                        {
                            int i = int.Parse(GetParamValue(modbusPojo));
                            byte[] bytes = BitConverter.GetBytes(i);

                            var newBytes = new byte[] { bytes[1], bytes[0], bytes[3], bytes[2] };
                            registers.Add(BitConverter.ToUInt16(newBytes, 0));
                            registers.Add(BitConverter.ToUInt16(newBytes, 2));
                        }

                        await modbusBase.WriteRegisters_10(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            registers.ToArray()
                        );
                        break;
                    case "双寄存器;有符号;BigEndianByteSwap":
                        foreach (var modbusPojo in node.Model.NetWorkTriggerModel.WriteDvgList)
                        {
                            int i = int.Parse(GetParamValue(modbusPojo));
                            byte[] bytes = BitConverter.GetBytes(i);

                            var newBytes = new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] };
                            registers.Add(BitConverter.ToUInt16(newBytes, 0));
                            registers.Add(BitConverter.ToUInt16(newBytes, 2));
                        }

                        await modbusBase.WriteRegisters_10(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            registers.ToArray()
                        );
                        break;
                    case "双寄存器;有符号;LittleEndianByteSwap":
                        foreach (var modbusPojo in node.Model.NetWorkTriggerModel.WriteDvgList)
                        {
                            int i = int.Parse(GetParamValue(modbusPojo));
                            byte[] bytes = BitConverter.GetBytes(i);

                            var newBytes = new byte[] { bytes[0], bytes[1], bytes[2], bytes[3] };
                            registers.Add(BitConverter.ToUInt16(newBytes, 0));
                            registers.Add(BitConverter.ToUInt16(newBytes, 2));
                        }

                        await modbusBase.WriteRegisters_10(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            registers.ToArray()
                        );
                        break;
                    case "32位浮点数;BigEndian":
                        foreach (var modbusPojo in node.Model.NetWorkTriggerModel.WriteDvgList)
                        {
                            float i = float.Parse(GetParamValue(modbusPojo));
                            byte[] bytes = BitConverter.GetBytes(i);

                            var newBytes = new byte[] { bytes[2], bytes[3], bytes[0], bytes[1] };
                            registers.Add(BitConverter.ToUInt16(newBytes, 0));
                            registers.Add(BitConverter.ToUInt16(newBytes, 2));
                        }

                        await modbusBase.WriteRegisters_10(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            registers.ToArray()
                        );
                        break;
                    case "32位浮点数;LittleEndian":
                        foreach (var modbusPojo in node.Model.NetWorkTriggerModel.WriteDvgList)
                        {
                            float i = float.Parse(GetParamValue(modbusPojo));
                            byte[] bytes = BitConverter.GetBytes(i);

                            var newBytes = new byte[] { bytes[1], bytes[0], bytes[3], bytes[2] };
                            registers.Add(BitConverter.ToUInt16(newBytes, 0));
                            registers.Add(BitConverter.ToUInt16(newBytes, 2));
                        }

                        await modbusBase.WriteRegisters_10(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            registers.ToArray()
                        );
                        break;
                    case "32位浮点数;BigEndianByteSwap":
                        foreach (var modbusPojo in node.Model.NetWorkTriggerModel.WriteDvgList)
                        {
                            float i = float.Parse(GetParamValue(modbusPojo));
                            byte[] bytes = BitConverter.GetBytes(i);

                            var newBytes = new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] };
                            registers.Add(BitConverter.ToUInt16(newBytes, 0));
                            registers.Add(BitConverter.ToUInt16(newBytes, 2));
                        }

                        await modbusBase.WriteRegisters_10(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            registers.ToArray()
                        );
                        break;
                    case "32位浮点数;LittleEndianByteSwap":
                        foreach (var modbusPojo in node.Model.NetWorkTriggerModel.WriteDvgList)
                        {
                            float i = float.Parse(GetParamValue(modbusPojo));
                            byte[] bytes = BitConverter.GetBytes(i);

                            var newBytes = new byte[] { bytes[0], bytes[1], bytes[2], bytes[3] };
                            registers.Add(BitConverter.ToUInt16(newBytes, 0));
                            registers.Add(BitConverter.ToUInt16(newBytes, 2));
                        }

                        await modbusBase.WriteRegisters_10(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            registers.ToArray()
                        );
                        break;
                    case "ASCII字符串(低高位)":
                        var operationModel = node.Model.NetWorkTriggerModel.WriteDvgList[0];
                        //按低高位写入
                        List<ushort> list = new List<ushort>();

                        for (int i = 0; i < GetParamValue(operationModel).Length; i += 2)
                        {
                            char high = GetParamValue(operationModel)[i];
                            char low = (i + 1 < GetParamValue(operationModel).Length)
                                ? GetParamValue(operationModel)[i + 1]
                                : '\0'; // 补0
                            ushort packed = (ushort)((high << 8) | low);
                            list.Add(packed);
                        }

                        ushort[] result = list.ToArray();

                        await modbusBase.WriteRegisters_10(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            result);

                        break;
                    case "ASCII字符串(高低位)":
                        var operationModel2 = node.Model.NetWorkTriggerModel.WriteDvgList[0];
                        //按高低位写入
                        List<ushort> list2 = new List<ushort>();

                        for (int i = 0; i < GetParamValue(operationModel2).Length; i += 2)
                        {
                            char high = GetParamValue(operationModel2)[i];
                            char low = (i + 1 < GetParamValue(operationModel2).Length)
                                ? GetParamValue(operationModel2)[i + 1]
                                : '\0'; // 补0
                            ushort packed = (ushort)((low << 8) | high);
                            list2.Add(packed);
                        }

                        ushort[] result2 = list2.ToArray();

                        await modbusBase.WriteRegisters_10(
                            byte.Parse(node.Model.NetWorkTriggerModel.StationAddress),
                            ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress),
                            result2);
                        break;
                }


                Log.Info("执行10写多寄存器完成", $"{node.NodeName}:{node.Id}");
            }
            catch (Exception exception)
            {
                Log.Error($"执行10写多寄存器错误:{exception}", $"{node.NodeName}:{node.Id}");
            }
        }

        #region 显示读DGV

        public void ReadDvg<T>(T[] value)
        {
            var StartAddress = ushort.Parse(node.Model.NetWorkTriggerModel.StartAddress);

            ushort curAddress = StartAddress;
            int i = 0;
            foreach (OperationModel nodeOutputParam in node.OutputParams)
            {
                if (nodeOutputParam.Name.Equals(curAddress.ToString()))
                {
                    nodeOutputParam.ParamValue = value[i].ToString();
                }

                if
                    (node.Model.NetWorkTriggerModel.Format == "双寄存器;无符号;BigEndian" ||
                     node.Model.NetWorkTriggerModel.Format == "双寄存器;无符号;LittleEndian" ||
                     node.Model.NetWorkTriggerModel.Format == "双寄存器;无符号;BigEndianByteSwap" ||
                     node.Model.NetWorkTriggerModel.Format == "双寄存器;无符号;LittleEndianByteSwap" ||
                     node.Model.NetWorkTriggerModel.Format == "双寄存器;有符号;BigEndian" ||
                     node.Model.NetWorkTriggerModel.Format == "双寄存器;有符号;LittleEndian" ||
                     node.Model.NetWorkTriggerModel.Format == "双寄存器;有符号;BigEndianByteSwap" ||
                     node.Model.NetWorkTriggerModel.Format == "双寄存器;有符号;LittleEndianByteSwap" ||
                     node.Model.NetWorkTriggerModel.Format == "32位浮点数;BigEndian" ||
                     node.Model.NetWorkTriggerModel.Format == "32位浮点数;LittleEndian" ||
                     node.Model.NetWorkTriggerModel.Format == "32位浮点数;BigEndianByteSwap" ||
                     node.Model.NetWorkTriggerModel.Format == "32位浮点数;LittleEndianByteSwap"
                    )
                {
                    curAddress += 2;
                }
                else
                {
                    curAddress++;
                }

                i++;
            }
        }

        #endregion
    }
}