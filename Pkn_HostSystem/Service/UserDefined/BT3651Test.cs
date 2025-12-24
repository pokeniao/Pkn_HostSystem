using Azure.Core;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Models.Pojo;
using Pkn_HostSystem.Service.Stations;
using Pkn_HostSystem.Service.UserDefined.Interface;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Page;
using RestSharp;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Documents;


namespace Pkn_HostSystem.Service.UserDefined
{
    public class BT3651Test : IUserDefined
    {
        public async Task<(bool Succeed, object Return)> Main(CancellationTokenSource cts, params object[] args)
        {
            try
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
                //获取PLC
                var plcNetWork = GlobalManager.GetNetWork("PLC");
                if (plcNetWork == null)
                {
                    StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header,
                        "GetNetWork获取不到PLC");
                    return (false, "GetNetWork获取不到PLC");
                }

                KeyenceHostLinkTool keyenceHostLinkTool = plcNetWork.KeyenceHostLinkTool;
                //本地获取条码

                (bool s, ushort[] responseUshorts) = await keyenceHostLinkTool.ReadDMWords(1040, 20, cts);

                if (!s || responseUshorts == null)
                {
                    StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header,
                        "基恩士上链路读取DM失败");
                    return (false, null);
                }

                ushort[] readDmWords = responseUshorts;

                var bytes = new List<byte>();
                foreach (var itemUshort in readDmWords)
                {
                    //转成16进制
                    var v = itemUshort.ToString("x4");
                    //从2索引截取到结尾
                    var high = v.Substring(2);
                    var low = v.Substring(0, 2);
                    var ByteLow = byte.Parse(low, NumberStyles.HexNumber);
                    var ByteHigh = byte.Parse(high, NumberStyles.HexNumber);

                    //低位在前
                    bytes.Add(ByteLow);
                    bytes.Add(ByteHigh);
                }

                //输出ASCII码转换后的结果
                electricityTest.CurSN = Encoding.ASCII.GetString(bytes.ToArray());


                //获取对应条码行
                Station1 station1 = null;
                for (int i = StationBase.Items.Count - 1; i >= 0; i--)
                {
                    if (StationBase.Items[i].条码 == electricityTest.CurSN)
                    {
                        station1 = StationBase.Items[i];
                    }
                }

                if (station1 == null)
                {
                    station1 = new Station1();
                    station1.条码 = electricityTest.CurSN;
                    station1.时间 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    station1.型号 = electricityTest.CurSN.Substring(0, 2);
                    station1.批号 = electricityTest.CurSN.Substring(2, 4);
                }


