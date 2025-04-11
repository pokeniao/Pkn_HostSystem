using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Pojo.Page.HomePage;

namespace Pkn_HostSystem.Models.Page;

public partial class HomePageModel : ObservableObject
{
    
    [ObservableProperty] private ObservableCollection<string> logListBox;

    [ObservableProperty] private ObservableCollection<ConnectPojo> setConnectDg;

    [ObservableProperty] private string currentSetName;

}