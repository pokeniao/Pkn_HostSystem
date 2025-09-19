using Pkn_HostSystem.Base.Log;
using System.IO;
using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Interop;
using Azure;
using System.Reflection.Metadata;

namespace Pkn_HostSystem.Base
{
    public class ScpiSerialTool : IDisposable
    {
        public SerialPort serialPort;

        /// <summary>
        /// 判断是否打开
        /// </summary>
        public bool IsOpen => serialPort?.IsOpen ?? false;


        private TaskCompletionSource<bool> _connectTcs = new TaskCompletionSource<bool>();

        private LogControl<ScpiSerialTool> Log = new LogControl<ScpiSerialTool>();

        /// <summary>
        /// 接收消息事件
        /// </summary>
        public event Action<string> OnMessageReceived;

        /// <summary>
        /// 获取COM端口
        /// </summary>
        public static List<string> GetCOM()
        {
            return SerialPort.GetPortNames().ToList();
        }

        /// <summary>
        /// 获取波特率
        /// </summary>
        public static List<string> BaudRates = ["4800","9600", "14400", "19200", "38400","56000","576000","115200","128000"];

        /// <summary>
        /// 获取数据位
        /// </summary>
        public static List<string> DataBits = ["8", "7"];

        /// <summary>
        /// 获取停止位
        /// </summary>
        public static List<StopBits> StopBitsList = System.Enum.GetValues(typeof(StopBits)).Cast<StopBits>().ToList();

        /// <summary>
        /// 获取校验位
        /// </summary>
        public static List<Parity> Parities = System.Enum.GetValues(typeof(Parity)).Cast<Parity>().ToList();


        public static Dictionary<string, string> NewLines =
            new Dictionary<string, string>() { ["\\n"] = "\n", ["\\r"] = "\r", ["\\r\\n"] = "\r\n" };

        /// <summary>
        /// 设置的字符
        /// </summary>
        private string setNewLine;

        /// <summary>
        /// 行数据缓存
        /// </summary>
        private StringBuilder lineBuffer = new();


        #region 打开和关闭串口

        /// <summary>
        /// 开启串口通讯
        /// </summary>
        /// <param name="portName">COM端口</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="parity">校验位</param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        /// <param name="readTimeout"></param>
        /// <param name="newLine"> 定义结束换行字符, ReadLine() 和 WriteLine() 时非常重要</param>
        public bool Open(string portName, int baudRate = 9600, Parity parity = Parity.None,
            int dataBits = 8, StopBits stopBits = StopBits.One, int readTimeout = 1000, string newLine = "\r")
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                return false;
            }

