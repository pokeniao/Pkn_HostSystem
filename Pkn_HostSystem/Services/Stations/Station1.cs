using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Services.Stations.Interface;

namespace Pkn_HostSystem.Services.Stations
{
    public partial class Station1 : ObservableObject, IStation
    {
        //这样写的原因:[ObservableProperty]无法生成大写的中文
        private string _电芯条码1;

        public string 电芯条码1
        {
            get => _电芯条码1;
            set
            {
                SetProperty(ref _电芯条码1, value);
            }
        }


        private string _电芯条码2;
        public string 电芯条码2
        {
            get => _电芯条码2;
            set
            {
                SetProperty(ref _电芯条码2, value);
            }
        }

        private string _VOC最大值;
        public string VOC最大值
        {
            get => _VOC最大值;
            set
            {
                SetProperty(ref _VOC最大值, value);
            }
        }

        private string _腔体号;
        public string 腔体号
        {
            get => _腔体号;
            set
            {
                SetProperty(ref _腔体号, value);
            }
        }


        private string _正压值;
        public string 正压值
        {
            get => _正压值;
            set
            {
                SetProperty(ref _正压值, value);
            }
        }

        private string _负压值;
        public string 负压值
        {
            get => _负压值;
            set
            {
                SetProperty(ref _负压值, value);
            }
        }

        private string _结果;
        public string 结果
        {
            get => _结果;
            set
            {
                SetProperty(ref _结果, value);
            }
        }

        private string mes上传;
        public string Mes上传
        {
            get => mes上传;
            set
            {
                SetProperty(ref mes上传, value);
            }
        }

        private string voc_1s;
        public string Voc_1s
        {
            get => voc_1s;
            set
            {
                SetProperty(ref voc_1s, value);
            }
        }

        private string voc_2s;
        public string Voc_2s
        {
            get => voc_2s;
            set
            {
                SetProperty(ref voc_2s, value);
            }
        }

        private string voc_3s;
        public string Voc_3s
        {
            get => voc_3s;
            set
            {
                SetProperty(ref voc_3s, value);
            }
        }

        private string voc_4s;
        public string Voc_4s
        {
            get => voc_4s;
            set
            {
                SetProperty(ref voc_4s, value);
            }
        }

        private string voc_5s;
        public string Voc_5s
        {
            get => voc_5s;
            set
            {
                SetProperty(ref voc_5s, value);
            }
        }

        private string voc_6s;
        public string Voc_6s
        {
            get => voc_6s;
            set
            {
                SetProperty(ref voc_6s, value);
            }
        }
        private string voc_7s;
        public string Voc_7s
        {
            get => voc_7s;
            set
            {
                SetProperty(ref voc_7s, value);
            }
        }
        private string voc_8s;
        public string Voc_8s
        {
            get => voc_8s;
            set
            {
                SetProperty(ref voc_8s, value);
            }
        }

        private string voc_9s;
        public string Voc_9s
        {
            get => voc_9s;
            set
            {
                SetProperty(ref voc_9s, value);
            }
        }
        private string voc_10s;
        public string Voc_10s
        {
            get => voc_10s;
            set
            {
                SetProperty(ref voc_10s, value);
            }
        }
        private string voc_11s;
        public string Voc_11s
        {
            get => voc_11s;
            set
            {
                SetProperty(ref voc_11s, value);
            }
        }
        private string voc_12s;
        public string Voc_12s
        {
            get => voc_12s;
            set
            {
                SetProperty(ref voc_12s, value);
            }
        }
        private string voc_13s;
        public string Voc_13s
        {
            get => voc_13s;
            set
            {
                SetProperty(ref voc_13s, value);
            }
        }
        private string voc_14s;
        public string Voc_14s
        {
            get => voc_14s;
            set
            {
                SetProperty(ref voc_14s, value);
            }
        }
        private string voc_15s;
        public string Voc_15s
        {
            get => voc_15s;
            set
            {
                SetProperty(ref voc_15s, value);
            }
        }
        private string voc_16s;
        public string Voc_16s
        {
            get => voc_16s;
            set
            {
                SetProperty(ref voc_16s, value);
            }
        }
        private string voc_17s;
        public string Voc_17s
        {
            get => voc_17s;
            set
            {
                SetProperty(ref voc_17s, value);
            }
        }
        private string voc_18s;
        public string Voc_18s
        {
            get => voc_18s;
            set
            {
                SetProperty(ref voc_18s, value);
            }
        }
        private string voc_19s;
        public string Voc_19s
        {
            get => voc_19s;
            set
            {
                SetProperty(ref voc_19s, value);
            }
        }
        private string voc_20s;
        public string Voc_20s
        {
            get => voc_20s;
            set
            {
                SetProperty(ref voc_20s, value);
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
                //从TraceContext中获取参数
                var eachStation = TraceContext.GetParam("EachStation");

                if (TraceContext.GetParam("step") == null || TraceContext.GetParam("step") == 0)
                {
                    TraceContext.UpdateParam("step", 1);
                }
                int step = TraceContext.GetParam("step");

                switch (step)
                {
                    //第一步
                    case 1:

                        
                        // TraceContext.UpdateParam("start", DateTime.Now);
                        // Station1 station1 = new Station1();
                        // station1.时间 = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                        //
                        // station1.电阻上限 = StaticArrayRegister.ReadRegisterValue(50).ToString();
                        // station1.电阻下限 = StaticArrayRegister.ReadRegisterValue(51).ToString();
                        // station1.电压上限 = StaticArrayRegister.ReadRegisterValue(52).ToString();
                        // station1.电压下限 = StaticArrayRegister.ReadRegisterValue(53).ToString();
                        // //添加一行数据到显示
                        // eachStation.AddItem(station1);
                        // //当前添加的是第几行
                        // TraceContext.UpdateParam("curIndex", eachStation.Items.Count);
                        // //流程跳转
                        // TraceContext.UpdateParam("step", 2);
                        // return (true, null);
                    //第二步
                    case 2:
                        // //获取到当前的一行数据
                        // Station1 eachStationItem = eachStation.Items[TraceContext.GetParam("curIndex") - 1];
                        // //计算CT
                        // TimeSpan t = DateTime.Now - TraceContext.GetParam("start");
                        // //填入参数
                        // eachStationItem.CT = t.Seconds.ToString();
                        // eachStationItem.电阻值 = StaticArrayRegister.ReadRegisterValue(54).ToString();
                        // eachStationItem.电压值 = StaticArrayRegister.ReadRegisterValue(55).ToString();
                        // eachStationItem.合格 = StaticArrayRegister.ReadRegisterValue(56).ToString();
                        // TraceContext.UpdateParam("step", 0);
                        // return (true, null);
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