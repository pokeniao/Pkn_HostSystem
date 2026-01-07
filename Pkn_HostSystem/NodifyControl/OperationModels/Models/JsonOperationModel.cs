using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.NodifyControl.OperationModels.Interface;

namespace Pkn_HostSystem.NodifyControl.OperationModels.Models
{

    public partial class JsonOperationModel : ObservableObject, IOperationModel
    {
        [ObservableProperty]
        private string jsonMethod ="路径解析";


        public string OldJsonMethod { get; set; }
    }
}