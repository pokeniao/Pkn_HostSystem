using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Models.Core;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace Pkn_HostSystem.Models.Page
{
    public partial class DesignModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<TreeNodes> nodes;
    }
}