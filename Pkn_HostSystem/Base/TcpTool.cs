using Pkn_HostSystem.Base.Log;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Interop;

namespace Pkn_HostSystem.Base
{
    public class TcpTool
    {
        /// <summary>
        /// 日志
        /// </summary>
        public LogBase<TcpTool> Log = new();
        /// <summary>
        /// CTS
        /// </summary>
        private CancellationTokenSource? _cts;
        //客户端集合
        public  ConcurrentDictionary<string, TcpClient> _clients = new();
        //维护一个接受客户端消息的集合, 最多储存10条历史数据

        public ConcurrentDictionary<string, List<string>> _clientsResponse = new();

        /// <summary>
        /// 服务器
        /// </summary>
        private TcpListener? _server;
        /// <summary>
        /// 客户端
        /// </summary>
        public TcpClient? _client;
        /// <summary>
        /// 通讯流
        /// </summary>
        private NetworkStream? _clientStream;
        /// <summary>
        /// 服务器是否处于运行
        /// </summary>
        public bool IsServerRunning => _server != null;
        /// <summary>
        /// 客户端是否处于运行
        /// </summary>
        public bool IsClientConnected => _client?.Connected ?? false;

        /// <summary>
        /// 服务器 收到 客户端连接事件
        /// </summary>
        public event Action<string> OnClientConnected;
        /// <summary>
        /// 服务器 收到 客户端断开连接事件
        /// </summary>
        public event Action<string> OnClientDisconnected;
        /// <summary>
        /// 服务器 收到 客户端发送消息事件
        /// </summary>
        public event Action<string, string> OnMessageReceived;

        #region 服务器连接和断开
        public async Task<bool> StartServerAsync(int port , bool isListen)
        {
            if (_server != null) return false;

            _cts = new CancellationTokenSource();
            //任何地址都可以开启服务器
            _server = new TcpListener(IPAddress.Any, port);
            try
            {
                _server.Start();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return false;
            }
            Log.Info($"[{TraceContext.Name}]--[Server] 启动监听端口 {port}");

            //开启一个异步线程监听客户端连接
            _ = Task.Run(async () =>
            {
                //循环监听客户端
                while (!_cts!.IsCancellationRequested)
                {
                    try
                    {
                        var client = await _server.AcceptTcpClientAsync();
                        var ip = ((IPEndPoint)client.Client.RemoteEndPoint!).ToString();
                        //添加到客户端集合
                        _clients[ip] = client;
                        _clientsResponse[ip] = new List<string>(); 
                        //调用客户端已执行委托
                        OnClientConnected?.Invoke(ip);
                        Log.Info($"[{TraceContext.Name}]--[Server] 客户端连接: {ip}");
                        if (isListen)
                        {
                            Log.Info($"[{TraceContext.Name}]--[Server] 对客户端{ip} 进行消息监听");
                            _ = ServeListenReadClient(client, ip, _cts.Token);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"[{TraceContext.Name}]--[Server] Accept 失败: {ex.Message}");
                        break;
                    }
                }
            });
            return true;
        }

        public void StopServer()
        {
            _cts?.Cancel();
            //循环进行连接断开
            foreach (var kv in _clients)
            {
                kv.Value.Close();
                kv.Value.Dispose();
            }
            _clients.Clear();
            _server?.Stop();
            _server?.Dispose();
            _server = null;
            _clientsResponse = new();
            Log.Info($"[{TraceContext.Name}]--[Server] 已停止监听");
        }
        #endregion

