using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Base.Log.Interface;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.Models.Core
{
    public interface IEachStation
    {
        string Header { get; set; }

        ObservableCollection<object> Items { get; set; }

        ILogControl UserLog { get; set; }

        ILogControl ErrorLog { get; set; }

        ILogControl DevLog { get; set; }
    }
}