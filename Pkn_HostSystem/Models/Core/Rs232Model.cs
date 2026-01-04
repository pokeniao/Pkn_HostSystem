using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using Newtonsoft.Json;

namespace Pkn_HostSystem.Models.Core
{
    public partial class Rs232Model : ObservableObject
    {
        //通讯网络列表
        [JsonIgnore][ObservableProperty] private ObservableCollectionExtended<NetWork> netWorkList = new();

        //触发网络名称
        private string networkName;

        public string NetworkName
        {
            get => networkName;

            set
            {
                SetProperty(ref networkName, value);
            }
        }

        /// <summary>
        /// 通讯的方式
        /// </summary>
        [ObservableProperty] private string netMethodName = "发送并等待读取";

        /// <summary>
        /// 发送的内容
        /// </summary>
        [ObservableProperty] private string sendMessage;
    }
}