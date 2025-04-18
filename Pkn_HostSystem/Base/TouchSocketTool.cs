using Pkn_HostSystem.Base.Log;
using System.Net;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace Pkn_HostSystem.Base;

public class TouchSocketTool
{
    // 日志
    public LogBase<TouchSocketTool> Log = new LogBase<TouchSocketTool>();

    // 服务器
    public TcpService Server { get; private set; }

    // 客户端
    public TcpClient Client { get; private set; }

    // 服务器是否运行
    public bool IsServerRunning => Server?.ServerState == ServerState.Running;

    // 客户端连接状态
    public bool IsConnected => Client?.Online ?? false;

    public bool IsClientOpen => Client != null;

    public async Task OpenServer(int port)
    {
        Server = new TcpService();
        Server.Closing += ClosingServer;
        Server.Closed += ClosedServer;
        Server.Connecting += ConnectingServer;
        Server.Connected += ConnectedServer;

        string Host = $"tcp://0.0.0.0:{port}";
        Server.SetupAsync(new TouchSocketConfig()//载入配置
            .SetListenIPHosts(Host)
        );
        await Server.StartAsync();//启动
    }

    //有客户端正在断开连接
    public virtual Task ClosingServer(TcpSessionClient client, ClosingEventArgs e)
    {
        Log.Info($"客户端 {client.Id} 连接断开中");
        return EasyTask.CompletedTask;
    }
    //有客户端已断开
    public virtual Task ClosedServer(TcpSessionClient client, ClosingEventArgs e)
    {
        Log.Info($"客户端 {client.Id} 连接断开");
        return EasyTask.CompletedTask;
    }
    //连接对象已连接
    private Task ConnectedServer(TcpSessionClient client, ConnectedEventArgs e)
    {
        Log.Info($"客户端 {client.Id} 已连接");
        return EasyTask.CompletedTask;
    }
    //对象连接中
    public virtual Task ConnectingServer(TcpSessionClient client, ConnectingEventArgs e)
    {
        Log.Info($"客户端 {client.Id} 连接中");
        return EasyTask.CompletedTask;
    }
}