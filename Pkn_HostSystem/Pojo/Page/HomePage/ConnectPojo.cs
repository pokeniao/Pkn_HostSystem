using System.IO.Ports;
using Pkn_HostSystem.Base;

namespace Pkn_HostSystem.Pojo.Page.HomePage;

/// <summary>
/// 连接网络的POJO
/// </summary>
public class ConnectPojo
{

    public string Id { get; set; } = new SnowflakeIdGenerator(1, 1).GetId().ToString();
    public string Name { get; set; }

    public bool NoSet { get; set; } = true;

    public bool Open { get; set; }

    public string IP { get; set; }
    public int Port { get; set; }

    public string Com { get; set; }

    public string BaudRate { get; set; }
    public string DataBits { get; set; }

    public StopBits StopBits { get; set; }
    public Parity Parity { get; set; }
}