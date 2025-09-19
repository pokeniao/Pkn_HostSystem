using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.Editor;
using Pkn_HostSystem.NodifyControl.LocalSave;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace Pkn_HostSystem.Models.Page
{
    public partial class DesignModel : ObservableObject
    {
  

        [ObservableProperty] private string projectName;

        [JsonIgnore]
        public EditorViewModel EditorViewModel { get; set; }

        public LocalSaveNodify LocalSaveNodify { get; set; }


    }
}