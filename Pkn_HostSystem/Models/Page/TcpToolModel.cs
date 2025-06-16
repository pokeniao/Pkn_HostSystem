using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Base;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.Models.Page
{
    public partial class TcpToolModel : ObservableObject
    {
        //连接服务器的IP
        [ObservableProperty] private string connectServerIp;
        //连接服务器的端口
        [ObservableProperty] private string connectServerPost;
        //服务器端口
        [ObservableProperty] private string serverPost;
        //服务器是否监听
        [ObservableProperty] private bool isLearn;

       
        //选择客户端
        [ObservableProperty] private ObservableCollection<TcpNet> selectClient = new ();
        //选择服务器
        [ObservableProperty] private ObservableCollection<TcpNet> selectServer = new ();
        //选中的客户端
        [ObservableProperty] private TcpNet selectClientTcpNet;
        //选中的服务器
        [ObservableProperty] private TcpNet selectServerTcpNet;


    }

    public partial class TcpNet:ObservableObject
    {
        public string Ip { get; set; }

        public string Post { get; set; }

        private string ipAndPost;
        public string IpAndPost
        {
            get
            {
                return Ip + ":" + Post;
            }
            set
            {
                ipAndPost = value;
                string[] strings = value.Split(':');
                Ip = strings[3].Remove(strings[3].Length - 1);
                Post = strings[4];
            }
        }
        /// <summary>
        /// 接受消息事件
        /// </summary>
        public Action<string, string> OnMessageReceived;



        [ObservableProperty] private string acceptMessage;

        [ObservableProperty] private string sendMessage;

        //以连接的客户端
        [ObservableProperty] private ObservableCollection<string> serverConnectClient = new();
        public bool isLearn { get; set; }

        public TcpTool  TcpTool { get; set; }

    }

}