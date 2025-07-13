using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Service.Stations.Interface;
using Pkn_HostSystem.Static;

namespace Pkn_HostSystem.Service.Stations
{
    public partial class Station1 : ObservableObject, IStation
    {
        //这样写的原因:[ObservableProperty]无法生成大写的中文
        private string _时间;

        public string 时间
        {
            get => _时间;
            set
            {
                SetProperty(ref _时间, value);
            }
        }


        private string _条码;

        public string 条码
        {
            get => _条码;
            set
            {
                SetProperty(ref _条码, value);
            }
        }

        private string _工单编码;

        public string 工单编码
        {
            get => _工单编码;
            set
            {
                SetProperty(ref _工单编码, value);
            }
        }

        private string _排程编码;

        public string 排程编码
        {
            get => _排程编码;
            set
            {
                SetProperty(ref _排程编码, value);
            }
        }

        [ObservableProperty] private string cT;

        private string _合格;

        public string 合格
        {
            get => _合格;
            set
            {
                SetProperty(ref _合格, value);
            }
        }


        /// <summary>
        /// 主入口
        /// </summary>
        /// <returns></returns>
        public async Task<(bool succeed, string message)> Main(CancellationTokenSource cts)
        {
            try
            {
                var eachStation = TraceContext.GetParam("EachStation");
                int step = TraceContext.GetParam("step");

                if (step == null)
                {
                    TraceContext.UpdateParam("step", 1);
                }

                switch (step)
                {
                    case 1:
                        TraceContext.UpdateParam("start", DateTime.Now);
                        //解析JSON
                        JObject jObject = JObject.Parse(TraceContext.GetParam("response"));
                        Station1 station1 = new Station1();
                        station1.时间 = DateTime.Now.ToString("yyyy-MM-mm:ss");
                        station1.工单编码 = jObject["workOrderNumber"]?.ToString();
                        station1.排程编码 = jObject["scheduleNumber"]?.ToString();
                        station1.条码 = jObject["snNumber"]?.ToString();
                        eachStation.AddItem(station1);
                        TraceContext.UpdateParam("curIndex", eachStation.Items.Count);
                        TraceContext.UpdateParam("step", 2);
                        return (true, null);

                    case 2:
                        Station1 eachStationItem = eachStation.Items[TraceContext.GetParam("curIndex") - 1];
                        TimeSpan t = DateTime.Now - TraceContext.GetParam("start");
                        eachStationItem.CT = t.Seconds.ToString();
                        //解析JSON
                       string json =JsonTool<object>.TryFormatJson(TraceContext.GetParam("response"), out bool isJson);

                        JObject jObject2 = null;
                        string? success =null;
                        string? fail =null;
                        string? code = null;
                        if (isJson)
                        {
                            jObject2 = JObject.Parse(json);
                            success = jObject2["success"]?.ToString();
                            fail = jObject2["fail"]?.ToString();
                            code = jObject2["code"]?.ToString();
                        }
                        
                        if (isJson && success == "True" && fail == "False" && code == "000000")
                        {
                            eachStationItem.合格 = "True";
                        }
                        else
                        {
                            eachStationItem.合格 = "False";
                        }

                        TraceContext.UpdateParam("step", 0);
                        return (true, null);

                    case 3:
                        Station1 eachStationItem2 = eachStation.Items[TraceContext.GetParam("curIndex") - 1];
                        TimeSpan t2 = DateTime.Now - TraceContext.GetParam("start");
                        eachStationItem2.CT = t2.Seconds.ToString();
                        eachStationItem2.合格 = "False";
                        return (true, null);
                    default:
                        TraceContext.UpdateParam("step", 0);
                        return (false, "流程步走到default中");
                }
            }
            catch (Exception e)
            {
                return (false, $"扫码工位Main函数中出现异常: {e}");
            }
        }
    }
}