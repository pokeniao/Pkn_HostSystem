using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
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

        private string _型号;

        public string 型号
        {
            get => _型号;
            set
            {
                SetProperty(ref _型号, value);
            }
        }

        private string _批号;


        public string 批号
        {
            get => _批号;
            set
            {
                SetProperty(ref _批号, value);
            }
        }


        private string _电阻上限;

        public string 电阻上限
        {
            get => _电阻上限;
            set
            {
                SetProperty(ref _电阻上限, value);
            }
        }

        private string _电阻值;

        public string 电阻值
        {
            get => _电阻值;
            set
            {
                SetProperty(ref _电阻值, value);
            }
        }

        private string _电阻下限;

        public string 电阻下限
        {
            get => _电阻下限;
            set
            {
                SetProperty(ref _电阻下限, value);
            }
        }


        private string _电压上限;

        public string 电压上限
        {
            get => _电压上限;
            set
            {
                SetProperty(ref _电压上限, value);
            }
        }

        private string _电压值;

        public string 电压值
        {
            get => _电压值;
            set
            {
                SetProperty(ref _电压值, value);
            }
        }

        private string _电压下限;

        public string 电压下限
        {
            get => _电压下限;
            set
            {
                SetProperty(ref _电压下限, value);
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
        /// 50 电阻上线
        /// 51 电阻下线
        /// 52 电压上线
        /// 53 电压下线
        /// 54 电阻值
        /// 55 电压值
        /// 56 合格 NG/OK
        /// 57 条码
        /// 58 型号
        /// 59 批号
        /// </summary>
        /// <returns></returns>
        public async Task<(bool succeed, string message)> Main(CancellationTokenSource cts)
        {
            try
            {
                //从TraceContext中获取参数
                var eachStation = TraceContext.GetParam("EachStation");

                if ( TraceContext.GetParam("step") == null || TraceContext.GetParam("step") ==0)
                {
                    TraceContext.UpdateParam("step", 1);
                }
                int step = TraceContext.GetParam("step");

                switch (step)
                {
                    //第一步
                    case 1:

                        TraceContext.UpdateParam("start", DateTime.Now);
                        Station1 station1 = new Station1();
                        station1.时间 = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                        station1.电阻上限 = Volatile.Read(ref GlobalManager.ArrayRegister[50]).ToString();
                        station1.电阻下限 = Volatile.Read(ref GlobalManager.ArrayRegister[51]).ToString();
                        station1.电压上限 = Volatile.Read(ref GlobalManager.ArrayRegister[52]).ToString();
                        station1.电压下限 = Volatile.Read(ref GlobalManager.ArrayRegister[53]).ToString();
                      
                        //添加一行数据到显示
                        eachStation.AddItem(station1);
                        //当前添加的是第几行
                        TraceContext.UpdateParam("curIndex", eachStation.Items.Count);
                        //流程跳转
                        TraceContext.UpdateParam("step", 2);
                        return (true, null);
                    //第二步
                    case 2:
                        //获取到当前的一行数据
                        Station1 eachStationItem = eachStation.Items[TraceContext.GetParam("curIndex") - 1];
                        //计算CT
                        TimeSpan t = DateTime.Now - TraceContext.GetParam("start");
                        //填入参数
                        eachStationItem.条码 = Volatile.Read(ref GlobalManager.ArrayRegister[57]).ToString();

                        try
                        {
                            if (!string.IsNullOrEmpty(eachStationItem.条码))
                            {
                                //型号
                                string substring = eachStationItem.条码?.Substring(0,2);
                                Volatile.Write(ref GlobalManager.ArrayRegister[58] , substring);

                                //批号
                                string substring2 = eachStationItem.条码?.Substring(2, 4);
                                Volatile.Write(ref GlobalManager.ArrayRegister[59], substring2);
                            }
                        }
                        catch (Exception e)
                        {
                            eachStationItem.CT = t.Seconds.ToString();
                            eachStationItem.电阻值 = Volatile.Read(ref GlobalManager.ArrayRegister[54]).ToString();
                            eachStationItem.电压值 = Volatile.Read(ref GlobalManager.ArrayRegister[55]).ToString();
                            eachStationItem.合格 = "NG";
                            TraceContext.UpdateParam("step", 0);
                            throw ;
                        }

                        

                        eachStationItem.CT = t.Seconds.ToString();
                        eachStationItem.电阻值 = Volatile.Read(ref GlobalManager.ArrayRegister[54]).ToString();
                        eachStationItem.电压值 = Volatile.Read(ref GlobalManager.ArrayRegister[55]).ToString();
                        eachStationItem.合格 = Volatile.Read(ref GlobalManager.ArrayRegister[56]).ToString();
                        eachStationItem.型号 = Volatile.Read(ref GlobalManager.ArrayRegister[58]).ToString();
                        eachStationItem.批号 = Volatile.Read(ref GlobalManager.ArrayRegister[59]).ToString();
                        TraceContext.UpdateParam("step", 0);
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