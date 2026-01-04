using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.OperationModels.Interface;

namespace Pkn_HostSystem.NodifyControl.OperationModels.Models
{
    public partial class Rs232OperationModel : ObservableObject, IOperationModel
    {
        [ObservableProperty] private Rs232Model rs232Model = new Rs232Model();

    }
}