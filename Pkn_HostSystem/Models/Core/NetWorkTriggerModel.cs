using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using Newtonsoft.Json;
using Pkn_HostSystem.NodifyControl.OperationModels.Models.Core;
using Pkn_HostSystem.Static;
using System.Collections.ObjectModel;
using System.Windows;

namespace Pkn_HostSystem.Models.Core
{
    public partial class NetWorkTriggerModel :ObservableObject
    {
        //通讯网络列表
        [JsonIgnore][ObservableProperty] private ObservableCollectionExtended<NetWork> netWorkList = new();
        //触发类型
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(showTriggerSet))]
        private string triggerType = "循环触发";

        //触发时间
        [ObservableProperty] private int triggerTime = 300;




        //触发网络名称
        private string networkName;

        public string NetworkName
        {
            get => networkName;

            set
            {
                //当通讯断掉的时候,防止清除掉上次连接的连接名
                // if (value == null)
                // {
                //     return; 
                // }
             
                SetProperty(ref networkName, value);
                OnPropertyChanged(nameof(showModbusTriggerParam));
                OnPropertyChanged(nameof(showKeyenceTriggerParam));
                OnNetworkNameChanged(value);
            }
        }


        public void OnNetworkNameChanged(string value)
        {
            string? networkDetailedNetMethod = GlobalManager.GetNetWork(value)?.NetworkDetailed.NetMethod;
            if (networkDetailedNetMethod == null)
            {
                return;
            }


            NetMethod.Clear();
            switch (networkDetailedNetMethod)
            {
                case "ModbusTcp":
                    NetMethod.Add("01读线圈");
                    NetMethod.Add("02读输入状态");
                    NetMethod.Add("03读保持寄存器");
                    NetMethod.Add("04读输入寄存器");
                    NetMethod.Add("05写单线圈");
                    NetMethod.Add("06写单寄存器");
                    NetMethod.Add("0F写多线圈");
                    NetMethod.Add("10写多寄存器");
                    if (NetMethodName == null)
                    {
                        NetMethodName = "03读保持寄存器";
                    }
                    break;
                case "ModbusRtu":
                    NetMethod.Add("01读线圈");
                    NetMethod.Add("02读输入状态");
                    NetMethod.Add("03读保持寄存器");
                    NetMethod.Add("04读输入寄存器");
                    NetMethod.Add("05写单线圈");
                    NetMethod.Add("06写单寄存器");
                    NetMethod.Add("0F写多线圈");
                    NetMethod.Add("10写多寄存器");
                    if (NetMethodName == null)
                    {
                        NetMethodName = "03读保持寄存器";
                    }
                    break;
                case "基恩士上位链路通讯":
                    NetMethod.Add("读DM寄存器");
                    NetMethod.Add("读多DM寄存器");
                    NetMethod.Add("读R线圈");
                    NetMethod.Add("写DM寄存器");
                    NetMethod.Add("写R线圈");
                    NetMethod.Add("写字符串/写多DM");
                    if (NetMethodName == null)
                    {
                        NetMethodName = "读多DM寄存器";
                    }
                    break;
            }
        }



        //站点地址
        [ObservableProperty] private string stationAddress = "1";

        //起始地址
        [ObservableProperty] private string startAddress = "0";

        //触发的信息
        [ObservableProperty] private string triggerMessage = "1";

        //成功写回信息
        [ObservableProperty] private string successResponseMessage = "2";

        //失败写回信息
        [ObservableProperty] private string failResponseMessage = "3";
        //数量
        [ObservableProperty] private string count = "1";

        //方法s
        [ObservableProperty] private ObservableCollection<string> netMethod = new ObservableCollection<string>();

        //选中的方法名
        [ObservableProperty] private string netMethodName;

        
        [ObservableProperty] private ObservableCollection<OperationModel> readDvgList = new ();
        [ObservableProperty] private ObservableCollection<OperationModel> writeDvgList = new();

        //格式
        [ObservableProperty] private string format = "单寄存器(无符号)";
        //上一次的格式,用于决定是否需要更新
        public string LastFormat { get; set; }


        public Visibility showTriggerSet => TriggerType == "消息触发" ? Visibility.Visible : Visibility.Collapsed;

        public Visibility showModbusTriggerParam =>GlobalManager.GetNetWork(networkName)?.NetworkDetailed.NetMethod == "ModbusTcp" ||
            GlobalManager.GetNetWork(networkName)?.NetworkDetailed.NetMethod == "ModbusRtu" ? Visibility.Visible : Visibility.Collapsed;

        public Visibility showKeyenceTriggerParam => GlobalManager.GetNetWork(networkName)?.NetworkDetailed.NetMethod == "基恩士上位链路通讯" ? Visibility.Visible : Visibility.Collapsed;

    }
}