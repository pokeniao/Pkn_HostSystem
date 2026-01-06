using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.NodifyControl.OperationModels.Interface;

namespace Pkn_HostSystem.NodifyControl.OperationModels.Models
{
    public partial class SwitchOperationModel : ObservableObject, IOperationModel
    {

        [ObservableProperty] private int switchCount = 1;
    }
}