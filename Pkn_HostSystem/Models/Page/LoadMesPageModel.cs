using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using Newtonsoft.Json;
using Pkn_HostSystem.Models.Windows;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.Models.Page;

public partial class LoadMesPageModel : ObservableObject
{
    /// <summary>
    /// HTTP发送的列表
    /// </summary>
    [ObservableProperty]
    private ObservableCollectionExtended<LoadMesAddAndUpdateWindowModel> mesPojoList;

}