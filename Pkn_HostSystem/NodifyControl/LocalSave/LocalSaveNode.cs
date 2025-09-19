using Pkn_HostSystem.Models.Core;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.LocalSave
{
    public class LocalSaveNode
    {
        public string Id { get; set; }

        public TreeNodes TreeNodes { get; set; }

        public Point Location { get; set; }

        public List<LocalSaveConnector> Input { get; set; }
        public List<LocalSaveConnector> Output { get; set; }
    }
}