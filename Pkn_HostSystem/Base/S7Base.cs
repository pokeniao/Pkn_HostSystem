using OpenTK.Audio.OpenAL;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Base.Log;
using RestSharp;
using S7.Net;
using System;
using System.IO.Ports;
using Wpf.Ui;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace Pkn_HostSystem.Base
{
    public class S7Base
    {
        /// <summary>
        /// 获取所有CPU类型
        /// </summary>
        public static List<CpuType> CpuTypes { get; set; } =
            System.Enum.GetValues(typeof(CpuType)).Cast<CpuType>().ToList();



        public LogControl<S7Base> Log = new LogControl<S7Base>();
        /// <summary>
        /// PLC的实例化对象
        /// </summary>
        public Plc SiemPlc { get; set; }

        // 同等于 SiemPlc != null && SiemPlc.IsConnected 
        public bool IsConnected => SiemPlc is { IsConnected: true } ? true : false;

        #region 连接与断开

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="cpuType"></param>
        /// <param name="ip"></param>
        /// <param name="rack"></param>
        /// <param name="slot"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public async Task<(bool succeed, string message)> Connect(CpuType cpuType, string ip, int rack, int slot,
            int port = 102)
        {
            if (SiemPlc != null)
            {
                if (SiemPlc.IsConnected)
                {
                    return (true, null);
                }
            }

            try
            {
                SiemPlc = new Plc(cpuType, ip, port, (short)rack, (short)slot);
                SiemPlc.OpenAsync();
            }
            catch (Exception e)
            {
                return (false, e.ToString());
            }

            if (SiemPlc.IsConnected)
            {
                return (true, null);
            }
            else
            {
                return (false, "连接失败");
            }
        }

        public bool Close()
        {
            if (SiemPlc == null)
            {
                return true;
            }

            SiemPlc.Close();
            return true;
        }

        #endregion


        public async Task<(bool succeed, object? message, T response, T[] responseBytes)> Read<T>(S7MethodEnum s7Method,
            string dataArea, int numberData,
            string offset, int num)
        {
            if (!IsConnected)
            {
                return (false, "未连接", default(T), null);
            }
            //组合字符串
            string value = null;
            //偏移量转换
            double.TryParse(offset, out double d);

            switch (s7Method)
            {
                case S7MethodEnum.位:
                    if (num == 1)
                    {
                        if (dataArea == "DB")
                        {
                            offset = d.ToString("0.0");
                            value = dataArea + numberData + ".DBX" + offset;
                        }
                        else if (dataArea == "M" || dataArea == "I" || dataArea == "Q")
                        {
                            //舍去小数位
                            int result = (int)d;
                            value = dataArea + numberData + "." + result;
                        }
                    }
                    else
                    {
                        return (false, "位暂时不支持批量写入", default(T), null);
                    }

                    //发送
                    try
                    {
                        object? readAsync = await SiemPlc.ReadAsync(value);
                        return (true, null, (T)readAsync, null);
                    }
                    catch (Exception e)
                    {
                        return (false, e.ToString(), default(T), null);
                    }

                    break;
                case S7MethodEnum.Byte8位:
                    if (num == 1)
                    {
                        if (dataArea == "DB")
                        {
                            //舍去小数位
                            int result = (int)d;
                            value = dataArea + numberData + ".DBB" + result;
                        }
                        else
                        {
                            value = dataArea + numberData;
                        }

                        //发送
                        try
                        {
                            object? readAsync = await SiemPlc.ReadAsync(value);
                            return (true, null, (T)readAsync, null);
                        }
                        catch (Exception e)
                        {
                            return (false, e.ToString(), default(T), null);
                        }
                    }
                    else
                    {
                        if (dataArea == "DB")
                        {
                            //舍去小数位
                            int result = (int)d;
                            try
                            {
                                byte[] readBytesAsync =
                                    await SiemPlc.ReadBytesAsync(DataType.DataBlock, numberData, result, num);
                                return (true, null, default(T), readBytesAsync as T[]);
                            }
                            catch (Exception e)
                            {
                                return (false, e.ToString(), default(T), null);
                            }
                        }
                        else if (dataArea == "MB")
                        {
                            try
                            {
                                byte[] readBytesAsync =
                                    await SiemPlc.ReadBytesAsync(DataType.Memory, 0, numberData, num);
                                return (true, null, default(T), readBytesAsync as T[]);
                            }
                            catch (Exception e)
                            {
                                return (false, e.ToString(), default(T), null);
                            }
                        }
                        else if (dataArea == "IB")
                        {
                            try
                            {
                                byte[] readBytesAsync =
                                    await SiemPlc.ReadBytesAsync(DataType.Input, 0, numberData, num);
                                return (true, null, default(T), readBytesAsync as T[]);
                            }
                            catch (Exception e)
                            {
                                return (false, e.ToString(), default(T), null);
                            }
                        }
                        else if (dataArea == "QB")
                        {
                            try
                            {
                                byte[] readBytesAsync =
                                    await SiemPlc.ReadBytesAsync(DataType.Output, 0, numberData, num);
                                return (true, null, default(T), readBytesAsync as T[]);
                            }
                            catch (Exception e)
                            {
                                return (false, e.ToString(), default(T), null);
                            }
                        }
                    }

                    break;

                case S7MethodEnum.无符号16位 or S7MethodEnum.有符号16位:

                    if (num == 1)
                    {
                        if (dataArea == "DB")
                        {
                            //舍去小数位
                            int result = (int)d;
                            //检测必须位2的倍数
                            if (result % 2 != 0)
                            {
                                result = result - (result % 2);
                            }

                            value = dataArea + numberData + ".DBW" + result;
                        }
                        else
                        {
                            value = dataArea + numberData;
                        }

                        //发送
                        try
                        {
                            object? readAsync = await SiemPlc?.ReadAsync(value);
                            return (true, null, (T)readAsync, null);
                        }
                        catch (Exception e)
                        {
                            return (false, e.ToString(), default(T), null);
                        }
                    }
                    else
                    {
                        if (dataArea == "DB")
                        {
                            //舍去小数位
                            int result = (int)d;
                            byte[] readBytesAsync;
                            try
                            {
                                readBytesAsync =
                                    await SiemPlc.ReadBytesAsync(DataType.DataBlock, numberData, result, num * 2);
                            }
                            catch (Exception e)
                            {
                                return (false, e.ToString(), default(T), null);
                            }


                            if (s7Method == S7MethodEnum.无符号16位)
                            {
                                ushort[] shorts = new ushort[num];
                                for (int i = 0; i < shorts.Length; i++)
                                {
                                    shorts[i] = BitConverter.ToUInt16(readBytesAsync, i * 2);
                                }

                                return (true, null, default(T), shorts as T[]);
                            }
                            else
                            {
                                short[] shorts = new short[num];
                                for (int i = 0; i < shorts.Length; i++)
                                {
                                    shorts[i] = BitConverter.ToInt16(readBytesAsync, i * 2);
                                }

                                return (true, null, default(T), shorts as T[]);
                            }
                        }
                        else if (dataArea == "MW")
                        {
                            try
                            {
                                byte[] readBytesAsync =
                                    await SiemPlc.ReadBytesAsync(DataType.Memory, 0, numberData, num * 2);

                                if (s7Method == S7MethodEnum.无符号16位)
                                {
                                    ushort[] shorts = new ushort[num];
                                    for (int i = 0; i < shorts.Length; i++)
                                    {
                                        shorts[i] = BitConverter.ToUInt16(readBytesAsync, i * 2);
                                    }

                                    return (true, null, default(T), shorts as T[]);
                                }
                                else
                                {
                                    short[] shorts = new short[num];
                                    for (int i = 0; i < shorts.Length; i++)
                                    {
                                        shorts[i] = BitConverter.ToInt16(readBytesAsync, i * 2);
                                    }

                                    return (true, null, default(T), shorts as T[]);
                                }
                            }
                            catch (Exception e)
                            {
                                return (false, e.ToString(), default(T), null);
                            }
                        }
                        else if (dataArea == "IW")
                        {
                            try
                            {
                                byte[] readBytesAsync =
                                    await SiemPlc.ReadBytesAsync(DataType.Input, 0, numberData, num * 2);

                                if (s7Method == S7MethodEnum.无符号16位)
                                {
                                    ushort[] shorts = new ushort[num];
                                    for (int i = 0; i < shorts.Length; i++)
                                    {
                                        shorts[i] = BitConverter.ToUInt16(readBytesAsync, i * 2);
                                    }

                                    return (true, null, default(T), shorts as T[]);
                                }
                                else
                                {
                                    short[] shorts = new short[num];
                                    for (int i = 0; i < shorts.Length; i++)
                                    {
                                        shorts[i] = BitConverter.ToInt16(readBytesAsync, i * 2);
                                    }

                                    return (true, null, default(T), shorts as T[]);
                                }
                            }
                            catch (Exception e)
                            {
                                return (false, e.ToString(), default(T), null);
                            }
                        }
                        else if (dataArea == "QW")
                        {
                            try
                            {
                                byte[] readBytesAsync =
                                    await SiemPlc.ReadBytesAsync(DataType.Output, 0, numberData, num * 2);
                                if (s7Method == S7MethodEnum.无符号16位)
                                {
                                    ushort[] shorts = new ushort[num];
                                    for (int i = 0; i < shorts.Length; i++)
                                    {
                                        shorts[i] = BitConverter.ToUInt16(readBytesAsync, i * 2);
                                    }

                                    return (true, null, default(T), shorts as T[]);
                                }
                                else
                                {
                                    short[] shorts = new short[num];
                                    for (int i = 0; i < shorts.Length; i++)
                                    {
                                        shorts[i] = BitConverter.ToInt16(readBytesAsync, i * 2);
                                    }

                                    return (true, null, default(T), shorts as T[]);
                                }
                            }
                            catch (Exception e)
                            {
                                return (false, e.ToString(), default(T), null);
                            }
                        }
                    }

                    break;

                case S7MethodEnum.无符号32位 or S7MethodEnum.有符号32位 or S7MethodEnum.浮点数:
                    if (num == 1)
                    {
                        if (dataArea == "DB")
                        {
                            //舍去小数位
                            int result = (int)d;
                            //检测必须位4的倍数
                            if (result % 4 != 0)
                            {
                                result = result - (result % 4);
                            }

                            value = dataArea + numberData + ".DBD" + result;
                        }
                        else
                        {
                            value = dataArea + numberData;
                        }

                        //发送
                        try
                        {
                            object? readAsync = await SiemPlc.ReadAsync(value);
                            return (true, null, (T)readAsync, null);
                        }
                        catch (Exception e)
                        {
                            return (false, e.ToString(), default(T), null);
                        }
                    }
                    else
                    {
                        if (dataArea == "DB")
                        {
                            //舍去小数位
                            int result = (int)d;
                            byte[] readBytesAsync;
                            try
                            {
                                readBytesAsync =
                                    await SiemPlc.ReadBytesAsync(DataType.DataBlock, numberData, result, num * 4);
                            }
                            catch (Exception e)
                            {
                                return (false, e.ToString(), default(T), null);
                            }


                            if (s7Method == S7MethodEnum.无符号32位)
                            {
                                uint[] uints = new uint[num];
                                for (int i = 0; i < uints.Length; i++)
                                {
                                    uints[i] = BitConverter.ToUInt32(readBytesAsync, i * 4);
                                }

                                return (true, null, default(T), uints as T[]);
                            }
                            else if  (s7Method == S7MethodEnum.有符号32位)
                            {
                                int[] ints = new int[num];
                                for (int i = 0; i < ints.Length; i++)
                                {
                                    ints[i] = BitConverter.ToInt32(readBytesAsync, i * 4);
                                }

                                return (true, null, default(T), ints as T[]);
                            }
                            else if (s7Method == S7MethodEnum.浮点数)
                            {
                                double[] doubles = new double[num];
                                for (int i = 0; i < doubles.Length; i++)
                                {
                                    doubles[i] = BitConverter.ToDouble(readBytesAsync, i * 4);
                                }
                                return (true, null, default(T), doubles as T[]);
                            }
                        }
                        else if (dataArea == "MD")
                        {
                            try
                            {
                                byte[] readBytesAsync =
                                    await SiemPlc.ReadBytesAsync(DataType.Memory, 0, numberData, num * 4);

                                if (s7Method == S7MethodEnum.无符号32位)
                                {
                                    uint[] uints = new uint[num];
                                    for (int i = 0; i < uints.Length; i++)
                                    {
                                        uints[i] = BitConverter.ToUInt32(readBytesAsync, i * 4);
                                    }

                                    return (true, null, default(T), uints as T[]);
                                }
                                else if (s7Method == S7MethodEnum.有符号32位)
                                {
                                    int[] ints = new int[num];
                                    for (int i = 0; i < ints.Length; i++)
                                    {
                                        ints[i] = BitConverter.ToInt32(readBytesAsync, i * 4);
                                    }

                                    return (true, null, default(T), ints as T[]);
                                }
                                else if (s7Method == S7MethodEnum.浮点数)
                                {
                                    double[] doubles = new double[num];
                                    for (int i = 0; i < doubles.Length; i++)
                                    {
                                        doubles[i] = BitConverter.ToDouble(readBytesAsync, i * 4);
                                    }
                                    return (true, null, default(T), doubles as T[]);
                                }
                            }
                            catch (Exception e)
                            {
                                return (false, e.ToString(), default(T), null);
                            }
                        }
                    }
                    break;

                case S7MethodEnum.字符串:
                    break;
            }


            return (false, null, default(T), null);
        }


        public static List<string> GetDataArea(S7MethodEnum method)
        {
            List<string> list = new List<string>();
            switch (method)
            {
                case S7MethodEnum.位:
                    list = ["DB", "M", "I", "Q"];
                    break;
                case S7MethodEnum.Byte8位:
                    list = ["DB", "MB", "IB", "QB"];
                    break;
                case S7MethodEnum.无符号16位:
                    list = ["DB", "MW", "IW", "QW"];
                    break;
                case S7MethodEnum.有符号16位:
                    list = ["DB", "MW", "IW", "QW"];
                    break;
                case S7MethodEnum.无符号32位:
                    list = ["DB", "MD"];
                    break;
                case S7MethodEnum.有符号32位:
                    list = ["DB", "MD"];
                    break;
                case S7MethodEnum.浮点数:
                    list = ["DB", "MD"];
                    break;
            }

            return list;
        }
    }
}