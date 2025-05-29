using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using System.Collections.ObjectModel;
using System.Reflection.Emit;
using System.Windows.Documents;

namespace Pkn_HostSystem.Models.Page
{
    public partial class StationModel :ObservableObject
    {
        [ObservableProperty] private ObservableCollectionExtended<IEachStation> stations;
    }
}