using Pkn_HostSystem.Base.Log.Interface;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.Service.LoadMes.Interface;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.Models.Core.Interface
{
    public interface IEachStation
    {
        string Header { get; set; }

        object Station { get; set; }

        Func<ILoadMesService, ILoadMesService> CreateDecoratorFunc { get; set; }

        ObservableCollection<object> Items { get; set; }

        ILogControl UserLog { get; set; }

        ILogControl ErrorLog { get; set; }

        ILogControl DevLog { get; set; }
    }
}