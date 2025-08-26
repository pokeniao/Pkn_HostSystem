using Pkn_HostSystem.Base.Enum;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace Pkn_HostSystem.Models.Core
{
    public class TreeNodes
    {
        public string Name { get; set; }

        public NodeEnum NodeType { get; set; }
        public ObservableCollection<TreeNodes> Children { get; set; } = new();

        public bool IsLeaf => Children == null || Children.Count == 0;
    }
}