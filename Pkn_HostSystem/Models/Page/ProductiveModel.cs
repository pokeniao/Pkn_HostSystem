using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using Pkn_HostSystem.Models.Core;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.Models.Page
{
    public partial class ProductiveModel:ObservableObject
    {
        /// <summary>
        /// 生产者集合
        /// </summary>
        [ObservableProperty]private ObservableCollection<Productive> productives;
        /// <summary>
        /// 生产者列表
        /// </summary>
        [ObservableProperty] private ObservableCollectionExtended<NetWork> producerList;
        /// <summary>
        /// 消费者集合
        /// </summary>
        [ObservableProperty] private ObservableCollectionExtended<NetWork> consumerList;
        /// <summary>
        /// 显示当前选中的设置一行生产者消费者的详细信息
        /// </summary>
        [ObservableProperty] private ObservableCollection<ProductiveDetailed> showDgDetailed =new ObservableCollection<ProductiveDetailed>();


    }
}