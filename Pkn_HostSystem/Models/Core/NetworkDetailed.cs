using Pkn_HostSystem.Base;
using Pkn_HostSystem.Static;
using System.IO.Ports;

namespace Pkn_HostSystem.Models.Core;

/// <summary>
/// 网络的详细内容
/// </summary>
public class NetworkDetailed
{
    /// <summary>
    /// 网路的独立ID
    /// </summary>
    public string Id { get; set; } = GlobalManager.SnowflakeId.GetId().ToString();
    /// <summary>
    /// 网络名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 当前选择的通讯模式:
    /// </summary>
    public string NetMethod { get; set; } = "ModbusTcp";
    /// <summary>
    /// 是否处于运行状态
    /// </summary>
    public bool Open { get; set; }

    /// <summary>
    /// 当前Ip地址
    /// </summary>
    public string IP { get; set; } = "127.0.0.1";

    /// <summary>
    /// 当前端口号
    /// </summary>
    public int Port { get; set; } = 502;
    /// <summary>
    /// 当前Com口
    /// </summary>
    public string Com { get; set; }

    /// <summary>
    /// 比特率
    /// </summary>
    public string BaudRate { get; set; } = "9600";

    /// <summary>
    /// 数据位
    /// </summary>
    public string DataBits { get; set; } = "8";
    /// <summary>
    /// 停止位
    /// </summary>
    public StopBits StopBits { get; set; }
    /// <summary>
    /// 校验码
    /// </summary>
    public Parity Parity { get; set; }

    /// <summary>
    /// 串口超时时间
    /// </summary>
    public int TimeOut { get; set; } = 3000;

    /// <summary>
    /// 串口换行符
    /// </summary>
    public string NewLine { get; set; } = "\n";

    /// <summary>
    /// 服务器是否是监听模式
    /// </summary>
    public bool IsServerListen { get; set; }
}