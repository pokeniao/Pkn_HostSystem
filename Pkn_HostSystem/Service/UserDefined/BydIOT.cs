using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Static;
using System.Net.Sockets;

namespace Pkn_HostSystem.Service.UserDefined
{
    public class BydIOT :IUserDefined
    {

        public LogBase<BydIOT> Log = new();
        public object GetPropertyValue(string key)
        {
            return null;
        }

        public async Task<(bool Succeed, object Return)> Main(CancellationTokenSource cts)
        {
            var netWorkPoJoes = GlobalMannager.NetWorkDictionary.Items.ToList();
            NetWork netWork = null;
            foreach (var netWorkPoJo in netWorkPoJoes)
            {
                if (netWorkPoJo.NetworkDetailed.Name == "上位机服务器")
                {
                     netWork = netWorkPoJo;
                }
            }
            if (netWork == null)
            {
                Log.Error($"[{TraceContext.Name}]--没有配置名为:[上位机服务器]的TCP服务器通讯连接");
                return (false, $"[{TraceContext.Name}]--没有配置名为:[上位机服务器]的TCP服务器通讯连接");
            }

            TcpTool tcpTool = netWork.TcpTool;
            var  cArray = tcpTool._clients.ToArray();
            if (cArray.Length == 0)
            {
                Log.Error($"[{TraceContext.Name}]--没有任何客户端进行连接");
                return (false, $"[{TraceContext.Name}]--没有任何客户端进行连接");
            }

            TcpClient tcpClient = cArray[0].Value;
            string ip = cArray[0].Key;
            //从ip中获取数据
            List<string> list = tcpTool._clientsResponse[ip];
            //获取最新的一行数据
            int responseIndex = list.Count;
            if (responseIndex!=0)
            {
                string response = list[responseIndex-1];
                Log.Info($"[{TraceContext.Name}]--服务器监听来自[{ip}]内容 {response}");
                return (true, response);
            }

            return (false, null);
        }

        public string ErrorMessage()
        {
            return "";
        }
    }
}