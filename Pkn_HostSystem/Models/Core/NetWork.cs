using KeyenceTool;
using Newtonsoft.Json;
using Pkn_HostSystem.Base;

namespace Pkn_HostSystem.Pojo.Page.HomePage;

public class NetWork
{
    /// <summary>
    /// 连接id ,用于当前连接,与ConnectPojo内容一致
    /// </summary>
    public string NetWorkId { get; set; }
    /// <summary>
    ///  存储实例化的当前Modbus通讯体
    /// </summary>
    [JsonIgnore] public ModbusBase ModbusBase { get; set; }

    /// <summary>
    /// 存储实例化的基恩士通讯体
    /// </summary>
    [JsonIgnore] public KeyenceHostLinkTool KeyenceHostLinkTool { get; set; }

    /// <summary>
    /// 存储实例化的套接字通讯体
    /// </summary>
    [JsonIgnore] public TcpTool TcpTool { get; set; }

    /// <summary>
    /// 令牌 控制当前网络任务
    /// </summary>
    [JsonIgnore] public CancellationTokenSource CancellationTokenSource { get; set; }

    /// <summary>
    /// 网络连接任务进程,启动网络,重连等
    /// </summary>
    [JsonIgnore] public Lazy<Task> Task { get; set; }

    /// <summary>
    /// 网络详细内容
    /// </summary>
    public NetworkDetailed NetworkDetailed { get; set; }
}