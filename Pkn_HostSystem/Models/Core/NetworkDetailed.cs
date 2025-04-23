using System.IO.Ports;
using Pkn_HostSystem.Base;

namespace Pkn_HostSystem.Pojo.Page.HomePage;

/// <summary>
/// 网络的详细内容
/// </summary>
public class NetworkDetailed
{
    /// <summary>
    /// 网路的独立ID
    /// </summary>
    public string Id { get; set; } = new SnowflakeIdGenerator(1, 1).GetId().ToString();
    /// <summary>
    /// 网络名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 当前选择的通讯模式 :
    /// </summary>
    public string NetMethod { get; set; } 
    /// <summary>
    /// 是否处于运行状态
    /// </summary>
    public bool Open { get; set; }
    /// <summary>
    /// 当前Ip地址
    /// </summary>
    public string IP { get; set; }
    /// <summary>
    /// 当前端口号
    /// </summary>
    public int Port { get; set; }
    /// <summary>
    /// 当前Com口
    /// </summary>
    public string Com { get; set; }
    /// <summary>
    /// 比特率
    /// </summary>
    public string BaudRate { get; set; }
    /// <summary>
    /// 数据位
    /// </summary>
    public string DataBits { get; set; }
    /// <summary>
    /// 停止位
    /// </summary>
    public StopBits StopBits { get; set; }
    /// <summary>
    /// 校验码
    /// </summary>
    public Parity Parity { get; set; }
}