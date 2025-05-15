using NModbus;
using NModbus.Serial;
using System.IO.Ports;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Pkn_HostSystem.Base
{
    public class ModbusBase
    {
        //串口
        private SerialPort serialPort { get; set; }

        //网口
        private TcpClient tcpClient { get; set; }

        //超时时间
        public int ReadTimeout { get; set; } = 300;

        //等待时间
        public int Retries { get; set; } = 3;

        //RTU主站实例化,TCP主站(客户端)实例化
        public IModbusMaster modbusMaster { get; set; }

        //RTU从站实例化,用于监听主站请求
        public IModbusSlaveNetwork rtuSlaveNetwork { get; set; }

        //TCP从站监听
        public bool isListener { get; set; }

        private readonly object _lock = new object();

        private TaskCompletionSource<bool> _connectTcs = new TaskCompletionSource<bool>();


        #region 判断是否连接上

        public bool IsTCPConnect()
        {
            if (tcpClient == null || tcpClient.Client == null) return false;

            // 1. 检查是否仍然连接
            if (!tcpClient.Connected) return false;

            // 2. 使用 Poll 检查是否可读（如果对方关闭，读取状态将会变更）
            if (tcpClient.Client.Poll(0, SelectMode.SelectRead))
            {
                byte[] buffer = new byte[1];
                try
                {
                    if (tcpClient.Client.Receive(buffer, SocketFlags.Peek) == 0)
                    {
                        // 连接已关闭
                        return false;
                    }
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsRTUConnect()
        {
            //判断串口
            if (serialPort != null)
                if (serialPort.IsOpen)
                    return true;

            return false;
        }

        #endregion

        #region 打开和关闭TCP

        public async Task<bool> OpenTcpMaster(string ip, int port)
        {
            if (IsTCPConnect() || IsRTUConnect()) return false; //串口或网口其中有一个被占用

            try
            {
                //要在 lock 中使用 await，否则可能造成死锁
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(ip, port);
                lock (_lock)
                {
                    var factory = new ModbusFactory();
                    modbusMaster = factory.CreateMaster(tcpClient);
                    modbusMaster.Transport.ReadTimeout = ReadTimeout;
                    modbusMaster.Transport.Retries = Retries;
                }

                // 连接成功，通知所有等待者
                _connectTcs.TrySetResult(true);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取IP
        /// </summary>
        /// <returns></returns>
        public string[] getIpAddress()
        {
            var ips = new List<string>();
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                if (ni.OperationalStatus == OperationalStatus.Up)
                    //&& ni.NetworkInterfaceType != NetworkInterfaceType.Loopback 可以排除127.0.0.1
                    foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork) // 只获取 IPv4
                            ips.Add(ip.Address.ToString());

            return ips.ToArray();
        }

        // public async Task OpenTcpSlave(int port)
        // {
        //     //1. 创建监听
        //     TcpListener listener = new TcpListener(IPAddress.Any, 502);
        //     listener.Start();
        //     isListener=true;
        //     while (isListener)
        //     {
        //         try
        //         {
        //             //2. 执行监听异步
        //             TcpClient client = await listener.AcceptTcpClientAsync();
        //             //3. 客户端循环处理
        //             var factory = new ModbusFactory();
        //         }
        //         catch (Exception e)
        //         {
        //             Console.WriteLine(e);
        //             throw;
        //         }
        //     }
        // }

        #endregion

        #region 打开串口和关闭串口

        /// <summary>
        /// 创建RTU主站
        /// </summary>
        /// <param name="port">COM口</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="dataBits">数据位</param>
        /// <param name="stopBits">停止位</param>
        /// <param name="parity">奇偶校验</param>
        /// <returns></returns>
        public async Task<bool> OpenRTUMaster(string port, int baudRate, int dataBits, StopBits stopBits, Parity parity)
        {
            if (IsTCPConnect() || IsRTUConnect()) return false; //串口或网口其中有一个被占用
            //第一步初始化串口操作对象
            try
            {
                serialPort = new SerialPort()
                {
                    PortName = port, //端口号
                    BaudRate = baudRate, //波特率
                    DataBits = dataBits, //数据位
                    StopBits = stopBits, //停止位
                    Parity = parity, //奇偶校验
                    ReadTimeout = ReadTimeout, //设置超时时间 ,必须要有,不然连接会一直等待
                    WriteTimeout = ReadTimeout
                };
                //第二步: 打开串口
                bool b = await Task.Run(() =>
                {
                    try
                    {
                        serialPort.Open();
                        return true;
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                });
                if (!b)
                {
                    return false;
                }

                //创建Modbus主站
                var factory = new ModbusFactory();
                modbusMaster = factory.CreateRtuMaster(serialPort);
                modbusMaster.Transport.ReadTimeout = ReadTimeout;
                modbusMaster.Transport.Retries = Retries;
                // 连接成功，通知所有等待者
                _connectTcs.TrySetResult(true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool OpenRTUSlave(string port, int baudRate, int dataBits, StopBits stopBits, Parity parity,
            byte slaveAddress)
        {
            if (serialPort != null) return false;
            //第一步初始化串口操作对象
            serialPort = new SerialPort()
            {
                PortName = port, //端口号
                BaudRate = baudRate, //波特率
                DataBits = dataBits, //数据位
                StopBits = stopBits, //停止位
                Parity = parity, //奇偶校验
                ReadTimeout = ReadTimeout, //设置超时时间 ,必须要有,不然连接会一直等待
                WriteTimeout = ReadTimeout
            };
            //第二步: 打开串口
            try
            {
                serialPort.Open();
                //创建ModbusRTU从站
                var factory = new ModbusFactory();
                rtuSlaveNetwork = factory.CreateRtuSlaveNetwork(serialPort);
                var modbusSlave = factory.CreateSlave(slaveAddress);
                rtuSlaveNetwork.AddSlave(modbusSlave); //添加从站
                rtuSlaveNetwork.ListenAsync(); // 启动从站监听

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        public void CloseRTU()
        {
            //第一步:判断是否打开串口
            if (serialPort == null) return;
            //第二步:判断是否打开串口
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                serialPort.Dispose();
            }
        }

        /// <summary>
        /// 关闭Tcp客户端
        /// </summary>
        public void CloseTCP()
        {
            if (tcpClient != null)
                if (tcpClient.Connected)
                {
                    tcpClient.Close();
                    tcpClient.Dispose();
                    tcpClient = null;
                }
        }

        /// <summary>
        /// 获取端口
        /// </summary>
        /// <returns></returns>
        public string[] getCOM()
        {
            return SerialPort.GetPortNames();
        }

        #endregion

        #region 01读线圈
        /// <summary>
        /// 01读线圈
        /// </summary>
        /// <param name="slaveAddress">站地址</param>
        /// <param name="startAddress">起始地址</param>
        /// <param name="ReadCount">数量</param>
        /// <returns></returns>
        public async Task<bool[]> ReadCoils_01(byte slaveAddress, ushort startAddress, ushort ReadCount)
        {
            // 等待连接真正建立好，最多等3秒
            var timeoutTask = Task.Delay(3000);
            // 等待连接真正建立好_connectTcs.Task和timeoutTask ,WhenAny只要满足一个就放行
            var completedTask = await Task.WhenAny(_connectTcs.Task, timeoutTask);
            if (completedTask != _connectTcs.Task)
            {
                throw new TimeoutException("ReadCoils_01 连接建立超时");
            }

            try
            {
                return await modbusMaster.ReadCoilsAsync(slaveAddress, startAddress, ReadCount);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 02读输入状态

        /// <summary>
        /// 02读输入状态
        /// </summary>
        /// <param name="slaveAddress">站地址</param>
        /// <param name="startAddress">起始地址</param>
        /// <param name="ReadCount">数量</param>
        /// <returns></returns>
        public async Task<bool[]> ReadInputs_02(byte slaveAddress, ushort startAddress, ushort ReadCount)
        {
            // 等待连接真正建立好，最多等3秒
            var timeoutTask = Task.Delay(3000);
            // 等待连接真正建立好_connectTcs.Task和timeoutTask ,WhenAny只要满足一个就放行
            var completedTask = await Task.WhenAny(_connectTcs.Task, timeoutTask);
            if (completedTask != _connectTcs.Task)
            {
                throw new TimeoutException("ReadInputs_02 连接建立超时");
            }

            try
            {
                return await modbusMaster.ReadInputsAsync(slaveAddress, startAddress, ReadCount);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region 03读保存寄存器

        /// <summary>
        /// 03读保存寄存器
        /// </summary>
        /// <param name="slaveAddress">站地址</param>
        /// <param name="startAddress">起始地址</param>
        /// <param name="ReadCount">数量</param>
        /// <returns></returns>
        public async Task<ushort[]> ReadHoldingRegisters_03(byte slaveAddress, ushort startAddress, ushort ReadCount)
        {
            // 等待连接真正建立好，最多等3秒
            var timeoutTask = Task.Delay(3000);
            // 等待连接真正建立好_connectTcs.Task和timeoutTask ,WhenAny只要满足一个就放行
            var completedTask = await Task.WhenAny(_connectTcs.Task, timeoutTask);
            if (completedTask != _connectTcs.Task)
            {
                throw new TimeoutException("ReadHoldingRegisters_03 连接建立超时");
            }
            try
            {
                return await modbusMaster.ReadHoldingRegistersAsync(slaveAddress, startAddress, ReadCount);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region 04读取输入寄存器
        /// <summary>
        /// 04读取输入寄存器
        /// </summary>
        /// <param name="slaveAddress">站地址</param>
        /// <param name="startAddress">起始地址</param>
        /// <param name="ReadCount">数量</param>
        /// <returns></returns>
        public async Task<ushort[]> ReadInputRegisters_04(byte slaveAddress, ushort startAddress, ushort ReadCount)
        {
            // 等待连接真正建立好，最多等3秒
            var timeoutTask = Task.Delay(3000);
            // 等待连接真正建立好_connectTcs.Task和timeoutTask ,WhenAny只要满足一个就放行
            var completedTask = await Task.WhenAny(_connectTcs.Task, timeoutTask);
            if (completedTask != _connectTcs.Task)
            {
                throw new TimeoutException("ReadInputRegisters_04 连接建立超时");
            }
            try
            {
                return await modbusMaster.ReadInputRegistersAsync(slaveAddress, startAddress, ReadCount);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 05写单线圈
        /// <summary>
        /// 05写单线圈
        /// </summary>
        /// <param name="slaveAddress">站地址</param>
        /// <param name="coilAddress">线圈位置</param>
        /// <param name="value">bool值</param>
        /// <returns></returns>
        public async Task WriteCoil_05(byte slaveAddress, ushort coilAddress, bool value)
        {
            // 等待连接真正建立好，最多等3秒
            var timeoutTask = Task.Delay(3000);
            // 等待连接真正建立好_connectTcs.Task和timeoutTask ,WhenAny只要满足一个就放行
            var completedTask = await Task.WhenAny(_connectTcs.Task, timeoutTask);
            if (completedTask != _connectTcs.Task)
            {
                throw new TimeoutException("WriteCoil_05 连接建立超时");
            }

            try
            {
                await modbusMaster.WriteSingleCoilAsync(slaveAddress, coilAddress, value);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region 06写单寄存器

        /// <summary>
        /// 06写单寄存器
        /// </summary>
        /// <param name="slaveAddress">站地址</param>
        /// <param name="registerAddress">寄存器地址</param>
        /// <param name="value">修改值</param>
        /// <returns></returns>
        public async Task WriteRegister_06(byte slaveAddress, ushort registerAddress, ushort value)
        {
            // 等待连接真正建立好，最多等3秒
            var timeoutTask = Task.Delay(3000);
            // 等待连接真正建立好_connectTcs.Task和timeoutTask ,WhenAny只要满足一个就放行
            var completedTask = await Task.WhenAny(_connectTcs.Task, timeoutTask);
            if (completedTask != _connectTcs.Task)
            {
                throw new TimeoutException("WriteRegister_06 连接建立超时");
            }

            try
            {
                await modbusMaster.WriteSingleRegisterAsync(slaveAddress, registerAddress, value);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region 0F写多线圈

        /// <summary>
        /// 0F写多线圈
        /// </summary>
        /// <param name="slaveAddress">站地址</param>
        /// <param name="startAddress">起始地址</param>
        /// <param name="values">bool数组</param>
        /// <returns></returns>
        public async Task WriteCoils_0F(byte slaveAddress, ushort startAddress, bool[] values)
        {
            // 等待连接真正建立好，最多等3秒
            var timeoutTask = Task.Delay(3000);
            // 等待连接真正建立好_connectTcs.Task和timeoutTask ,WhenAny只要满足一个就放行
            var completedTask = await Task.WhenAny(_connectTcs.Task, timeoutTask);
            if (completedTask != _connectTcs.Task)
            {
                throw new TimeoutException("WriteCoils_0F 连接建立超时");
            }
            try
            {
                await modbusMaster.WriteMultipleCoilsAsync(slaveAddress, startAddress, values);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region 10写多寄存器
        /// <summary>
        /// 10写多寄存器
        /// </summary>
        /// <param name="slaveAddress">站地址</param>
        /// <param name="startAddress">起始地址</param>
        /// <param name="values">修改的值数组</param>
        /// <returns></returns>
        public async Task WriteRegisters_10(byte slaveAddress, ushort startAddress, ushort[] values)
        {
            // 等待连接真正建立好，最多等3秒
            var timeoutTask = Task.Delay(3000);
            // 等待连接真正建立好_connectTcs.Task和timeoutTask ,WhenAny只要满足一个就放行
            var completedTask = await Task.WhenAny(_connectTcs.Task, timeoutTask);
            if (completedTask != _connectTcs.Task)
            {
                throw new TimeoutException("WriteRegisters_10 连接建立超时");
            }

            try
            {
                await modbusMaster.WriteMultipleRegistersAsync(slaveAddress, startAddress, values);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion
    }
}