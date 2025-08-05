using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using Pkn_HostSystem.Static;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.Models.Page
{
    public partial class LiveChartsModel :ObservableObject
    {

        //良率Ok饼图
       [ObservableProperty] private ObservableValue ok = new ObservableValue(1);
       [ObservableProperty] private ObservableValue ng = new ObservableValue(1);
       [ObservableProperty] private ObservableValue runTime = new ObservableValue(1);
       [ObservableProperty] private ObservableValue stopTime = new ObservableValue(1); //一天一共86400秒
       [ObservableProperty] private ObservableValue errorTime = new ObservableValue(1);
        
        #region 七日产量

        //OK数量
        private ObservableCollection<ObservableValue> oks = new ObservableCollection<ObservableValue>()
        {
            new ObservableValue(0),//1
            new ObservableValue(0),//2
            new ObservableValue(0),//3
            new ObservableValue(0),//4
            new ObservableValue(0),//5
            new ObservableValue(0),//6
            new ObservableValue(0),//7
            new ObservableValue(0),//8
            new ObservableValue(0),//9
            new ObservableValue(0),//10
            new ObservableValue(0),//11
            new ObservableValue(0),//12
            new ObservableValue(0),//13
            new ObservableValue(0),//14
            new ObservableValue(0),//15
            new ObservableValue(0),//16
            new ObservableValue(0),//17
            new ObservableValue(0),//18
            new ObservableValue(0),//19
            new ObservableValue(0),//20
            new ObservableValue(0),//21
            new ObservableValue(0),//22
            new ObservableValue(0),//23
            new ObservableValue(0),//24
        };



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
            new ObservableValue(0),//1
            new ObservableValue(0),//2
            new ObservableValue(0),//3
            new ObservableValue(0),//4
            new ObservableValue(0),//5
            new ObservableValue(0),//6
            new ObservableValue(0),//7
            new ObservableValue(0),//8
            new ObservableValue(0),//9
            new ObservableValue(0),//10
            new ObservableValue(0),//11
            new ObservableValue(0),//12
            new ObservableValue(0),//13
            new ObservableValue(0),//14
            new ObservableValue(0),//15
            new ObservableValue(0),//16
            new ObservableValue(0),//17
            new ObservableValue(0),//18
            new ObservableValue(0),//19
            new ObservableValue(0),//20
            new ObservableValue(0),//21
            new ObservableValue(0),//22
            new ObservableValue(0),//23
            new ObservableValue(0),//24
        };

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
            new ObservableValue(0),//1
            new ObservableValue(0),//2
            new ObservableValue(0),//3
            new ObservableValue(0),//4
            new ObservableValue(0),//5
            new ObservableValue(0),//6
            new ObservableValue(0),//7
            new ObservableValue(0),//8
            new ObservableValue(0),//9
            new ObservableValue(0),//10
            new ObservableValue(0),//11
            new ObservableValue(0),//12
            new ObservableValue(0),//13
            new ObservableValue(0),//14
            new ObservableValue(0),//15
            new ObservableValue(0),//16
            new ObservableValue(0),//17
            new ObservableValue(0),//18
            new ObservableValue(0),//19
            new ObservableValue(0),//20
            new ObservableValue(0),//21
            new ObservableValue(0),//22
            new ObservableValue(0),//23
            new ObservableValue(0),//24
        };

        public ObservableCollection<ObservableValue> All
        {
            get => all;
            set
            {
                SetProperty(ref all, value);
            }
        }


        #endregion



    }
}