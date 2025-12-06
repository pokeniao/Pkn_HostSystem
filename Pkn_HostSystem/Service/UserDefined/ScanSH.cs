using CommunityToolkit.Mvvm.DependencyInjection;
using Newtonsoft.Json;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Pojo;
using Pkn_HostSystem.Service.Stations;
using Pkn_HostSystem.Service.UserDefined.Interface;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Page;
using System.Collections.ObjectModel;
using System.IO;

namespace Pkn_HostSystem.Service.UserDefined
{
    public class ScanSH : IUserDefined
    {
        public async Task<(bool Succeed, object Return)> Main(CancellationTokenSource cts, params object[] args)
        {
            //一.获取数据
            var eachStation = TraceContext.GetParam("EachStation");
            //获取工站
            EachStation<Station1>? StationBase = eachStation as EachStation<Station1>;
            if (StationBase == null)
            {
                return (false, "工站信息获取失败");
            }
            HomePageViewModel homePageViewModel = Ioc.Default.GetRequiredService<HomePageViewModel>();

            ElectricityTest electricityTest = homePageViewModel.HomePageModel.ElectricityTest;


            StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, StationBase.Header, "开始扫码");
            //获取相机

            //获取串口
            //获得网络,遍历获取对应的网络
            var netWork = GlobalManager.GetNetWork("相机");
            if (netWork == null)
            {
                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header, "GetNetWork获取不到相机");
                return (false, "GetNetWork获取不到相机");
            }
            // 获取串口通讯
            ScpiSerialTool scpiSerialTool = netWork.ScpiSerialTool;
            (bool succeed, string response) = await scpiSerialTool.WriteLineAndWaitResponse("LON\r");
            if (!succeed)
            {
                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header, "扫码串口WriteLineAndWaitResponse执行失败");
                return (false, "扫码串口WriteLineAndWaitResponse执行失败");
                
            }
            StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header, $"扫码成功返回:{response}");
            //记录当前扫码结果
            electricityTest.CurSN = response;
            Station1 station1 = new();

            station1.时间 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            station1.条码 = response;
            station1.型号 = response.Substring(0, 2);
            station1.批号 = response.Substring(2, 4);
            StationBase.AddItem(station1);
            var jsonSave = new
            {
                时间 = station1.时间,
                条码 = station1.条码,
                型号 = station1.型号,
                批号 = station1.批号
            };
            string save = JsonConvert.SerializeObject(jsonSave);
            //不存在,创建
            string saveDirectory = Path.Combine(GlobalManager.SaveFile, "电测");
            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);
            string FilePath = Path.Combine(saveDirectory, $"{DateTime.Now:yyyy-MM-dd}.csv");
            CsvHelper csvHelper = new CsvHelper(FilePath);
            csvHelper.Load();
            save = JsonTool<object>.TryFormatJson(save, out bool isJson);
            csvHelper.AddRowFromJson(save);
            csvHelper.Save(cts.Token);
            StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, StationBase.Header, "本地日志已更新");

            return (true, default);
        }

        public async Task<string> ErrorMessage(CancellationTokenSource cts, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}