using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WPF_NET.Pojo.Page.MESTcp;

public class MesTcpPojo
{
    public string Name { get; set; }

    public ObservableCollection<DynConditionItem> DynCondition { get; set; }
    public string Message { get; set; }
}