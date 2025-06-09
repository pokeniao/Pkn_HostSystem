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
        //客户端接收消息
        [ObservableProperty] private string clientAcceptMessage;
        //客户端发送消息
        [ObservableProperty] private string clientSendMessage;
        //服务器端口
        [ObservableProperty] private string serverPost;
        //服务器接收消息
        [ObservableProperty] private string serverAcceptMessage;
        //服务器发送消息
        [ObservableProperty] private string serverSendMessage;
        //服务器是否监听
        [ObservableProperty] private bool isLearn;

        [ObservableProperty] private ObservableCollection<string> serverConnectClient = new ();
        //选择客户端
        [ObservableProperty] private ObservableCollection<TcpNet> selectClient = new ();
        //选择服务器
        [ObservableProperty] private ObservableCollection<TcpNet> selectServer = new ();
    }

    public class TcpNet
    {
        public string Ip { get; set; }

        public string Post { get; set; }


        public string IpAndPost
        {
            get
            {
                return Ip + ":" + Post;
            }

            set => IpAndPost = value;

        }

        public string AcceptMessage { get; set; }

        public string SendMessage { get; set; }

        public bool isLearn { get; set; }

        public TcpTool  TcpTool { get; set; }

    }

}