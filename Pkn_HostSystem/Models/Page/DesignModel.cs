using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Pkn_HostSystem.NodifyControl.Editor;
using Pkn_HostSystem.NodifyControl.LocalSave;

namespace Pkn_HostSystem.Models.Page
{
    public partial class DesignModel : ObservableObject
    {


        [ObservableProperty] private string projectName;

        [JsonIgnore] public EditorViewModel EditorViewModel { get; set; } = new EditorViewModel();

        public LocalSaveNodify LocalSaveNodify { get; set; } = new LocalSaveNodify();


    }
}