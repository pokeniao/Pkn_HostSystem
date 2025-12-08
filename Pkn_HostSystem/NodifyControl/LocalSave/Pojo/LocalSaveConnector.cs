using Pkn_HostSystem.Base.Enum;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.LocalSave.Pojo
{
    public class LocalSaveConnector
    {
        public string Id { get; set; }

        public string NodeId { get; set; }

        public string ConnectorName { get; set; }

        public ConnectorTypeEnum ConnectorType { get; set; }

        public Point Anchor { get; set; }
    }
}