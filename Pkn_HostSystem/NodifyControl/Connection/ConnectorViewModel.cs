using Pkn_HostSystem.NodifyControl.Node.Connector;

namespace Pkn_HostSystem.NodifyControl.Connection
{

    /// <summary>
    /// 连接线的视图模型，包含连接的起点和终点
    /// </summary>
    public class ConnectorViewModel
    {
        public MyConnector Source { get; set; }
        public MyConnector Target { get; set; }

        public ConnectorViewModel(MyConnector source, MyConnector target)
        {
            Source = source;
            Target = target;
            Source.IsConnected = true;
            Target.IsConnected = true;
        }
    }
}