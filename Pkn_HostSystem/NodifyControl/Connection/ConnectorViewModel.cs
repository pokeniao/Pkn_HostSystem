using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.NodifyControl.Node.Connector;

namespace Pkn_HostSystem.NodifyControl.Connection
{

    /// <summary>
    /// 连接线的视图模型，包含连接的起点和终点
    /// </summary>
    public partial class ConnectorViewModel : ObservableObject
    {
        public MyConnector Source { get; set; }
        public MyConnector Target { get; set; }

        [ObservableProperty] private bool _isActive = false;
        public ConnectorViewModel(MyConnector source, MyConnector target)
        {
         
            Source = source;
            Target = target;

            Source.ValueObservers.Add(Target);
            Source.IsConnected = true;
            Target.IsConnected = true;
        }
    }
}