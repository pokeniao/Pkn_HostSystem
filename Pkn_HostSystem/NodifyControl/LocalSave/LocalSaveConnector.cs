using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.NodifyControl.Node.Connector;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.LocalSave
{
    public class LocalSaveConnector
    {
        public string Id { get; set; }

        public string NodeId { get; set; }

        public string ConnectorName { get; set; }

        public ConnectorTypeEnum ConnectorType { get; set; }

        public List<LocalSaveConnector> ValueObservers { get; set; }

        public Point Anchor { get; set; }
    }
}