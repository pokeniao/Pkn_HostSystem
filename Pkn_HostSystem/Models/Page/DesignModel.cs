using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Models.Core;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace Pkn_HostSystem.Models.Page
{
    public partial class DesignModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<TreeNodes> nodes;

        [ObservableProperty] private string projectName;

        [ObservableProperty] private ObservableCollection<string> projectList = new ObservableCollection<string>();
    }
}