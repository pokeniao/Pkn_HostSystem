using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO.Ports;

namespace WPF_NET.Pojo;

/// <summary>
/// 连接网络的POJO
/// </summary>
public class ConnectPojo
{
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