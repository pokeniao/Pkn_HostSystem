using DynamicData.Binding;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.Models.Page
{
    public class MenuSelectModel
    {
        public ObservableCollectionExtended<DesignModel> ProjectList { get; set; }
    }
}