using CommunityToolkit.Mvvm.ComponentModel;

namespace Pkn_HostSystem.Models.Core
{
    public partial class PlcAlarmItem : ObservableObject
    {
        //序号
        [ObservableProperty] private int id;
        //报警
        [ObservableProperty] private string alarm = "";
        // 
        public bool OldMemory { get; set; } = false;

        public PlcAlarmItem(int Index)
        {
            Id = Index;
        }
    }
}