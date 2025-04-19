using System;
using System.Globalization;
using System.Net.Sockets;
using System.Text;

namespace KeyenceTool
{
    public class KeyenceHostLinkTool : IDisposable
    {
        private TcpClient client;
        private NetworkStream stream;

        public bool IsConnected => client?.Connected ?? false;

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

        public string ReadR(string address) => SendCommand($"RD R{address}");

        public bool WriteR(string address, bool value)
        {
            string cmd = value ? $"ST R{address}" : $"RS R{address}";
            return SendCommand(cmd).Equals("OK", StringComparison.OrdinalIgnoreCase);
        }

        public string ReadZF(string address) => SendCommand($"RD ZF{address}");

        public bool WriteZF(string address, bool value)
        {
            string cmd = value ? $"ST ZF{address}" : $"RS ZF{address}";
            return SendCommand(cmd).Equals("OK", StringComparison.OrdinalIgnoreCase);
        }

        public T ReadDM<T>(int address) where T : struct
        {
            bool is32Bit = Is32BitType<T>();
            string suffix = is32Bit ? ".L" : "";
            string command = $"RD DM{address}{suffix}";
            string response = SendCommand(command);

            return KeyenceMcDataConverter.ConvertFromResponse<T>(response);
        }

        public bool WriteDM<T>(int address, T value) where T : struct
        {
            string hexString = KeyenceMcDataConverter.ConvertToWriteData(value);
            bool is32Bit = Is32BitType<T>();
            string suffix = is32Bit ? ".L" : "";
            string command = $"WR DM{address}{suffix} {hexString}";
            string response = SendCommand(command);
            return IsSuccessResponse(response);
        }

        private bool Is32BitType<T>() where T : struct
        {
            Type type = typeof(T);
            return type == typeof(int) || type == typeof(uint) || type == typeof(float);
        }

        private bool IsSuccessResponse(string response)
        {
            return response?.Trim().Equals("OK", StringComparison.OrdinalIgnoreCase) == true;
        }

        private string SendCommand(string command)
        {
            if (!IsConnected) return "未连接";

            try
            {
                byte[] sendData = Encoding.ASCII.GetBytes(command + "\r");
                stream.Write(sendData, 0, sendData.Length);

                byte[] buffer = new byte[256];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                return Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim('\0', '\r', '\n');
            }
            catch (Exception ex)
            {
                return $"错误: {ex.Message}";
            }
        }

        // 读取 DM 寄存器的值（32位普通版本）
        public string ReadDMCommon(string address)
        {
            return SendCommand2("RD DM" + address + ".L");
        }

        // 写 DM 寄存器 ()
        public bool WriteDMCommon(string address, string value)
        {
            return SendCommand2("WR DM" + address + ".L " + value) == "OK";
        }

        /// <summary>
        /// 发送指令并接收响应
        /// </summary>
        private string SendCommand2(string command)
        {
            if (!IsConnected) return "未连接";

            try
            {
                byte[] sendData = Encoding.ASCII.GetBytes(command + "\r");
                stream.Write(sendData, 0, sendData.Length);

                byte[] buffer = new byte[256];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                return response.Trim('\0', '\r', '\n');
            }
            catch (Exception ex)
            {
                return "错误：" + ex.Message;
            }
        }


        public void Dispose()
        {
            Disconnect();
        }
    }

    public static class KeyenceMcDataConverter
    {
        public static T ConvertFromResponse<T>(string response) where T : struct
        {
            response = response.Trim().TrimStart('+');

            try
            {
                if (typeof(T) == typeof(ushort))
                    return (T)(object)ushort.Parse(response, NumberStyles.HexNumber);

                if (typeof(T) == typeof(short))
                    return (T)(object)(short)ushort.Parse(response, NumberStyles.HexNumber);

                if (typeof(T) == typeof(uint))
                    return (T)(object)uint.Parse(response, NumberStyles.HexNumber);

                if (typeof(T) == typeof(int))
                    return (T)(object)(int)uint.Parse(response, NumberStyles.HexNumber);

                if (typeof(T) == typeof(float))
                {
                    uint raw = uint.Parse(response, NumberStyles.HexNumber);
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
            if (value is ushort || value is short)
                return Convert.ToUInt16(value).ToString("X4");

            if (value is uint || value is int)
                return Convert.ToUInt32(value).ToString("X8");

            if (value is float fval)
            {
                var bytes = BitConverter.GetBytes(fval);
                uint raw = BitConverter.ToUInt32(bytes, 0);
                return raw.ToString("X8");
            }

            throw new NotSupportedException($"不支持的数据类型: {typeof(T)}");
        }
    }
}