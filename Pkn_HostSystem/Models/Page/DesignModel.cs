using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.Editor;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace Pkn_HostSystem.Models.Page
{
    public partial class DesignModel : ObservableObject
    {
  

        [ObservableProperty] private string projectName;

        public EditorViewModel EditorViewModel { get; set; } = new EditorViewModel();


    }
}