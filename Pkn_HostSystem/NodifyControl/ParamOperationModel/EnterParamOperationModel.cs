using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Models.Core;
using System.Diagnostics;

namespace Pkn_HostSystem.NodifyControl.ParamOperationModel
{
    public partial class EnterParamOperationModel : ObservableObject, IOperationModel
    {



        [ObservableProperty] private NetWorkTriggerModel netWorkTriggerModel  = new NetWorkTriggerModel();




    }
}