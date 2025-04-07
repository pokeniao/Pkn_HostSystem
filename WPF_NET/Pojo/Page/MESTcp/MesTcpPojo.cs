using System.Collections.ObjectModel;

namespace WPF_NET.Pojo.Page.MESTcp;

public class MesTcpPojo
{
    public string Name { get; set; }

    public ObservableCollection<DynConditionItem> DynCondition { get; set; }
}