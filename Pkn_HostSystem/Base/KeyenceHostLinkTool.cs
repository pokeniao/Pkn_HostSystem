using Pkn_HostSystem.Base.Log;
using System.Net.Sockets;
using System.Text;

namespace Pkn_HostSystem.Base
{
    public class KeyenceHostLinkTool
    {
        private TcpClient client;
        private NetworkStream stream;
        private LogControl<KeyenceHostLinkTool> Log = new LogControl<KeyenceHostLinkTool>();

        public bool IsConnected => client?.Connected ?? false;

        #region 连接与断开

        public async Task<bool> Connect(string ip, int port = 8501)
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync(ip, port);
                stream = client.GetStream();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Disconnect()
        {
            stream?.Close();
            client?.Close();
            stream = null;
            client = null;
        }

        #endregion

        #region 读取R线圈和写R线圈

        public async Task<(bool succeed, string? response)> ReadR(string address, CancellationTokenSource cts)
        {
            Log.Info($"[{TraceContext.Name}]--读取R线圈,地址:{address}");
            return await SendCommand($"RD R{address}", cts);
        }

        public async Task<bool> WriteR(string address, bool value, CancellationTokenSource cts)
        {
            Log.Info($"[{TraceContext.Name}]--写R线圈,地址:{address},写入内容:{value}");
            string cmd = value ? $"ST R{address}" : $"RS R{address}";

            (bool succeed, string? response) = await SendCommand(cmd, cts);

            return response.Trim().Equals("OK", StringComparison.OrdinalIgnoreCase);
        }

        #endregion


        #region 读ZF写ZF

        public async Task<(bool succeed, string? response)> ReadZF(string address, CancellationTokenSource cts)
        {
            Log.Info($"[{TraceContext.Name}]--读ZF,地址:{address}");
            return await SendCommand($"RD ZF{address}", cts);
        }

