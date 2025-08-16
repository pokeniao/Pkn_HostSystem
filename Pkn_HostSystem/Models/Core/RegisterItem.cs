using System.Collections.ObjectModel;

namespace Pkn_HostSystem.Models.Core
{
    public class RegisterItem
    {
        public string Name { get; set; }

        public string Index { get; set; }

        public ObservableCollection<string> Value { get; set; }

    }
}