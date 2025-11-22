using CommunityToolkit.Mvvm.DependencyInjection;
using log4net;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Pojo;
using Pkn_HostSystem.Service.Stations;
using Pkn_HostSystem.Service.UserDefined.Interface;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Page;
using System.Net.Sockets;

namespace Pkn_HostSystem.Service.UserDefined
{
    public class SR1000 : IUserDefined
    {
        public async Task<(bool Succeed, object Return)> Main(CancellationTokenSource cts, params object[] args)
        {
            //一.获取数据
            var eachStation = TraceContext.GetParam("EachStation");
            //获取工站
            EachStation<Station1>? StationBase = eachStation as EachStation<Station1>;
            if (StationBase ==null)
            {
                return (false, "工站信息获取失败");
            }

            StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, StationBase.Header, "开始扫码", false);

            //获取串口
            //获得网络,遍历获取对应的网络
            var netWork = GlobalManager.GetNetWork("基恩士扫码枪(不能修改名称不然会运行失败)");
            if (netWork == null)
            {
                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header, "获取与扫码枪的连接", false);
                return (false, "获取与扫码枪的连接");
            }
            // 获取TCP通讯
            TcpTool netWorkTcpTool = netWork.TcpTool;

            //获取PLC
            var netWork2 = GlobalManager.GetNetWork("PLC(不能修改名称不然会运行失败)");
            if (netWork2 == null)
            {
                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header, "获取不到PLC网络", false);
                return (false, "获取不到PLC网络");
            }
            (bool succeed, string? response) = await netWorkTcpTool.SendAndWaitClientAsync("LON\r",cts);
            if (!succeed)
            {
                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header, "扫码失败,扫码枪未返回数据", false);
                return (false, "扫码失败,扫码枪未返回数据");
            }
            //处理数据
            string[] strings = response.Split(":");
            //获取第一段
            response = strings[0];

            StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, StationBase.Header, $"条码:{response}", false);


            //获取PLC的 ModbusBase
            ModbusBase PlcModbusTcp = netWork2.ModbusBase;

            List<ushort> list = new List<ushort>();
            try
            {
                //按高低位写入
                for (int i = 0; i < response.Length; i += 2)
                {
                    char high = response[i];
                    char low = (i + 1 < response.Length) ? response[i + 1] : '\0'; // 补0
                    ushort packed = (ushort)((high << 8) | low);
                    list.Add(packed);
                }
                ushort[] result = list.ToArray();
                await PlcModbusTcp.WriteRegisters_10(
                    1,20
                    , result);
            }
            catch (Exception e)
            {
                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header, $"扫码结果写入PLC失败{e}", false);
                return (false, null);
            }

            // 检测时长
            return (true, default);
        }

        public async Task<string> ErrorMessage(CancellationTokenSource cts, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}