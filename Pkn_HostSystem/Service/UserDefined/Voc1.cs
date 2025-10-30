using CommunityToolkit.Mvvm.DependencyInjection;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Base.Halcon;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Pojo;
using Pkn_HostSystem.Service.Stations;
using Pkn_HostSystem.Service.UserDefined.Interface;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Page;
using System.Globalization;
using System.Text;

namespace Pkn_HostSystem.Service.UserDefined
{
    public class Voc1 : IUserDefined
    {
        public async Task<(bool Succeed, object Return)> Main(CancellationTokenSource cts, params object[] args)
        {
         
            //一.获取数据
            var eachStation = TraceContext.GetParam("EachStation");
            //获取工站
            EachStation<Station1>? StationBase = eachStation as EachStation<Station1>;
            Station1 station1 = new Station1();
            //添加一行数据到显示
            StationBase.AddItem(station1);
            StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, StationBase.Header, "开始VOC测试", false);

            HomePageViewModel homePageViewModel = Ioc.Default.GetRequiredService<HomePageViewModel>();
            //获取参数
            VOCPojo vocPojo = homePageViewModel.HomePageModel.VocPojo;
            //获取串口
            //获得网络,遍历获取对应的网络
            var netWork = GlobalManager.GetNetWork("VOC检漏仪器1(不能修改名称不然会运行失败)");
            if (netWork == null)
            {
                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header, "获取不到串口网络",false);
                return (false, "获取不到串口网络");
            }
            // 获取串口
            ScpiSerialTool scpiSerialTool = netWork.ScpiSerialTool;

          
            

            //获取PLC
            var netWork2 = GlobalManager.GetNetWork("PLC(不能修改名称不然会运行失败)");
            if (netWork2 == null)
            {
                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header, "获取不到PLC网络", false);
                return (false, "获取不到PLC网络");
            }
            //获取PLC的 ModbusBase
            ModbusBase PlcModbusTcp = netWork2.ModbusBase;

            station1.电芯条码1 = GetString(await PlcModbusTcp.ReadHoldingRegisters_03(1, 0, 20));
            station1.电芯条码2 = GetString(await PlcModbusTcp.ReadHoldingRegisters_03(1, 20, 20));
            station1.腔体号 = GetOneRegister(await PlcModbusTcp.ReadHoldingRegisters_03(1, 40, 1)).ToString();
            station1.正压值 = GetOneRegister(await PlcModbusTcp.ReadHoldingRegisters_03(1, 41, 1)).ToString();
            station1.负压值 = GetOneRegister(await PlcModbusTcp.ReadHoldingRegisters_03(1, 42, 1)).ToString();

            double MaxValue =0;
            // 检测时长
            for (int i = 1; i <= (int)vocPojo.TestTime; i++)
            {
                await Task.Delay(1000);
                //先读一次清理缓存
                scpiSerialTool.ClearSerialChannel();
                (bool succeed, string response) = await scpiSerialTool.ReadLine();
                if (!succeed)
                {
                    StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header, "串口读取超时/失败", false);
                    return (false, "串口读取超时/失败");
                }
                succeed = double.TryParse(response, out double responseDouble);
                if (!succeed)
                {
                    StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header, "数据转成double类型失败", false);
                    return (false, "数据转成double类型失败");
                }
                    
                    
                //存储最大的数值
                if (responseDouble > MaxValue)
                {
                    MaxValue = responseDouble;
                }
                switch (i)
                {
                    case 1:
                        station1.Voc_1s = response;
                        break;
                    case 2:
                        station1.Voc_2s = response;
                        break;
                    case 3:
                        station1.Voc_3s = response;
                        break;
                    case 4:
                        station1.Voc_4s = response;
                        break;
                    case 5:
                        station1.Voc_5s = response;
                        break;
                    case 6:
                        station1.Voc_6s = response;
                        break;
                }
            }
            //存储数值
            station1.VOC最大值 = MaxValue.ToString();

            //比较大小
            if(MaxValue > vocPojo.TriggerMax)
            {
                station1.结果 = "NG";
                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, StationBase.Header, "结果NG", false);
                return (false, "结果NG");
            }
            //返回结果
            station1.结果 = "OK";
            StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, StationBase.Header, "结果OK", false);
            return (true,default);
        }

        public async Task<string> ErrorMessage(CancellationTokenSource cts, params object[] args)
        {
            throw new NotImplementedException();
        }

        public string GetString(ushort[] readHoldingRegisters03)
        {
            var result_4 = new List<byte>();
            foreach (var itemUshort in readHoldingRegisters03)
            {
                //转成16进制
                var value = itemUshort.ToString("x4");
                //从2索引截取到结尾
                var high = value.Substring(2);
                var low = value.Substring(0, 2);
                var ByteLow = byte.Parse(low, NumberStyles.HexNumber);
                var ByteHigh = byte.Parse(high, NumberStyles.HexNumber);

                //高位在前
                result_4.Add(ByteLow);
                result_4.Add(ByteHigh);
            }

            //输出ASCII码转换后的结果
            return  Encoding.ASCII.GetString(result_4.ToArray()).Trim('\0');
        }


        public int GetOneRegister(ushort[] readHoldingRegisters03)
        {
            return readHoldingRegisters03[0];
        }
    }
}