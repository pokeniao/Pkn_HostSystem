using CommunityToolkit.Mvvm.ComponentModel;

namespace Pkn_HostSystem.Models.Page
{
    public partial class UserLoginModel:ObservableObject
    {
        [ObservableProperty] private string name;

        [ObservableProperty] private string id;

        [ObservableProperty] private string emp;
    }
}