using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.ViewModels.Page;

namespace Pkn_HostSystem.NodifyControl.LocalSave
{
    public class LocalSaveNodifyMethod
    {
        public void Save()
        {
            DesignViewModel designViewModel = Ioc.Default.GetRequiredService<DesignViewModel>();
           
        }


        public void Load()
        {

        }
    }
}