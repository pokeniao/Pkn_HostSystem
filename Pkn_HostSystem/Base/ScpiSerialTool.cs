using Pkn_HostSystem.Base.Log;
using System.IO;
using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Interop;

namespace Pkn_HostSystem.Base
{
    public class ScpiSerialTool : IDisposable
    {
        private SerialPort serialPort;
        /// <summary>
        /// 判断是否打开
        /// </summary>
        public bool IsOpen => serialPort?.IsOpen ?? false;
        

        private TaskCompletionSource<bool> _connectTcs = new TaskCompletionSource<bool>();

        private LogBase<ScpiSerialTool> Log =new LogBase<ScpiSerialTool>();


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
        public static List<string> BaudRates = ["9600", "14400", "19200"];

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


        public static Dictionary<string, string> NewLines  = new Dictionary<string, string>() { ["\\r"]="\r", ["\\r\\n"] = "\r\n", ["空"] = "" };



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
        public void Open(string portName, int baudRate = 9600, Parity parity = Parity.None,
            int dataBits = 8, StopBits stopBits = StopBits.One, int readTimeout = 1000, string newLine = "\r")
        {
            if (serialPort.IsOpen)
            {
                return;
            }
            serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
            {
                Encoding = System.Text.Encoding.ASCII,
                ReadTimeout = readTimeout,
                WriteTimeout = 500,
                NewLine = newLine  //通常以 CR（\r）结束响应
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
        }
        /// <summary>
        /// 关闭串口通讯
        /// </summary>
        public void Close()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        public async Task SendCommand(string command)
        {
            await EnsureConnected();
            serialPort.WriteLine(command);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="delayMs"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async  Task<(bool ,string)> SendAndWaitResponse(string command, int timeout = 3000)
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
                try
                {
                    string readLine = serialPort.ReadLine();
                    if (!string.IsNullOrEmpty(readLine))
                    {
                        Log.Info($"[{TraceContext.Name}]--串口通讯,收到消息: {readLine}");
                        return (true, readLine);
                    }
                }
                catch (TimeoutException)
                {
                    // 串口设置的 ReadTimeout 到时间，但数据还没来，继续等待,  ReadTimeout 在serialPort.ReadLine(); 没有读取到东西,过一段时间就会超时,所以需要跳过这个
                }
                // 没数据，休息一下再看
                await Task.Delay(100);
            }
        }


        public async Task<string> ReadResponse()
        {
            await EnsureConnected();
            return serialPort.ReadLine();
        }



        public void Dispose()
        {
            try
            {
                Close();
                serialPort?.Dispose();
            }
            catch
            {
            
            }
        }
        private async Task EnsureConnected()
        {
            var timeoutTask = Task.Delay(3000);
            var completedTask = await Task.WhenAny(_connectTcs.Task, timeoutTask);
            if (completedTask != _connectTcs.Task || !IsOpen)
            {
                throw new TimeoutException("串口未打开。");
            }
        }

    }


    
}