using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using WPF_NET.Static;

namespace WPF_NET.Models;

public partial class SettingsPageModel : ObservableObject
{
    public string AssemblyVersion { get; set; } = GlobalMannager.AssemblyVersion;
}