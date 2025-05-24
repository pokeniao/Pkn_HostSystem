using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Models.Core;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.Models.Page
{
    public partial class VisionPageModel:ObservableObject
    {

        [ObservableProperty] private ObservableCollection<CameraDetailed> cameraList;

        [ObservableProperty]
        private string realTimeName = "实时";

        [ObservableProperty] private string controlCameraGim1;
        [ObservableProperty] private string controlCameraGim2;
        [ObservableProperty] private string controlCameraGim3;
        [ObservableProperty] private string controlCameraGim4;
    }
}