using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace WPF_NET.Models;

public partial class LoadMesPageModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<LoadMesAddAndUpdateWindowModel> mesPojoList;
}