            try
            {
                setNewLine = newLine;
                serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
                {
                    Encoding = Encoding.ASCII,
                    ReadTimeout = readTimeout,
                    WriteTimeout = 500,
                    NewLine = newLine //通常以 CR（\r）结束响应
                };

                if (serialPort != null && !serialPort.IsOpen)
                {
                    serialPort.Open();
                    // 清空可能残留的数据
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                }

                // 连接成功，通知所有等待者
                _connectTcs.TrySetResult(true);
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"[{TraceContext.Name}]--{e}");
            }
            return false;
        }

        /// <summary>
        /// 关闭串口通讯
        /// </summary>
        public void Close()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
                serialPort?.Dispose();
            }
        }

        public void Dispose()
        {
            serialPort.Close();
            serialPort.Dispose();
        }

        #endregion

        #region WriteLine

        /// <summary>
        /// 写入串口WriteLine
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task WriteLine(string command)
        {
            await EnsureConnected();
            serialPort.WriteLine(command);
        }

        public async Task Write(string command)
        {
            await EnsureConnected();

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(command);

            serialPort.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 写入串口,并等待消息返回
        /// </summary>
        /// <param name="command"></param>
        /// <param name="delayMs"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<(bool succeed, string response)> WriteLineAndWaitResponse(string command, int timeout = 3000)
        {
            await EnsureConnected();

            //写入内容
            serialPort.WriteLine(command);
            serialPort.DiscardInBuffer(); // 清空旧数据
            var startTime = Environment.TickCount; // 记录开始时间
            while (true)
            {
                // 检查超时
                int elapsed = Environment.TickCount - startTime;
                if (elapsed >= timeout)
                {
                    Log.Info($"[{TraceContext.Name}]--串口通讯,等待消息超时！");
                    return (false, null);
                }

                // 检查是否有数据可读

                string? readLine = await Task.Run(() =>
                {
                    try
                    {
                        return serialPort.ReadLine();
                    }
                    catch (TimeoutException)
                    {
                        // 串口设置的 ReadTimeout 到时间，但数据还没来，继续等待,  ReadTimeout 在serialPort.ReadLine(); 没有读取到东西,过一段时间就会超时,所以需要跳过这个
                        return null;
                    }
                    catch (Exception e)
                    {
                        Log.Error($"[{TraceContext.Name}]--{e}");
                        return null;
                    }
                });
                if (!string.IsNullOrEmpty(readLine))
                {
                    Log.Info($"[{TraceContext.Name}]--串口通讯,收到消息: {readLine}");
                    return (true, readLine);
                }

                // 没数据，休息一下再看
                await Task.Delay(100);
            }
        }

        #endregion

        #region ReadLine

        /// <summary>
        /// 读取串口消息 ,serialPort.ReadLine() 是一个阻塞调用，它会等到接收到 NewLine 才返回，否则会等待到超时。
        /// </summary>
        /// <returns></returns>
        public async Task<(bool succeed, string response)> ReadLine()
        {
            try
            {
                await EnsureConnected();
                return (true, serialPort.ReadLine());
            }
            catch (TimeoutException e)
            {
                return (false, "串口通讯超时");
            }
            catch (Exception e)
            {
                return (false, $"{e}");
            }
        }

        /// <summary>
        /// 循环读取通讯触发事件
        /// </summary>
        /// <param name="cts"></param>
        /// <returns></returns>
        public async Task ReadLineEvenTask(CancellationTokenSource cts)
        {
            if (serialPort == null)
            {
                return;
            }

            byte[] buffer = new byte[1024];
            try
            {
                while (!cts.Token.IsCancellationRequested && serialPort.IsOpen)
                {
                    // string response = await ReadLine();
                    if (serialPort.BytesToRead > 0)
                    {
                        int bytesRead = serialPort.Read(buffer, 0, buffer.Length);
                        //读取到所有数据
                        string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                        //更具结束符进行换行
                        lineBuffer.Append(data);
                        string content = lineBuffer.ToString();
                        int idx;

                        // 多行处理
                        while ((idx = content.IndexOf($"{setNewLine}")) >= 0)
                        {
                            string line = content.Substring(0, idx);
                            OnMessageReceived?.Invoke(line);
                            content = content.Substring(idx + 1); // 去掉 \r
                        }

                        lineBuffer.Clear();
                        //防止越存越长,爆内存
                        if (content.Length > 32768)
                        {
                            Log.Error($"[{TraceContext.Name}] -- 缓冲数据超过 32768 字符，可能存在异常数据或缺少换行符，已清除");
                            content = "";
                        }

                        lineBuffer.Append(content); // 保留未完成的部分
                    }
                    else
                    {
                        await Task.Delay(10); // 避免 CPU 占用过高
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"[{TraceContext.Name}] -- 串口通讯, 循环读取事件出现异常: {e}");
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 检验连接是否已经处于打开状态
        /// </summary>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        private async Task EnsureConnected()
        {
            var timeoutTask = Task.Delay(3000);
            var completedTask = await Task.WhenAny(_connectTcs.Task, timeoutTask);
            if (completedTask != _connectTcs.Task || !IsOpen)
            {
                throw new TimeoutException("串口未打开。");
            }
        }

        #endregion
    }
}