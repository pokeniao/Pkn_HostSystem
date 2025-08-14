using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.Defaults;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.Models.Page
{
    public partial class LiveChartsModel : ObservableObject
    {
        #region 不需要保存

        #region 良率饼图

        private ObservableValue ok = new ObservableValue(1);

        [JsonIgnore]
        public ObservableValue Ok
        {
            get => ok;
            set
            {
                SetProperty(ref ok, value);
            }
        }


        private ObservableValue ng = new ObservableValue(1);

        [JsonIgnore]
        public ObservableValue Ng
        {
            get => ng;
            set
            {
                SetProperty(ref ng, value);
            }
        }

        #endregion

        //良率Ok饼图

        #region 停机运行时间

        private ObservableValue runTime = new ObservableValue(1);

        [JsonIgnore]
        public ObservableValue RunTime
        {
            get => runTime;
            set
            {
                SetProperty(ref runTime, value);
            }
        }

        private ObservableValue stopTime = new ObservableValue(1); //一天一共86400秒

        [JsonIgnore]
        public ObservableValue StopTime
        {
            get => stopTime;
            set
            {
                SetProperty(ref stopTime, value);
            }
        }

        private ObservableValue errorTime = new ObservableValue(1);

        [JsonIgnore]
        public ObservableValue ErrorTime
        {
            get => errorTime;
            set
            {
                SetProperty(ref errorTime, value);
            }
        }

        #endregion

        #region 七日产量

        //OK数量
        private ObservableCollection<ObservableValue> oks = new ObservableCollection<ObservableValue>()
        {
            new ObservableValue(0), //1
            new ObservableValue(0), //2
            new ObservableValue(0), //3
            new ObservableValue(0), //4
            new ObservableValue(0), //5
            new ObservableValue(0), //6
            new ObservableValue(0), //7
            new ObservableValue(0), //8
            new ObservableValue(0), //9
            new ObservableValue(0), //10
            new ObservableValue(0), //11
            new ObservableValue(0), //12
            new ObservableValue(0), //13
            new ObservableValue(0), //14
            new ObservableValue(0), //15
            new ObservableValue(0), //16
            new ObservableValue(0), //17
            new ObservableValue(0), //18
            new ObservableValue(0), //19
            new ObservableValue(0), //20
            new ObservableValue(0), //21
            new ObservableValue(0), //22
            new ObservableValue(0), //23
            new ObservableValue(0), //24
        };

        [JsonIgnore]
        public ObservableCollection<ObservableValue> Oks
        {
            get => oks;
            set
            {
                SetProperty(ref oks, value);
            }
        }

        //NG数量
        private ObservableCollection<ObservableValue> ngs = new ObservableCollection<ObservableValue>()
        {
            new ObservableValue(0), //1
            new ObservableValue(0), //2
            new ObservableValue(0), //3
            new ObservableValue(0), //4
            new ObservableValue(0), //5
            new ObservableValue(0), //6
            new ObservableValue(0), //7
            new ObservableValue(0), //8
            new ObservableValue(0), //9
            new ObservableValue(0), //10
            new ObservableValue(0), //11
            new ObservableValue(0), //12
            new ObservableValue(0), //13
            new ObservableValue(0), //14
            new ObservableValue(0), //15
            new ObservableValue(0), //16
            new ObservableValue(0), //17
            new ObservableValue(0), //18
            new ObservableValue(0), //19
            new ObservableValue(0), //20
            new ObservableValue(0), //21
            new ObservableValue(0), //22
            new ObservableValue(0), //23
            new ObservableValue(0), //24
        };

        [JsonIgnore]
        public ObservableCollection<ObservableValue> Ngs
        {
            get => ngs;
            set
            {
                SetProperty(ref ngs, value);
            }
        }

        //全部
        private ObservableCollection<ObservableValue> all = new ObservableCollection<ObservableValue>()
        {
            new ObservableValue(0), //1
            new ObservableValue(0), //2
            new ObservableValue(0), //3
            new ObservableValue(0), //4
            new ObservableValue(0), //5
            new ObservableValue(0), //6
            new ObservableValue(0), //7
            new ObservableValue(0), //8
            new ObservableValue(0), //9
            new ObservableValue(0), //10
            new ObservableValue(0), //11
            new ObservableValue(0), //12
            new ObservableValue(0), //13
            new ObservableValue(0), //14
            new ObservableValue(0), //15
            new ObservableValue(0), //16
            new ObservableValue(0), //17
            new ObservableValue(0), //18
            new ObservableValue(0), //19
            new ObservableValue(0), //20
            new ObservableValue(0), //21
            new ObservableValue(0), //22
            new ObservableValue(0), //23
            new ObservableValue(0), //24
        };

        [JsonIgnore]
        public ObservableCollection<ObservableValue> All
        {
            get => all;
            set
            {
                SetProperty(ref all, value);
            }
        }

        #endregion

        #endregion


        #region 需要保存

        /// <summary>
        /// 产量统计,动态获取的名
        /// </summary>
        [ObservableProperty] private string dayProductionDynName;


        [ObservableProperty] private string oeeDynName;


        [ObservableProperty] private string runStopTimeDynName;

        /// <summary>
        /// 运行LiveCharts进行数据展示按钮
        /// </summary>
        [ObservableProperty] private string runLiveChartsButton = "启用";

        [ObservableProperty] private int timeCyc = 100;


        [ObservableProperty] private ObservableCollection<string> labelsXAxesOEEYield ;


        [ObservableProperty] private ObservableCollection<string> labelsXAxesDayTimeYield =
        [
            "0:00-1:00", "1:00-2:00", "2:00-3:00", "3:00-4:00", "4:00-5:00", "5:00-6:00", "6:00-7:00", "7:00-8:00",
            "8:00-9:00", "9:00-10:00", "10:00-11:00",
            "11:00-12:00", "12:00-13:00", "13:00-14:00", "14:00-15:00", "15:00-16:00", "16:00-17:00", "17:00-18:00",
            "18:00-19:00", "19:00-20:00", "20:00-21:00",
            "21:00-22:00", "22:00-23:00", "23:00-0:00"
        ];


 

        private ObservableCollection<ObservableValue> oees = new();
        [JsonIgnore]
        public ObservableCollection<ObservableValue> Oees
        {
            get => oees;
            set
            {
                SetProperty(ref oees, value);
            }

        }

        public List<double?> OeeSave { get; set; } = new();

        [ObservableProperty] private string xDayTimeMethod = "常量设置";
        [ObservableProperty] private string yDayTimeMethod = "数量坐标";

        [ObservableProperty] private string xOeeMethod = "随月份更新(保存31天)";
        [ObservableProperty] private string yOeeMethod = "数量坐标";

        /// <summary>
        /// 产量统计启动
        /// </summary>
        [ObservableProperty] private bool dateTimeRun = false;
        /// <summary>
        /// oee启动
        /// </summary>
        [ObservableProperty] private bool oeeRun = false;
        /// <summary>
        /// 运行停机报警启动
        /// </summary>
        [ObservableProperty] private bool waitAlarmRun = false;

        #endregion
    }
}