namespace Pkn_HostSystem.NodifyControl.LocalSave
{
    public class LocalSaveNodify
    {
        public List<LocalSaveNode> Nodes { get; set; } = new List<LocalSaveNode>();

        public List<LocalSaveConnection> Connections { get; set; } = new List<LocalSaveConnection>();
    }
}