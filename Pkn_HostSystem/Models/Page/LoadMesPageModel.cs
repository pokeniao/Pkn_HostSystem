using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Models.Windows;

namespace Pkn_HostSystem.Models.Page;

public partial class LoadMesPageModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<LoadMesAddAndUpdateWindowModel> mesPojoList;

    [ObservableProperty] private ObservableCollection<string> returnMessageList;
}