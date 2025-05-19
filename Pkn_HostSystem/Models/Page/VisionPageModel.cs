using CommunityToolkit.Mvvm.ComponentModel;

namespace Pkn_HostSystem.Models.Page
{
    public partial class VisionPageModel:ObservableObject
    {

        [ObservableProperty] private string cameraName;

        [ObservableProperty]
        private string realTimeName = "实时";
    }
}