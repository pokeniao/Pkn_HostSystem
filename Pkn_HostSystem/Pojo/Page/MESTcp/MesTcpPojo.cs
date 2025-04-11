using System.Collections.ObjectModel;

namespace Pkn_HostSystem.Pojo.Page.MESTcp;

public class MesTcpPojo
{
    public string Name { get; set; }

    public ObservableCollection<DynConditionItem> DynCondition { get; set; }
    public string Message { get; set; }
}