        public async Task<bool> WriteZF(string address, bool value, CancellationTokenSource cts)
        {
            Log.Info($"[{TraceContext.Name}]--写ZF,地址:{address},值{value}");
            string cmd = value ? $"ST ZF{address}" : $"RS ZF{address}";

            (bool succeed, string? response) = await SendCommand(cmd, cts);
            return response.Trim().Equals("OK", StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region 读取写入DM
        /// <summary>
        /// 读单寄存器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address"></param>
        /// <param name="cts"></param>
        /// <param name="noLog"></param>
        /// <returns></returns>
        public async Task<(bool, T)> ReadDM<T>(int address, CancellationTokenSource cts, bool noLog = false)
            where T : struct
        {
            if (!noLog)
            {
                Log.Info($"[{TraceContext.Name}]--读DM执行,地址:{address}");
            }

            //判断是否是32位,来决定是否+L
            bool is32Bit = Is32BitType<T>();
            string suffix = is32Bit ? ".L" : "";
            //拼接报文
            string command = $"RD DM{address}{suffix}";
            //发送报文
            (bool succeed, string response) = await SendCommand(command, cts, noLog);
            try
            {
                if (succeed)
                {
                    T convertFromResponse = KeyenceMcDataConverter.ConvertFromResponse<T>(response);
                    return (true, convertFromResponse);
                }
                else
                {
                    return (false, default);
                }
            }
            catch (Exception e)
            {
                Log.Error($"[{TraceContext.Name}]--执行基恩士类型转换时候发送异常:{e}");
                return (false, default);
            }
        }
        /// <summary>
        /// 写单寄存器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <param name="cts"></param>
        /// <returns></returns>
        public async Task<bool> WriteDM<T>(int address, T value, CancellationTokenSource cts) where T : struct
        {
            Log.Info($"WriteDM执行,address:{address} ,value: {value}");
            string String = KeyenceMcDataConverter.ConvertToWriteData(value);
            //判断是否是32位
            bool is32Bit = Is32BitType<T>();
            string suffix = is32Bit ? ".L" : "";
            string command = $"WR DM{address}{suffix} {String}";
            //发送数据
            (bool succeed, string? response) = await SendCommand(command, cts);

            Log.Info($"[{TraceContext.Name}]--基恩士写入后收到返回内容:{response}");

            bool equals = response.Trim().Equals("OK", StringComparison.OrdinalIgnoreCase);

            //判断是否接受成功
            return equals;
        }

        /// <summary>
        /// 读取多个寄存器
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<(bool succeed, ushort[] response)> ReadDMWords(int startAddress, int count,
            CancellationTokenSource cts)
        {
            (bool succeed, string response) = await SendCommand($"RD DM{startAddress} {count}", cts);

            if (!succeed)
            {
                return (false, null);
            }


            if (response.StartsWith("+"))
                response = response.Substring(1);

            // 统一去掉空格、换行
            response = response.Replace(" ", "").Trim();

            ushort[] values = new ushort[count];

            //转成16进制存储
            for (int i = 0; i < count; i++)
            {
                string hex = response.Substring(i * 4, 4);
                values[i] = Convert.ToUInt16(hex, 16);
            }

            return (true, values);
        }
        /// <summary>
        /// 写字符串
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="text"></param>
        /// <param name="cts"></param>
        /// <returns></returns>
        public async Task<bool> WriteDMString(int startAddress, string text, CancellationTokenSource cts)
        {
            // 转成 ASCII 字节
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            // 如果不是偶数个字节，补 0（或者补空格也行）
            if (bytes.Length % 2 != 0)
            {
                Array.Resize(ref bytes, bytes.Length + 1);
                bytes[bytes.Length - 1] = 00; // 0x20 是空格
            }

            int wordCount = bytes.Length / 2;
            ushort[] words = new ushort[wordCount];
            for (int i = 0; i < wordCount; i++)
            {
                byte low = bytes[i * 2];       // 低位
                byte high = bytes[i * 2 + 1];  // 高位
                words[i] = (ushort)((low << 8) | high); // 和你读时的反向逻辑一致
            }
            // 拼接 WR 命令
            StringBuilder cmdBuilder = new StringBuilder();
            cmdBuilder.Append($"WR DM{startAddress} {wordCount}");
            foreach (ushort w in words)
            {
                cmdBuilder.Append(" ");
                cmdBuilder.Append(w.ToString("X4"));
            }

            string command = cmdBuilder.ToString();
            // 发送
            (bool succeed, string response) = await SendCommand(command, cts);

            return succeed;
        }

        #endregion

        #region 内部方法

        /// <summary>
        /// 用于判断是否是32位数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private bool Is32BitType<T>() where T : struct
        {
            Type type = typeof(T);
            return type == typeof(int) || type == typeof(uint) || type == typeof(float);
        }

        #endregion

        private static readonly SemaphoreSlim _commLock = new(1, 1);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private async Task<(bool succeed, string response)> SendCommand(string command, CancellationTokenSource cts,
            bool noLog = false)
        {
            if (!IsConnected) return (false, "未连接");

            await _commLock.WaitAsync(cts.Token); // 添加通信串行化


            try
            {
                try
                {
                    byte[] sendData = Encoding.ASCII.GetBytes(command + "\r");
                    if (stream == null)
                    {
                        return (false, null);
                    }
                    await stream.WriteAsync(sendData, 0, sendData.Length);

                    if (!noLog)
                    {
                        Log.Info($"[{TraceContext.Name}]--基恩士上位链路协议发送");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"[{TraceContext.Name}]--在基恩士上链路通讯TCP客户端执行发送消息时,出现异常{ex.Message}");
                    return (false, $"[{TraceContext.Name}]--在基恩士上链路通讯TCP客户端执行发送消息时,出现异常{ex.Message}");
                }

                if (!noLog)
                    Log.Info($"[{TraceContext.Name}]--基恩士上位链路协议发送后,等待消息返回");
                byte[] buffer = new byte[256];

                var startTime = Environment.TickCount; // 获取当前时间戳
                while (!cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        //检查超时
                        int elapsed = Environment.TickCount - startTime;
                        if (elapsed > 3000) // 超时3秒
                        {
                            Log.Error($"[{TraceContext.Name}]--基恩士上位链路协议发送后,等待消息返回超时");
                            return (false, "基恩士超时");
                        }

                        //检查是否有数据可读

                        // 累积数据直到检测到 "\r\n"
                        var sb = new StringBuilder();
                        if (stream.DataAvailable)
                        {
                            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length,cts.Token);

                            sb.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));

                            //  一次读完一帧，并自动丢弃多余粘包内容。
                            if (sb.ToString().Contains("\r\n"))
                            {
                                var raw = sb.ToString();
                                var cleanLines = raw.Split(["\r\n"], StringSplitOptions.RemoveEmptyEntries);
                                string response = cleanLines.FirstOrDefault() ?? "";
                                return (true, response);
                            }
                        }
                        // 没数据，休息一下再看
                        await Task.Delay(100, cts.Token);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"[{TraceContext.Name}]--在基恩士上链路通讯TCP客户端执行读取消息时,出现异常{e.Message}");
                        return (false, $"[{TraceContext.Name}]--在基恩士上链路通讯TCP客户端执行读取消息时,出现异常{e.Message}");
                    }
       
                }
            }
            finally
            {
                _commLock.Release();
            }
         
            return (false, "未收到数据");
        }
    }

    /// <summary>
    /// 静态类,用于格式转换
    /// </summary>
    public static class KeyenceMcDataConverter
    {
        /// <summary>
        /// 返回消息格式转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public static T ConvertFromResponse<T>(string response) where T : struct
        {
            //前面是+号的情况 ,TrimStart去除前面的+号
            response = response.Trim().TrimStart('+');

            try
            {
                if (typeof(T) == typeof(ushort))
                    //
                    return (T)(object)ushort.Parse(response);

                if (typeof(T) == typeof(short))
                    return (T)(object)(short)ushort.Parse(response);

                if (typeof(T) == typeof(uint))
                    return (T)(object)uint.Parse(response);

                if (typeof(T) == typeof(int))
                    return (T)(object)int.Parse(response);

                if (typeof(T) == typeof(float))
                {
                    uint raw = uint.Parse(response);
                    return (T)(object)BitConverter.ToSingle(BitConverter.GetBytes(raw), 0);
                }
            }
            catch (Exception ex)
            {
                throw new FormatException($"无法解析响应 '{response}' 为类型 {typeof(T)}，错误: {ex.Message}");
            }

            throw new NotSupportedException($"不支持的数据类型: {typeof(T)}");
        }

        public static string ConvertToWriteData<T>(T value) where T : struct
        {
            if (value is ushort)
                return Convert.ToUInt16(value).ToString();

            if (value is short sval)
            {
                ushort uval = (ushort)sval;
                return uval.ToString();
            }

            if (value is uint)
                return Convert.ToUInt32(value).ToString();

            if (value is int)
                return Convert.ToInt32(value).ToString();
            if (value is float fval)
            {
                var bytes = BitConverter.GetBytes(fval);
                uint raw = BitConverter.ToUInt32(bytes, 0);
                return raw.ToString();
            }

            throw new NotSupportedException($"不支持的数据类型: {typeof(T)}");
        }
    }
}