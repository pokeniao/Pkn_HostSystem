using Pkn_HostSystem.Base.Log;
using System.Collections.Concurrent;
using System.Net;
using System.Text;
using WatsonTcp;

namespace Pkn_HostSystem.Base;

public class WatsonTcpTool
{
    //日志
    public LogBase<WatsonTcpTool> Log = new LogBase<WatsonTcpTool>();

    //服务器
    public WatsonTcpServer Server { get; private set; }

    //客户端

    public WatsonTcpClient Client { get; private set; }

    //服务器是否运行
    public bool IsServerRunning => Server?.IsListening ?? false;

    //客户端连接状态,客户端是否连接服务器
    public bool IsConnected => Client?.Connected ?? false;

    public bool IsClientOpen => Client == null ? false : true;

    // 线程安全的映射：ipPort <-> Guid   , Guid代表客户端的唯一标识符
    private readonly ConcurrentDictionary<string, Guid> _clientMap = new();

    /// <summary>
    /// 获得所有以连接客户端
    /// </summary>
    /// <returns></returns>
    public List<string> GetConnectedClients() => _clientMap.Keys.ToList();

    #region 服务器的打开和停止

    public bool OpenServer(int port)
    {
        if (Server != null && Server.IsListening)
        {
            Log.Info("服务器已经存在,无需创建");
            return false;
        }

        //"0.0.0.0"  == IPAddress.Any.ToString()  含义:代表监听所有本地 IPv4 地址 ,如127.0.0.1 网卡, 无线网卡等
        Server = new WatsonTcpServer(IPAddress.Any.ToString(), port);
        RegisterEventsServer();
        try
        {
            Server.Start();
        }
        catch (Exception e)
        {
            Log.Error($"错误:{e}");
            return false;
        }

        Log.Info("服务器创建成功");

        return true;
    }

    public bool StopServer()
    {
        if (Server != null && Server.IsListening)
        {
            Server.Stop();
            Log.Info("服务器停止成功");
        }

        Server = null;
        return true;
    }

    #endregion

    #region 服务器发送消息

    /// <summary>
    /// 广播消息
    /// </summary>
    public void BroadcastSend(string message, int timeoutMs = 1000)
    {
        foreach (var client in Server.ListClients())
        {
            Server.SendAsync(client.Guid, message);
        }
    }


    /// <summary>
    /// 发送消息不等待
    /// </summary>
    /// <param name="ipPort"></param>
    /// <param name="message"></param>
    /// <param name="timeoutMs"></param>
    /// <returns></returns>
    public async Task<bool?> SendNoWaitServer(string ipPort, string message, int timeoutMs = 1000)
    {
        if (!_clientMap.TryGetValue(ipPort, out Guid guid))
        {
            Log.Info($"[SendNoWait] 客户端 {ipPort}找不到");
            return false;
        }

        return await Server.SendAsync(guid, message);
    }

    /// <summary>
    /// 发送消息等待返回结果
    /// </summary>
    /// <param name="ipPort"></param>
    /// <param name="message"></param>
    /// <param name="timeoutMs"></param>
    /// <returns></returns>
    public async Task<string?> SendWaitServer(string ipPort, string message, int timeoutMs = 1000)
    {
        if (!_clientMap.TryGetValue(ipPort, out Guid guid))
        {
            Log.Info($"[SendWait] 客户端 {ipPort}找不到");
            return null;
        }

        var response = await Server.SendAndWaitAsync(timeoutMs, guid, message);
        //返回消息内容
        return response?.Data != null ? Encoding.UTF8.GetString(response.Data) : null;
    }

    #endregion

    #region 服务器事件

    private void RegisterEventsServer()
    {
        Server.Events.ClientConnected += Events_ClientConnected;
        Server.Events.ClientDisconnected += Events_ClientDisconnected;
        Server.Events.MessageReceived += Events_MessageReceived;
    }

    /// <summary>
    /// 消息接收时发生
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    public virtual void Events_MessageReceived(object? sender, MessageReceivedEventArgs e)
    {
        string ipPort = e.Client.IpPort;
        string msg = Encoding.UTF8.GetString(e.Data);
        Log.Info($"[接受消息] From {ipPort}: {msg}");
    }

    /// <summary>
    /// 客户端断开时 发生事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void Events_ClientDisconnected(object? sender, DisconnectionEventArgs e)
    {
        //字典中移除
        _clientMap.TryRemove(e.Client.IpPort, out _);
        Log.Info($"[客户端] {e.Client.IpPort} disconnected.");
    }

    /// <summary>
    /// 客户端连接时 发生事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void Events_ClientConnected(object? sender, ConnectionEventArgs e)
    {
        //字典中添加
        _clientMap[e.Client.IpPort] = e.Client.Guid;
        Log.Info($"[客户端] {e.Client.IpPort} connected.");
    }

    #endregion

    #region 客户端打开和关闭

    /// <summary>
    /// 连接客户端
    /// </summary>
    /// <param name="serverIp"></param>
    /// <param name="serverPort"></param>
    /// <returns></returns>
    public bool OpenClint(string serverIp, int serverPort)
    {
        if (Client != null && Client.Connected)
            return false;

        try
        {
            Client = new WatsonTcpClient(serverIp, serverPort);
            RegisterEventsClient();
            Client.Connect();
        }
        catch (Exception e)
        {
            Log.Error("客户端创建");
        }

        return true;
    }

    /// <summary>
    /// 断开客户端连接
    /// </summary>
    /// <returns></returns>
    public bool CloseClint()
    {
        Client?.Dispose();
        Client = null;
        return true;
    }

    #endregion

    #region 客户端事件

    private void RegisterEventsClient()
    {
        Client.Events.ServerConnected += Events_ServerConnected;

        Client.Events.ServerDisconnected += Events_ServerDisconnected;

        Client.Events.MessageReceived += Events_MessageReceivedClient;
    }

    /// <summary>
    /// 接收到服务器的消息时事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void Events_MessageReceivedClient(object? sender, MessageReceivedEventArgs e)
    {
    }

    /// <summary>
    /// 客户端断开时,事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void Events_ServerDisconnected(object? sender, DisconnectionEventArgs e)
    {
    }

    /// <summary>
    /// 客户端连接时,事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void Events_ServerConnected(object? sender, ConnectionEventArgs e)
    {
    }

    #endregion

    #region 客户端发送

    public async Task<bool> SendNoWaitClint(string msg)
    {
        if (!IsConnected) return false;

        byte[] data = Encoding.UTF8.GetBytes(msg);
        return await Client.SendAsync(data);
    }

    public async Task<string> SendWaitClint(string msg, int timeoutMs = 3000)
    {
        if (!IsConnected) return null;

        byte[] data = Encoding.UTF8.GetBytes(msg);
        var response = await Client.SendAndWaitAsync(timeoutMs, data);
        return Encoding.UTF8.GetString(response.Data);
    }

    #endregion
}