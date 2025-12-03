using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Models.Pojo;
using Pkn_HostSystem.Services.Stations;
using Pkn_HostSystem.Services.UserDefined.Interface;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Page;
using RestSharp;
using System.Globalization;
using System.IO;
using System.Text;

namespace Pkn_HostSystem.Services.UserDefined
{
    public class Voc1 : IUserDefined
    {
        public async Task<(bool Succeed, object Return)> Main(CancellationTokenSource cts, params object[] args)
        {
            try
            {
                StaticArrayRegister.WriteRegisterValue(0, 0);
                //一.获取数据
                var eachStation = TraceContext.GetParam("EachStation");
                //获取工站
                EachStation<Station1>? StationBase = eachStation as EachStation<Station1>;
                if (StationBase == null)
                {
                    return (false, "工站信息获取失败");
                }
                Station1 station1 = new Station1();
                //添加一行数据到显示
                StationBase.AddItem(station1);
                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, StationBase.Header, "开始VOC测试", true);
                HomePageViewModel homePageViewModel = Ioc.Default.GetRequiredService<HomePageViewModel>();
                //获取参数
                VOCPojo vocPojo = homePageViewModel.HomePageModel.VocPojo;
                //获取串口
                //获得网络,遍历获取对应的网络
                var netWork = GlobalManager.GetNetWork("VOC检漏仪器1(不能修改名称不然会运行失败)");
                if (netWork == null)
                {
                    StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header, "获取不到串口网络", true);
                    return (false, "获取不到串口网络");
                }
                // 获取串口
                ScpiSerialTool scpiSerialTool = netWork.ScpiSerialTool;
                //获取PLC
                var netWork2 = GlobalManager.GetNetWork("PLC(不能修改名称不然会运行失败)");
                if (netWork2 == null)
                {
                    StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header, "获取不到PLC网络", true);
                    return (false, "获取不到PLC网络");
                }
                //获取PLC的 ModbusBase
                ModbusBase PlcModbusTcp = netWork2.ModbusBase;
                station1.电芯条码1 = GetString(await PlcModbusTcp.ReadHoldingRegisters_03(1, 40, 10)).Trim('\0').Trim('\n').Trim('\r');
                station1.电芯条码2 = GetString(await PlcModbusTcp.ReadHoldingRegisters_03(1, 60, 10)).Trim('\0').Trim('\n').Trim('\r');
                station1.腔体号 = GetOneRegister(await PlcModbusTcp.ReadHoldingRegisters_03(1, 120, 2)).ToString();
                station1.正压值 = GetOneRegister(await PlcModbusTcp.ReadHoldingRegisters_03(1, 122, 2)).ToString();
                station1.负压值 = GetOneRegister(await PlcModbusTcp.ReadHoldingRegisters_03(1, 124, 2)).ToString();
                //保压时间
                string holeTime ="";
                //冲正压泄压时间
                string preTime = "";

                switch (station1.腔体号)
                {
                    case "1":
                    //保压时间
                    holeTime = GetOneRegister(await PlcModbusTcp.ReadHoldingRegisters_03(1, 190, 2)).ToString();
                    preTime= GetOneRegister(await PlcModbusTcp.ReadHoldingRegisters_03(1, 200, 2)).ToString();
                        break;
                    case "2":
                        //保压时间
                        holeTime = GetOneRegister(await PlcModbusTcp.ReadHoldingRegisters_03(1, 191, 2)).ToString();
                        preTime = GetOneRegister(await PlcModbusTcp.ReadHoldingRegisters_03(1, 201, 2)).ToString();
                        break;
                    case "3":
                        //保压时间
                        holeTime = GetOneRegister(await PlcModbusTcp.ReadHoldingRegisters_03(1, 192, 2)).ToString();
                        preTime = GetOneRegister(await PlcModbusTcp.ReadHoldingRegisters_03(1, 202, 2)).ToString();
                        break;
                    case "4":
                        //保压时间
                        holeTime = GetOneRegister(await PlcModbusTcp.ReadHoldingRegisters_03(1, 193, 2)).ToString();
                        preTime = GetOneRegister(await PlcModbusTcp.ReadHoldingRegisters_03(1, 203, 2)).ToString();
                        break;
                    case "5":
                        //保压时间
                        holeTime = GetOneRegister(await PlcModbusTcp.ReadHoldingRegisters_03(1, 194, 2)).ToString();
                        preTime = GetOneRegister(await PlcModbusTcp.ReadHoldingRegisters_03(1, 204, 2)).ToString();
                        break;
                    case "6":
                        //保压时间
                        holeTime = GetOneRegister(await PlcModbusTcp.ReadHoldingRegisters_03(1, 195, 2)).ToString();
                        preTime = GetOneRegister(await PlcModbusTcp.ReadHoldingRegisters_03(1, 205, 2)).ToString();
                        break;
                }
                double MaxValue =0;
                //等待3秒,管道气体流入需要时间
                await Task.Delay(3000);
                // 检测时长
                for (int i = 1; i <= (int)vocPojo.TestTime; i++)
                {
                    await Task.Delay(1000);
                    //先读一次清理缓存
                    scpiSerialTool.ClearSerialChannel();
                    (bool succeed, string response) = await scpiSerialTool.ReadLine();
                    if (!succeed)
                    {
                        StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header, "串口读取超时/失败", true);
                        return (false, "串口读取超时/失败");
                    }

