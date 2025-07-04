using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Service.Stations.Interface;
using Pkn_HostSystem.Static;

namespace Pkn_HostSystem.Service.Stations
{
    public partial class Station1 :ObservableObject,IStation
    {
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

        private int step = 0;
        private DateTime start ;

        private int curIndex;
        /// <summary>
        /// 主入口
        /// </summary>
        /// <returns></returns>
        public async Task<(bool Succeed, object Return)> Main(string station,CancellationTokenSource cts)
        {
            var eachStation = GlobalManager.StationDictionary.Lookup(station).Value as EachStation<Station1>;
            step++;
           

            switch (step)
            {
                case 1:
                    start = DateTime.Now;
                    //解析JSON
                    JObject jObject = JObject.Parse(TraceContext.Param?.ToString());
                    Station1 station1 = new Station1();
                    station1.工单编码 = jObject["orderCode"].ToString();
                    station1.排程编码 = jObject["scheduleCode"].ToString();
                    station1.条码 = jObject["snNumber"].ToString();
                    eachStation.AddItem(station1);
                    curIndex = eachStation.Items.Count;
                    break;
                case 2:
                    Station1 eachStationItem = eachStation.Items[curIndex-1];
                    TimeSpan t = DateTime.Now - start;
                    eachStationItem.CT = t.Seconds.ToString();
                    //解析JSON
                    JObject jObject2 = JObject.Parse(TraceContext.Param?.ToString());
                    string success = jObject2["success"].ToString();
                    string fail = jObject2["fail"].ToString();
                    string code = jObject2["code"].ToString();
                    if (success == "true" && fail =="false" && code == "000000")
                    {
                        eachStationItem.合格 = "Y";
                    }
                    else
                    {
                        eachStationItem.合格 = "N";
                    }
                    step = 0;
                    break;
            }

            return (false, null);
        }
    }
}