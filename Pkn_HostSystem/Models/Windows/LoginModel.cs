using CommunityToolkit.Mvvm.ComponentModel;

namespace Pkn_HostSystem.Models.Windows
{
    public partial class LoginModel :ObservableObject
    {
        [ObservableProperty] private string machineId;
        [ObservableProperty] private string mi;
        [ObservableProperty] private string userNumber;
        [ObservableProperty] private string passWord;
    }
}