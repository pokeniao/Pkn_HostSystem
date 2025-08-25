using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.NodifyControl.Node.Connector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.Node
{
    public partial class MyNode: ObservableObject
    {
        public string NodeName { get; set; }


        [ObservableProperty] private Point _location;

        public ObservableCollection<MyConnector> Input { get; set; } = new ObservableCollection<MyConnector>();
        public ObservableCollection<MyConnector> Output { get; set; } = new ObservableCollection<MyConnector>();
    }
}