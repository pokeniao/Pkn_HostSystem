using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Static;

namespace Pkn_HostSystem.Models.Page;

public partial class SettingsPageModel : ObservableObject
{
    public string AssemblyVersion { get; set; } = GlobalMannager.AssemblyVersion;
}