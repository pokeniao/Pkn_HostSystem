using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base.Log;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace Pkn_HostSystem.Base
{
    public class TcpTool
    {
        public event Action<string> OnClientConnected;
        public event Action<string> OnClientDisconnected;
        public event Action<string, string> OnMessageReceived;


        public LogBase<TcpTool> Log = new LogBase<TcpTool>();

        private TcpListener? _server;

        //客户端集合
        private readonly ConcurrentDictionary<string, TcpClient> _clients = new();
        private CancellationTokenSource? _cts;

        private TcpClient? _client;
        private NetworkStream? _clientStream;

        public bool IsServerRunning => _server != null;
        public bool IsClientConnected => _client?.Connected ?? false;


        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _responseTasks = new();


        #region 服务器连接和断开

        public async Task<bool> StartServerAsync(int port)
        {
            if (_server != null) return false;

            _cts = new CancellationTokenSource();
            _server = new TcpListener(IPAddress.Any, port);
            _server.Start();
            Log.Info($"[Server] 启动监听端口 {port}");

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
                        //调用客户端已执行委托
                        OnClientConnected?.Invoke(ip);
                        Log.Info($"[Server] 客户端连接: {ip}");
                        //执行与客户端通讯
                        _ = HandleClientAsync(client, ip, _cts.Token);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"[Server] Accept 失败: {ex.Message}");
                        break;
                    }
                }
            });
            return true;
        }

        public void StopServer()
        {
            _cts?.Cancel();

            foreach (var kv in _clients)
            {
                kv.Value.Close();
            }

            _clients.Clear();
            _server?.Stop();
            _server = null;
            Log.Info($"[Server] 已停止监听");
        }

        #endregion

        #region Server OnMessageReceived事件

        /// <summary>
        /// 服务器循环读取客户端消息OnMessageReceived
        /// </summary>
        /// <param name="client"></param>
        /// <param name="ip"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task HandleClientAsync(TcpClient client, string ip, CancellationToken token)
        {
            try
            {
                //创建客户端通讯流
                using var stream = client.GetStream();
                var buffer = new byte[1024];

                //循环读取客户端发送消息
                while (!token.IsCancellationRequested && client.Connected)
                {
                    await _readServerLock.WaitAsync(); //等待权限
                    //阻塞,等待读取成功
                    int count = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                    _readServerLock.Release(); //释放权限

                    if (count == 0) break;
                    //有直接字节输出到 
                    string msg = Encoding.UTF8.GetString(buffer, 0, count);
                    OnMessageReceived?.Invoke(ip, msg);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[Server] 客户端 {ip} 异常: {ex.Message}");
            }
            finally
            {
                _clients.TryRemove(ip, out _);
                client.Close();
                OnClientDisconnected?.Invoke(ip);
                Log.Info($"[Server] 客户端断开: {ip}");
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
        public async Task SendServerAsync(string ip, string message)
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
                    await SendServerAsync(kv.Key, message);
                }
                catch (Exception ex)
                {
                    Log.Error($"[Server] 广播到 {kv.Key} 失败: {ex.Message}");
                }
            }
        }

        private readonly SemaphoreSlim _readServerLock = new SemaphoreSlim(1, 1);

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
                await _readServerLock.WaitAsync(); //等待权限
                await SendServerAsync(clientId, request);
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
            finally
            {
                _readServerLock.Release(); //释放权限
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
                ClientResponceCTS = new CancellationTokenSource();
        //        _ = Task.Run(() => ReceiveFromServer(_clientStream));
                Log.Info($"[Client] 连接服务器成功: {ip}:{port}");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"[Client] 连接失败: {ex.Message}");
                return false;
            }
        }

        public void DisconnectClient()
        {
            _clientStream?.Close();
            _client?.Close();
            _client = null;
            _clientStream = null;
        }

        #endregion

        #region 客户端接受消息事件

        private CancellationTokenSource ClientResponceCTS;
        private async Task ReceiveFromServer(NetworkStream stream)
        {
            var buffer = new byte[1024];
            while (!ClientResponceCTS.Token.IsCancellationRequested)
            {
                if (_client?.Connected ?? false)
                {
                    try
                    {
                        int count = await stream.ReadAsync(buffer, 0, buffer.Length);

                        if (count == 0) break;

                        string msg = Encoding.UTF8.GetString(buffer, 0, count);
                        OnMessageReceived?.Invoke("Server", msg);
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            Log.Info($"[Client] 与服务器断开连接");
        }

        #endregion

        #region 客户端发送消息


        public async Task<string> SendAndWaitClientAsync(string message, int timeout = 3000)
        {
            if (_client == null || !_client.Connected || _clientStream == null)
                throw new InvalidOperationException("客户端未连接服务器");
            ClientResponceCTS.Cancel();
            // 发送消息
            var data = Encoding.UTF8.GetBytes(message);
            await _clientStream.WriteAsync(data, 0, data.Length);
            //等待返回消息
            var buffer = new byte[1024];
            var stream = _client.GetStream();
            int count = await stream.ReadAsync(buffer, 0, buffer.Length);

            string msg = Encoding.UTF8.GetString(buffer, 0, count);
            ClientResponceCTS = new CancellationTokenSource();
         //   _ = Task.Run(() => ReceiveFromServer(_clientStream));
            return msg;
        }


        public async Task<bool> SendClientAsync(string message)
        {
            if (_client == null || !_client.Connected || _clientStream == null) return false;

            var data = Encoding.UTF8.GetBytes(message);
            await _clientStream.WriteAsync(data, 0, data.Length);
            return true;
        }

        #endregion
    }
}