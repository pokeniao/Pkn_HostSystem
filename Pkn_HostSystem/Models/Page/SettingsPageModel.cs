using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Pkn_HostSystem.Static;

namespace Pkn_HostSystem.Models.Page;

public partial class SettingsPageModel : ObservableObject
{
    /// <summary>
    /// 当前软件的版本
    /// </summary>
    [JsonIgnore] public string AssemblyVersion { get; set; } = "当前版本: "+ GlobalManager.AssemblyVersion;

    [ObservableProperty]
    private string currentTheme = "系统主题";

    /// <summary>
    /// 开机自启动
    /// </summary>
    [ObservableProperty] private bool isSelfStart = true;
    /// <summary>
    /// 关闭时保存
    /// </summary>
    [ObservableProperty] private bool offSave = false;
}