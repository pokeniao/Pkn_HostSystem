using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.Node
{
    public partial class PNode: ObservableObject
    {
        public string NodeName { get; set; }


        [ObservableProperty] private Point _location;

        public ObservableCollection<Connector.Connector> Input { get; set; } = new ObservableCollection<Connector.Connector>();
        public ObservableCollection<Connector.Connector> Output { get; set; } = new ObservableCollection<Connector.Connector>();
    }
}