                    //处理字符串
                    string[] strings = response.Split(":");
                    response = strings[1];
                    response = response.Substring(0, response.IndexOf("PPB"));

                    succeed = double.TryParse(response, out double responseDouble);
                    if (!succeed)
                    {
                        StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header, "数据转成double类型失败", true);
                        return (false, "数据转成double类型失败");
                    }
                    StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, StationBase.Header, $"检测腔体{station1.腔体号},第{i}秒,VOC数据:{responseDouble}", true);
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
                        case 7:
                            station1.Voc_7s = response;
                            break;
                        case 8:
                            station1.Voc_8s = response;
                            break;
                        case 9:
                            station1.Voc_9s = response;
                            break;
                        case 10:
                            station1.Voc_10s = response;
                            break;
                        case 11:
                            station1.Voc_11s = response;
                            break;
                        case 12:
                            station1.Voc_12s = response;
                            break;
                        case 13:
                            station1.Voc_13s = response;
                            break;
                        case 14:
                            station1.Voc_14s = response;
                            break;
                        case 15:
                            station1.Voc_15s = response;
                            break;
                        case 16:
                            station1.Voc_16s = response;
                            break;
                        case 17:
                            station1.Voc_17s = response;
                            break;
                        case 18:
                            station1.Voc_18s = response;
                            break;
                        case 19:
                            station1.Voc_19s = response;
                            break;
                        case 20:
                            station1.Voc_20s = response;
                            break;
                    }
                }
                //存储数值
                station1.VOC最大值 = MaxValue.ToString();

                //比较大小
                if(MaxValue > vocPojo.TriggerMax)
                {
                    station1.结果 = "NG";
                    StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, StationBase.Header, "结果NG", true);
                    //将电阻 电压写入到寄存器中
                    StaticArrayRegister.WriteRegisterValue(0, 3);
                }
                else
                {
                    station1.结果 = "OK";
                    StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, StationBase.Header, "结果OK", true);
                    StaticArrayRegister.WriteRegisterValue(0, 2);
                }

                string save =
                    "{\r\n\"时间\":\"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\",\r\n\"条码一\":\"" + station1.电芯条码1 + "\",\r\n\"条码二\":\"" + station1.电芯条码2 + "\",\r\n\"腔体号\":\"" + station1.腔体号 + "\",\r\n\"正压值\":\"" + station1.正压值 + "\",\r\n\"负压值\":\"" + station1.负压值 + "\",\r\n\"结果\":\"" + station1.结果 + "\",\r\n\"检测第一秒\":\"" + station1.Voc_1s + "\",\r\n\"检测第二秒\":\"" + station1.Voc_2s + "\",\r\n\"检测第三秒\":\"" + station1.Voc_3s + "\",\r\n\"检测第四秒\":\"" + station1.Voc_4s + "\",\r\n\"检测第五秒\":\"" + station1.Voc_5s + "\",\r\n\"检测第六秒\":\"" + station1.Voc_6s + "\"}";
                //本地保存
                //不存在,创建
               string saveDirectory = Path.Combine(GlobalManager.SaveFile,"VOC");
                if (!Directory.Exists(saveDirectory))
                    Directory.CreateDirectory(saveDirectory);
                string FilePath = Path.Combine(saveDirectory, $"{DateTime.Now:yyyy-MM-dd}.csv");
                CsvHelper csvHelper = new CsvHelper(FilePath);
                csvHelper.Load();
                save = JsonTool<object>.TryFormatJson(save, out bool isJson);
                csvHelper.AddRowFromJson(save);
                csvHelper.Save(cts.Token);
                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, StationBase.Header, "本地日志已更新", true);

                if (vocPojo.MesOn)
                {
                    if (!station1.电芯条码1.IsNullOrEmpty())
                    {
                        //上传Mes
                        RestClient restClient = new RestClient(vocPojo.MesHttp);
                        RestRequest restRequest = new RestRequest("/ProductionGroupInfo", Method.Post);
                        UserLoginModel userLoginModel = homePageViewModel.UserLoginModel;
                        var data = new
                        {
                            GroupCode = $"{vocPojo.GroupCode}",
                            MachineId = $"{vocPojo.MachineId}",
                            OperatorId = $"{userLoginModel.Id}",
                            TimeStamp = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                            ProductSn = $"{station1.电芯条码1}",
                            SnType = "cell",
                            SubSn = "",
                            TestResult = station1.结果 == "OK" ? 0 : 1,
                            NgCode = "",
                            TestData = new[]
                            {
                        new
                        {
                            ItemCode = "HOLDING_TIME",
                            ItemName = "保压时间",
                            Value = holeTime,
                            ItemResult = "0"
                        },
                        new
                        {
                            ItemCode = "PRESSURE_RELIEF_TIME",
                            ItemName = "泄压时间",
                            Value = preTime,
                            ItemResult = "0"
                        },
                        new
                        {
                            ItemCode = "CHARGING_TIME",
                            ItemName = "充正压时间",
                            Value = preTime,
                            ItemResult = "0"
                        },
                        new
                        {
                            ItemCode = "TEST_TIME",
                            ItemName = "测试时间",
                            Value = $"{vocPojo.TestTime}",
                            ItemResult = "0"
                        },
                        new
                        {
                            ItemCode = "POSITIVE_PRESSURE",
                            ItemName = "正压压力",
                            Value = station1.正压值,
                            ItemResult = "0"
                        },
                        new
                        {
                            ItemCode = "NEGATIVE_PRESSURE",
                            ItemName = "负压压力",
                            Value = station1.负压值,
                            ItemResult = "0"
                        },
                        new
                        {
                            ItemCode = "OCV_MAXVALUE",
                            ItemName = "VOC最大值",
                            Value = station1.VOC最大值,
                            ItemResult = "0"
                        },
                        new
                        {
                            ItemCode = "CHANNEL_NUMBER",
                            ItemName = "通道号",
                            Value = station1.腔体号,
                            ItemResult = "0"
                        }
                    },
                            StepData = new[]
                            {
                        new
                        {
                            TRAY_ID = "",
                            TypeName = "",
                            StepNo = "",
                            CHANNEL_ID = "",
                            BATCH_ID = "",
                            STEP = "",
                            STEP_NAME = "",
                            START_DATE = "",
                            END_DATE = "",
                            CIRCULATING_NUMBER = "",
                            TURN_TIME = "",
                            END_ELECTRICITY = "",
                            CAPACITY = "",
                            ENERGY = "",
                            CONSTANT_CURRENT = "",
                            START_VOL = "",
                            MID_VOL = "",
                            END_VOL = "",
                            CHARGE_ELECTRICITY = "",
                            MARKING = "",
                            EndTemperature = "",
                            AVG_VOL = "",
                            ItemResult = ""
                        }
                    },

                        };
                        string json = JsonConvert.SerializeObject(data);
                        restRequest.AddParameter("jsonData", json);
                        RestResponse httpResponse = await restClient.ExecuteAsync(restRequest);
                        StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, StationBase.Header, $"上传Json:{json}", true);
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            JObject jObject = JObject.Parse(httpResponse.Content);


                            if (jObject["status"]?.ToString() == "true")
                            {
                                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, StationBase.Header, "上传mes成功", true);
                                station1.Mes上传 = "成功";
                            }
                            else
                            {
                                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header, $"上传mes失败,{jObject["result"]?.ToString()}", true);
                                station1.Mes上传 = "失败";
                            }
                        }
                        else
                        {
                            StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header, httpResponse.ErrorMessage == null ? httpResponse.Content : httpResponse.ErrorMessage, true);
                            station1.Mes上传 = "失败";
                        }
                    }

                    if (!station1.电芯条码2.IsNullOrEmpty())
                    {
                        //上传Mes
                        RestClient restClient = new RestClient(vocPojo.MesHttp);
                        RestRequest restRequest = new RestRequest("/ProductionGroupInfo", Method.Post);
                        UserLoginModel userLoginModel = homePageViewModel.UserLoginModel;
                        var data = new
                        {
                            GroupCode = $"{vocPojo.GroupCode}",
                            MachineId = $"{vocPojo.MachineId}",
                            OperatorId = $"{userLoginModel.Id}",
                            TimeStamp = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                            ProductSn = $"{station1.电芯条码2}",
                            SnType = "cell",
                            SubSn = "",
                            TestResult = station1.结果 == "OK" ? 0 : 1,
                            NgCode = "",
                            TestData = new[]
                            {
                        new
                        {
                            ItemCode = "HOLDING_TIME",
                            ItemName = "保压时间",
                            Value = holeTime,
                            ItemResult = "0"
                        },
                        new
                        {
                            ItemCode = "PRESSURE_RELIEF_TIME",
                            ItemName = "泄压时间",
                            Value = preTime,
                            ItemResult = "0"
                        },
                        new
                        {
                            ItemCode = "CHARGING_TIME",
                            ItemName = "充正压时间",
                            Value = preTime,
                            ItemResult = "0"
                        },
                        new
                        {
                            ItemCode = "TEST_TIME",
                            ItemName = "测试时间",
                            Value = $"{vocPojo.TestTime}",
                            ItemResult = "0"
                        },
                        new
                        {
                            ItemCode = "POSITIVE_PRESSURE",
                            ItemName = "正压压力",
                            Value = station1.正压值,
                            ItemResult = "0"
                        },
                        new
                        {
                            ItemCode = "NEGATIVE_PRESSURE",
                            ItemName = "负压压力",
                            Value = station1.负压值,
                            ItemResult = "0"
                        },
                        new
                        {
                            ItemCode = "OCV_MAXVALUE",
                            ItemName = "VOC最大值",
                            Value = station1.VOC最大值,
                            ItemResult = "0"
                        },
                        new
                        {
                            ItemCode = "CHANNEL_NUMBER",
                            ItemName = "通道号",
                            Value = station1.腔体号,
                            ItemResult = "0"
                        }
                    },
                            StepData = new[]
                            {
                        new
                        {
                            TRAY_ID = "",
                            TypeName = "",
                            StepNo = "",
                            CHANNEL_ID = "",
                            BATCH_ID = "",
                            STEP = "",
                            STEP_NAME = "",
                            START_DATE = "",
                            END_DATE = "",
                            CIRCULATING_NUMBER = "",
                            TURN_TIME = "",
                            END_ELECTRICITY = "",
                            CAPACITY = "",
                            ENERGY = "",
                            CONSTANT_CURRENT = "",
                            START_VOL = "",
                            MID_VOL = "",
                            END_VOL = "",
                            CHARGE_ELECTRICITY = "",
                            MARKING = "",
                            EndTemperature = "",
                            AVG_VOL = "",
                            ItemResult = ""
                        }
                    },

                        };
                        string json = JsonConvert.SerializeObject(data);
                        restRequest.AddParameter("jsonData", json);
                        RestResponse httpResponse = await restClient.ExecuteAsync(restRequest);
                        StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, StationBase.Header, $"上传Json:{json}", true);
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            JObject jObject = JObject.Parse(httpResponse.Content);


                            if (jObject["status"]?.ToString() == "true")
                            {
                                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, StationBase.Header, "上传mes成功", true);
                                station1.Mes上传 = "成功";
                            }
                            else
                            {
                                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header, $"上传mes失败,{jObject["result"]?.ToString()}", true);
                                station1.Mes上传 = "失败";
                            }
                        }
                        else
                        {
                            StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, StationBase.Header, httpResponse.ErrorMessage == null ? httpResponse.Content : httpResponse.ErrorMessage, true);
                            station1.Mes上传 = "失败";
                        }
                    }

                }
                else
                {
                    station1.Mes上传 = "关闭";
                }
                return (true, "");
            }
            catch (Exception e)
            {
                StaticArrayRegister.WriteRegisterValue(0, 4);
                return (false, e);
            }
            //字符串处理
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
            return (short)readHoldingRegisters03[0];
        }

    }
}