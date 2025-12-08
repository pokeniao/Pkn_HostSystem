using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.OperationModels.Interface;

namespace Pkn_HostSystem.NodifyControl.OperationModels.Models
{
    public partial class ModbusTcpOperationModel : ObservableObject , IOperationModel
    {

        [ObservableProperty] private NetWorkTriggerModel netWorkTriggerModel = new NetWorkTriggerModel();

    }
}