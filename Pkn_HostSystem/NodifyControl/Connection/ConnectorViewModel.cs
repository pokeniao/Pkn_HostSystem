using Pkn_HostSystem.NodifyControl.Node.Connector;

namespace Pkn_HostSystem.NodifyControl.Connection
{

    /// <summary>
    /// 连接线的视图模型，包含连接的起点和终点
    /// </summary>
    public class ConnectorViewModel
    {
        public Connector Source { get; set; }
        public Connector Target { get; set; }

        public ConnectorViewModel(Connector source, Connector target)
        {
            Source = source;
            Target = target;
            Source.IsConnected = true;
            Target.IsConnected = true;
        }
    }
}