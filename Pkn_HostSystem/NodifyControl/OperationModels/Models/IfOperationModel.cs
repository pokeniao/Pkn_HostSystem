using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.NodifyControl.OperationModels.Interface;

namespace Pkn_HostSystem.NodifyControl.OperationModels.Models
{
    public partial class IfOperationModel : ObservableObject, IOperationModel
    {
        [ObservableProperty] private string expression;
    }
}