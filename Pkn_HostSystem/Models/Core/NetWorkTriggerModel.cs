using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using Newtonsoft.Json;
using System.Windows;

namespace Pkn_HostSystem.Models.Core
{
    public partial class NetWorkTriggerModel :ObservableObject
    {
        //通讯网络列表
        [JsonIgnore][ObservableProperty] private ObservableCollectionExtended<NetWork> netWorkList = new ObservableCollectionExtended<NetWork>();
        //触发类型
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(showTriggerSet))]
        private string triggerType = "循环触发";

        //触发时间
        [ObservableProperty] private int triggerTime = 300;

        //触发网络名称
        [ObservableProperty] private string networkName ;

        //站点地址
        [ObservableProperty] private string stationAddress;

        //起始地址
        [ObservableProperty] private string startAddress;

        //触发的信息
        [ObservableProperty] private string triggerMessage;

        //成功写回信息
        [ObservableProperty] private string successResponseMessage;

        //失败写回信息
        [ObservableProperty] private string failResponseMessage;

        //触发返回的信息
        [ObservableProperty] private string triggerReturnMessage;

        public Visibility showTriggerSet => TriggerType == "消息触发" ? Visibility.Visible : Visibility.Collapsed;

    }
}