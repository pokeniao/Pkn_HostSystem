using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.NodifyControl.Nodes.Connector;

namespace Pkn_HostSystem.NodifyControl.ViewModels.Connection
{

    /// <summary>
    /// 连接线的视图模型，包含连接的起点和终点
    /// </summary>
    public partial class ConnectorViewModel : ObservableObject
    {
        public MyConnector Source { get; set; }
        public MyConnector Target { get; set; }



        [ObservableProperty] private bool _isActive = false;

        /// <summary>
        /// 已连接上,创建连接逻辑
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public ConnectorViewModel(MyConnector source, MyConnector target)
        {
         
            Source = source;
            Target = target;

            //添加到连接器的观察者列表中
            Source.ValueObservers.Add(Target);
            
            //连接的时候把结果给过去
            Target.InputValue.Add(Source.Value);
            Source.IsConnected = true;
            Target.IsConnected = true;
        }
    }
}