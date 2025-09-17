using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using Pkn_HostSystem.Models.Page;

namespace Pkn_HostSystem.Models.Core
{
    public partial class ProjectModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollectionExtended<DesignModel> projectList = new ObservableCollectionExtended<DesignModel>();

    }
}