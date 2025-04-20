using Pkn_HostSystem.Base.Log;
using System.Net.Sockets;
using System.Text;

namespace KeyenceTool
{
    public class KeyenceHostLinkTool
    {
        private TcpClient client;
        private NetworkStream stream;
        private LogBase<KeyenceHostLinkTool> Log = new LogBase<KeyenceHostLinkTool>();

        public bool IsConnected => client?.Connected ?? false;

        #region 连接与断开

        public bool Connect(string ip, int port = 8501)
        {
            try
            {
                client = new TcpClient();
                client.Connect(ip, port);
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

        public string ReadR(string address)
        {
            Log.Info($"ReadR执行,address:{address}");
            return SendCommand($"RD R{address}");
        }

        public bool WriteR(string address, bool value)
        {
            Log.Info($"WriteR执行,address:{address},value:{value}");
            string cmd = value ? $"ST R{address}" : $"RS R{address}";
            return SendCommand(cmd).Equals("OK", StringComparison.OrdinalIgnoreCase);
        }

        #endregion


        #region 读ZF写ZF

        public string ReadZF(string address)
        {
            Log.Info($"ReadZF执行,address:{address}");
            return SendCommand($"RD ZF{address}");
        }

        public bool WriteZF(string address, bool value)
        {
            Log.Info($"WriteZF执行,address:{address},value{value}");
            string cmd = value ? $"ST ZF{address}" : $"RS ZF{address}";
            return SendCommand(cmd).Equals("OK", StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region 读取写入DM

        public T ReadDM<T>(int address) where T : struct
        {
            Log.Info($"ReadDM执行,address:{address}");
            //判断是否是32位,来决定是否+L
            bool is32Bit = Is32BitType<T>();
            string suffix = is32Bit ? ".L" : "";
            //拼接报文
            string command = $"RD DM{address}{suffix}";
            //发送报文
            string response = SendCommand(command);
            return KeyenceMcDataConverter.ConvertFromResponse<T>(response);
        }

        public bool WriteDM<T>(int address, T value) where T : struct
        {
            Log.Info($"WriteDM执行,address:{address} ,value: {value}");
            string String = KeyenceMcDataConverter.ConvertToWriteData(value);
            //判断是否是32位
            bool is32Bit = Is32BitType<T>();
            string suffix = is32Bit ? ".L" : "";
            string command = $"WR DM{address}{suffix} {String}";
            //发送数据
            string response = SendCommand(command);
            //判断是否接受成功
            return IsSuccessResponse(response);
        }

        /// <summary>
        /// 读取多个
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ushort[] ReadDMWords(int startAddress, int count)
        {
            string response = SendCommand($"RD DM{startAddress} {count}");

            if (response.StartsWith("+"))
                response = response.Substring(1);

            // 统一去掉空格、换行
            response = response.Replace(" ", "").Trim();

            ushort[] values = new ushort[count];

            ///转成16进制存储
            for (int i = 0; i < count; i++)
            {
                string hex = response.Substring(i * 4, 4);
                values[i] = Convert.ToUInt16(hex, 16);
            }

            return values;
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

        /// <summary>
        /// 判断是否接受成功
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private bool IsSuccessResponse(string response)
        {
            bool b = response?.Trim().Equals("OK", StringComparison.OrdinalIgnoreCase) == true;
            if (b)
            {
                Log.Info("发送成功,返回消息提示成功");
            }
            else
            {
                Log.Info("发送失败,返回消息提示失败");
            }
            return b;
        }

        #endregion


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private string SendCommand(string command)
        {
            if (!IsConnected) return "未连接";

            try
            {
                byte[] sendData = Encoding.ASCII.GetBytes(command + "\r");
                stream.Write(sendData, 0, sendData.Length);

                byte[] buffer = new byte[256];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                Log.Info("SendCommand:基恩士上位链路协议发送");
                return Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim('\0', '\r', '\n');
            }
            catch (Exception ex)
            {
                Log.Error($"错误: {ex.Message}");
                return $"错误: {ex.Message}";
            }
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