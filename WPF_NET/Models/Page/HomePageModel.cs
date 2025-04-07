using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using WPF_NET.Pojo;

namespace WPF_NET.Models;

public partial class HomePageModel : ObservableObject
{
    
    [ObservableProperty] private ObservableCollection<string> logListBox;

    [ObservableProperty] private ObservableCollection<ConnectPojo> setConnectDg;

    [ObservableProperty] private string currentSetName;

}