        #region 服务器接受消息
        public async Task ServeListenReadClient(TcpClient client, string ip, CancellationToken token)
        {
            try
            {
                //创建客户端通讯流
                using var stream = client.GetStream();
                var buffer = new byte[1024];

                //循环读取客户端发送消息
                while (!token.IsCancellationRequested && client.Connected)
                {
                    //阻塞,等待读取成功
                    int count = await stream.ReadAsync(buffer, 0, buffer.Length, token);
           

                    if (count == 0) break;
                    //读取到的消息msg
                    string msg = Encoding.UTF8.GetString(buffer, 0, count);
                    //存储到集合
                    AddResponseList(ip, msg);
                    OnMessageReceived?.Invoke(ip, msg);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[{TraceContext.Name}]--[Server] 客户端 {ip} 异常: {ex.Message}");
            }
            finally
            {
                _clients.TryRemove(ip, out _);
                _clientsResponse.TryRemove(ip, out _);
                client.Close();
                OnClientDisconnected?.Invoke(ip);
                Log.Info($"[{TraceContext.Name}]--[Server] 客户端断开: {ip}");
            }
        }

        public async Task<(bool succeed , string response)> ServeReadClientOneToOne(TcpClient client, string ip, CancellationToken token, int timeout = 3000)
        {
            //创建客户端通讯流
            NetworkStream stream = null;
            try
            {
                 stream = client.GetStream();
            }
            catch (InvalidOperationException e)
            {
                Log.Error($"[{TraceContext.Name}]--[Server] 当前通讯对象[{ip}]已经断开");
                _clients.TryRemove(ip, out _);
                _clientsResponse.TryRemove(ip, out _);
                return (false, $"当前通讯对象[{ip}]已经断开");
            }
            var buffer = new byte[1024];

            Task<int> readTask = stream.ReadAsync(buffer, 0, buffer.Length, token);


            var any = await Task.WhenAny(Task.Delay(timeout, token), readTask);

            if (any== readTask)
            {
                int count = await readTask;
                if (count > 0)
                {
                    //读取到的消息msg
                    string msg = Encoding.UTF8.GetString(buffer, 0, count);
                    return (true, msg);
                }
                else
                {
                    Log.Error($"[{TraceContext.Name}]--[Server] 等待客户端消息返回空内容");
                    return (false,null);
                }
            }
            else
            {
                Log.Error($"[{TraceContext.Name}]--[Server] 等待客户端消息返回超时");
                return (false, null);
            }
        }
        #endregion

        #region 服务器发送消息

        /// <summary>
        /// 发送消息给客户端
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task ServerSendAsync(string ip, string message)
        {
            if (_clients.TryGetValue(ip, out TcpClient? client))
            {
                var data = Encoding.UTF8.GetBytes(message);
                await client.GetStream().WriteAsync(data, 0, data.Length);
            }
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task BroadcastAsync(string message)
        {
            foreach (var kv in _clients)
            {
                try
                {
                    await ServerSendAsync(kv.Key, message);
                }
                catch (Exception ex)
                {
                    Log.Error($"[Server] 广播到 {kv.Key} 失败: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 通过广播形式,不过是一对一通讯
        /// </summary>
        /// <param name="message"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<(bool succeed, string? response)> ServerSendWaitResponseOneToOne(string message,
            CancellationTokenSource cts,int timeout = 3000)
        {
            bool NoError = true;
            //等待返回消息
            string msg = null;
            if (_clients.Count == 0)
            {
                Log.Error($"[{TraceContext.Name}]--服务器没有任何连接对象");
                return (false, null);
            }
            var clinetList = _clients.ToArray();

            string ip = clinetList[0].Key;
            TcpClient client = clinetList[0].Value;
            msg = null;
            try
            {
                if (client == null || !client.Connected)
                {
                    Log.Error($"[{TraceContext.Name}]--在服务器执行发送消息时, 客户端未连接服务器");
                    NoError = false;
                }

                if (_server == null || !IsServerRunning)
                {
                    Log.Error($"[{TraceContext.Name}]--在服务器执行发送消息时, 服务器为null");
                    NoError = false;
                }
                // 发送消息
                try
                {
                    await ServerSendAsync(ip, message);
                }
                catch (InvalidOperationException ex)
                {
                    //对方掉线,移除
                    _clients.TryRemove(clinetList[0]);
                    //递归
                    (bool succeed, string? response) = await ServerSendWaitResponseOneToOne(message,cts);
                    if (succeed)
                    {
                        return (true, response);
                    }

                    return (false, null);
                }
                catch (IOException ex1)
                {
                    //对方掉线,移除
                    _clients.TryRemove(clinetList[0]);
                    (bool succeed, string? response) = await ServerSendWaitResponseOneToOne(message,cts);
                    if (succeed)
                    {
                        return (true, response);
                    }

                    return (false, null);
                }
                catch (Exception e)
                {
                    Log.Error($"[{TraceContext.Name}]--在服务器执行发送消息时,出现异常{e}");
                    NoError = false;
                }

                try
                {
                    Log.Info($"[{TraceContext.Name}]--在服务器执行发送消息后,[{ip}]等待消息返回中");
                    var buffer = new byte[1024];
                    var stream = client.GetStream();
                    var startTime = Environment.TickCount; // 记录开始时间
                    while (!cts.Token.IsCancellationRequested)
                    {
                        // 检查超时
                        int elapsed = Environment.TickCount - startTime;
                        if (elapsed >= timeout)
                        {
                            Log.Info($"[{TraceContext.Name}]--在服务器执行发送消息后,[{ip}]等待消息超时！");
                            NoError = false;
                            break;
                        }

                        // 检查是否有数据可读  stream.DataAvailable 可以读取流中是否有数据
                        if (stream.DataAvailable)
                        {
                            int count = await stream.ReadAsync(buffer, 0, buffer.Length);
                            msg = Encoding.UTF8.GetString(buffer, 0, count);
                            Log.Info($"[{TraceContext.Name}]--在服务器执行发送消息后,收到[{ip}]返回消息: {msg}");
                            break;
                        }

                        // 没数据，休息一下再看
                        await Task.Delay(100);
                    }
                }
                catch (Exception e)
                {
                    Log.Info($"[{TraceContext.Name}]--在服务器执行发送客户端[{ip}]消息后等待消息返回,出现异常{e}");
                    NoError = false;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[Server] 广播到 {ip} 失败: {ex.Message}");
                NoError = false;
            }
            if (NoError)
            {
                return (true, msg);
            }

            return (false, null);
        }


        /// <summary>
        /// 发送消息并且等待返回
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="request"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<string> SendAndWaitServerAsync(string clientId, string request, int timeout = 3000)
        {
            try
            {
                await ServerSendAsync(clientId, request);
                if (_clients.TryGetValue(clientId, out TcpClient? client))
                {
                    //创建客户端通讯流
                    using var stream = client.GetStream();
                    var buffer = new byte[1024];
                    using var cts = new CancellationTokenSource(timeout);
                    //阻塞,等待读取成功
                    int count = await stream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
                    string msg = Encoding.UTF8.GetString(buffer, 0, count);
                    return msg;
                }
            }
            catch (Exception e)
            {
                Log.Error($"服务器发送消息失败:{e.Message}");
            }

            return String.Empty;
        }

        #endregion

        #region 客户端连接和断开

        public async Task<bool> ConnectToServerAsync(string ip, int port)
        {
            if (_client != null && _client.Connected) return true;

            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(ip, port);
                _clientStream = _client.GetStream();
                // _ = Task.Run(() => ClientReceiveFromServer(_clientStream));
                Log.Info($"[{TraceContext.Name}]--[Client] 连接服务器成功: {ip}:{port}");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"[{TraceContext.Name}]--[Client] 连接失败: {ex.Message}");
                return false;
            }
        }

        public void DisconnectClient()
        {
            _clientStream?.Close();
            _client?.Close();
            _client?.Dispose();
            _client = null;
            _clientStream = null;
        }

        #endregion

        #region 客户端接受消息事件
        public async Task ClientReceiveFromServer(NetworkStream stream)
        {
            var buffer = new byte[1024];
         
                while (IsClientConnected)
                {
                    try
                    {
                        //阻塞,等待读取成功
                        int count = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (count == 0) break;

                        string msg = Encoding.UTF8.GetString(buffer, 0, count);
                        OnMessageReceived?.Invoke($"{stream}", msg);
                    }
                    catch
                    {
                        break;
                    }
                }
            Log.Info($"[Client] 与服务器断开连接");
        }
        #endregion

        #region 客户端发送消息

        public async Task<(bool succeed, string? response)> SendAndWaitClientAsync(string message,CancellationTokenSource cts, int timeout = 3000)
        {
            if (_client == null || !_client.Connected || _clientStream == null)
            {
                Log.Info($"[{TraceContext.Name}]--在客户端执行发送消息时, 客户端未连接服务器");
                return (false, null);
            }

            // 发送消息
            try
            {
                var data = Encoding.UTF8.GetBytes(message);
                await _clientStream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception e)
            {
                Log.Info($"[{TraceContext.Name}]--在客户端执行发送消息时,出现异常{e}");
                return (false, null);
            }

            //等待返回消息
            string msg = null;
            try
            {
                Log.Info($"[{TraceContext.Name}]--在客户端执行发送消息后,等待消息返回");
                var buffer = new byte[1024];
                var stream = _client.GetStream();
                var startTime = Environment.TickCount; // 记录开始时间
                while (!cts.Token.IsCancellationRequested)
                {
                    // 检查超时
                    int elapsed = Environment.TickCount - startTime;
                    if (elapsed >= timeout)
                    {
                        Log.Info($"[{TraceContext.Name}]--在客户端执行发送消息后,等待消息超时！");
                        return (false, null);
                    }

                    // 检查是否有数据可读  stream.DataAvailable 可以读取流中是否有数据
                    if (stream.DataAvailable)
                    {
                        int count = await stream.ReadAsync(buffer, 0, buffer.Length);
                        msg = Encoding.UTF8.GetString(buffer, 0, count);
                        Log.Info($"[{TraceContext.Name}]--在客户端执行发送消息后,收到服务器返回消息: {msg}");
                        break;
                    }

                    // 没数据，休息一下再看
                    await Task.Delay(100);
                }
            }
            catch (Exception e)
            {
                Log.Info($"[{TraceContext.Name}]--在客户端执行发送消息后等待消息返回,出现异常{e}");
                return (false, null);
            }

            return (true, msg);
        }

        public async Task<bool> ClientSendAsync(string message)
        {
            if (_client == null || !_client.Connected || _clientStream == null) return false;

            var data = Encoding.UTF8.GetBytes(message);
            await _clientStream.WriteAsync(data, 0, data.Length);
            return true;
        }
        #endregion

        #region 私有帮助方法
        private void AddResponseList(string ip , string message)
        {
            List<string> list = _clientsResponse[ip];
            if (list.Count >= 10)
            {
                list.RemoveAt(0);
            }
            list.Add(message);
        }

        #endregion
    }
}