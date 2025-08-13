using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Models.Core;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.Models.Page
{
    public partial class VisionPageModel:ObservableObject
    {
        [ObservableProperty] private string cameraShowMethod;

    }
}