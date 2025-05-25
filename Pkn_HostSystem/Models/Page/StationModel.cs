using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Documents;

namespace Pkn_HostSystem.Models.Page
{
    public partial class StationModel :ObservableObject
    {
        public  string Header { get; set; } //Tab标题

        public ObservableCollection<object> Items { get; set; }  // DataGrid数据

        [ObservableProperty] private FlowDocument logDocument;

    }
}