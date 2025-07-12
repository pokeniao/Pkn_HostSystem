using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Service.Stations.Interface;

namespace Pkn_HostSystem.Service.Stations
{
    public partial class Station2 :ObservableObject, IStation
    {
        public string 条码 { get; set; }

        public string 合格 { get; set; }
        public string 四螺丝A圈数 { get; set; }
        public string 四螺丝A扭力 { get; set; }
        public string 四螺丝A位移 { get; set; }
        public string 四螺丝B圈数 { get; set; }
        public string 四螺丝B扭力 { get; set; }
        public string 四螺丝B位移 { get; set; }
        public string 四螺丝C圈数 { get; set; }
        public string 四螺丝C扭力 { get; set; }
        public string 四螺丝C位移 { get; set; }
        public string 五螺丝D圈数 { get; set; }
        public string 五螺丝D扭力 { get; set; }
        public string 五螺丝D位移 { get; set; }

        public string 解锁电阻1 { get; set; }
        public string 闭锁电阻1 { get; set; }
        public string 压入力值 { get; set; }
        public string 拉出力值 { get; set; }
        public string 电压值 { get; set; }



        public string CT { get; set; }

        public string 参数2 { get; set; }


        /// <summary>
        /// 主入口
        /// </summary>
        /// <returns></returns>
        public async Task<(bool succeed, string message)> Main(CancellationTokenSource cts)
        {
            return (false,null);
        }
    }
}