                //获取串口
                //获得网络,遍历获取对应的网络
                var netWork = GlobalManager.GetNetWork("电测仪器");
                if (netWork == null)
                {
                    StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header,
                        "GetNetWork获取不到电测仪器");
                    return (false, "GetNetWork获取不到电测仪器");
                }

                // 获取串口通讯
                ScpiSerialTool scpiSerialTool = netWork.ScpiSerialTool;
                //进行电测
                (bool succeed, string response) = await scpiSerialTool.WriteLineAndWaitResponse(":FETCh?\r\n");
                if (!succeed)
                {
                    StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header,
                        "电测仪串口WriteLineAndWaitResponse执行失败");
                    return (false, "电测仪串口WriteLineAndWaitResponse执行失败");
                }

                //处理电测字符串
                response = response.Replace(" ", "").Trim();
                string[] strings = response.Split(",");

                if (strings.Length != 2)
                {
                    //电测失败
                    StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header,
                        "电测返回结果不符合规则");
                    return (false, "电测返回结果不符合规则");
                }

                //阻值 单位mΩ
                double value = double.Parse(strings[0]) * 1000;
                //电压 单位V
                double value2 = double.Parse(strings[1]);

                double RHight = double.Parse(electricityTest.ResistanceUpLimit.ToString());
                double RLow = double.Parse(electricityTest.ResistanceLowLimit.ToString());
                double VHight = double.Parse(electricityTest.VoltageUpLimit.ToString());
                double VLow = double.Parse(electricityTest.VoltageLowLimit.ToString());

                bool result = true;
                if (!(RHight > value && value > RLow))
                {
                    StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, StationBase.Header,
                        "得到电阻结果NG");
                    result = false;
                }

                if (!(VHight > value2 && value2 > VLow))
                {
                    StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, StationBase.Header,
                        "得到电压结果NG");
                    result = false;
                }

                station1.电阻上限 = RHight.ToString("0.000");
                station1.电阻值 = value.ToString("0.000");
                station1.电阻下限 = RLow.ToString("0.000");
                station1.电压上限 = VHight.ToString("0.000");
                station1.电压值 = value2.ToString("0.000");
                station1.电压下限 = VLow.ToString("0.000");

                if (result)
                {
                    station1.合格 = "OK";
                    StaticArrayRegister.WriteRegisterValue(0, 2);
                }
                else
                {
                    station1.合格 = "NG";
                    StaticArrayRegister.WriteRegisterValue(0, 3);
                }

                var jsonSave = new
                {
                    时间 = station1.时间,
                    条码 = station1.条码,
                    型号 = station1.型号,
                    批号 = station1.批号,
                    电阻上限 = station1.电阻上限,
                    电阻值 = station1.电阻值,
                    电阻下限 = station1.电阻下限,
                    电压上限 = station1.电压上限,
                    电压值 = station1.电压值,
                    电压下限 = station1.电压下限,
                    合格 = station1.合格
                };

                string save = JsonConvert.SerializeObject(jsonSave);
                //不存在,创建
                string saveDirectory = Path.Combine(GlobalManager.SaveFile, "电测");
                if (!Directory.Exists(saveDirectory))
                    Directory.CreateDirectory(saveDirectory);
                string FilePath = Path.Combine(saveDirectory, $"{DateTime.Now:yyyy-MM-dd}.csv");
                CsvHelper csvHelper = new CsvHelper(FilePath);
                csvHelper.Load();

                int rowIndexByCellValue = csvHelper.GetRowIndexByCellValue(1, electricityTest.CurSN);
                save = JsonTool<object>.TryFormatJson(save, out bool isJson);
                if (rowIndexByCellValue == -1)
                {
                    csvHelper.AddRowFromJson(save);
                }
                else
                {
                    csvHelper.UpdateRowFromJson(rowIndexByCellValue, save);
                }

                csvHelper.Save(cts.Token);
                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, StationBase.Header, "本地日志已更新");
                //上传MES
                RestClient restClient = new RestClient(electricityTest.HttpPath);
                RestRequest restRequest = new RestRequest(electricityTest.ApiPath, Method.Post);
                var data = new
                {
                    TABLE = "mes_pack_tcodyjc",
                    DCNO = "DCNO",
                    USERNO = "USERNO",
                    CONTNO = "CONTNO",
                    O1 = $"{station1.条码}",
                    O2 = $"{electricityTest.MachineId}",
                    O3 = $"{electricityTest.GroupCode}",
                    O4 = $"{station1.型号}",
                    O5 = $"{station1.批号}",
                    O6 = $"{electricityTest.VoltageStandard}",
                    O7 = $"{electricityTest.ResistanceStandard}",
                    O8 = "",
                    O9 = $"{station1.电压值}",
                    O10 = $"{station1.电阻值}",
                    O11 = "",
                    O12 = "",
                    O13 = $"{station1.合格}",
                    O14 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    O15 = "",
                    O16 = "",
                    O17 = "",
                    O18 = "",
                    O19 = "",
                    O20 = "",
                    O21 = "",
                    O22 = "",
                    O23 = "",
                    O24 = "",
                    O25 = "",
                    O26 = "",
                    O27 = "",
                    O28 = "",
                    O29 = "",
                    O30 = ""
                };
                string json = JsonConvert.SerializeObject(data);
                string xmlData =
                    $"<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:webs=\"http://webs.modules.jeesite.thinkgem.com/\">\r\n   <soapenv:Header/>\r\n   <soapenv:Body>\r\n      <webs:GetDataDb>\r\n         <!--参数:-->\r\n          <arg0>dataSource11</arg0>\r\n         <arg1>{json}</arg1>\r\n      </webs:GetDataDb>\r\n   </soapenv:Body>\r\n</soapenv:Envelope> ";
                restRequest.AddStringBody(xmlData, DataFormat.Xml);
                RestResponse httpResponse = await restClient.ExecuteAsync(restRequest);
                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, StationBase.Header,
                    $"上传内容:{xmlData}");
                if (httpResponse.IsSuccessStatusCode)
                {
                    //判断是否是JSON格式,如果是转成输出
                    httpResponse.Content = JsonTool<Object>.TryFormatJson(httpResponse.Content, out _);
                    StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, StationBase.Header,
                        $"上传mes成功 返回:\r\n{httpResponse.Content}");
                }
                else
                {
                    StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header,
                        httpResponse.ErrorMessage == null ? httpResponse.Content : httpResponse.ErrorMessage);
                    return (false, $"mes上传失败");
                }

                return (true, default);
            }
            catch (Exception e)
            {
                return (false, $"{e}");
            }
        }

        public async Task<string> ErrorMessage(CancellationTokenSource cts, params object[] args)
        {
            try
            {
                string message = args[0] as string;
                //处理字符串
                message = message.Replace(" ", "").Trim();

                string[] strings = message.Split(",");

                if (strings.Length != 2)
                {
                    return "电测未正确返回数据";
                }

                //阻值 单位mΩ
                double value = double.Parse(strings[0]);

                double RHight = double.Parse(StaticArrayRegister.ReadRegisterValue(50).ToString());
                double RLow = double.Parse(StaticArrayRegister.ReadRegisterValue(51).ToString());
                double VHight = double.Parse(StaticArrayRegister.ReadRegisterValue(52).ToString());
                double VLow = double.Parse(StaticArrayRegister.ReadRegisterValue(53).ToString());
                if (!(RHight > value && value > RLow))
                {
                    return "电阻未达到条件";
                }


                //电压 单位V
                double value2 = double.Parse(strings[1]);

                if (!(VHight > value2 && value2 > VLow))
                {
                    return "电压未达到条件";
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }

            return "未知错误";
        }
    }
}