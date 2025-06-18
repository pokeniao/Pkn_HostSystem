using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Pkn_HostSystem.Static;

namespace Pkn_HostSystem.Models.Page;

public partial class SettingsPageModel : ObservableObject
{
    /// <summary>
    /// 当前软件的版本
    /// </summary>
    [JsonIgnore] public string AssemblyVersion { get; set; } = "当前版本: "+ GlobalMannager.AssemblyVersion;

    [ObservableProperty]
    private string currentTheme = "系统主题";

    [ObservableProperty] private bool isSelfStart = true;
}