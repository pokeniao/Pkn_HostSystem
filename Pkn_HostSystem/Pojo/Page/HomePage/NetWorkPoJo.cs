using Newtonsoft.Json;
using Pkn_HostSystem.Base;

namespace Pkn_HostSystem.Pojo.Page.HomePage;

public class NetWorkPoJo
{
    //连接名 ,用于分辨当前连接
    public string NetWorkId { get; set; }

    // 连接体 <--> 用于发送Modbus
    [JsonIgnore] public ModbusBase ModbusBase { get; set; }



    //tcp客户端服务器,用于发送Tcp
    [JsonIgnore] public WatsonTcpTool WatsonTcpTool { get; set; }

    //令牌 控制线程开启关闭
    [JsonIgnore] public CancellationTokenSource CancellationTokenSource { get; set; }


    //任务-->网络任务
    [JsonIgnore] public Lazy<Task> Task { get; set; }

    //网络信息
    public ConnectPojo ConnectPojo { get; set; }
}