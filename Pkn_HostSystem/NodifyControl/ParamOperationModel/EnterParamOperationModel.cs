using CommunityToolkit.Mvvm.ComponentModel;

namespace Pkn_HostSystem.NodifyControl.ParamOperationModel
{
    public partial class EnterParamOperationModel :ObservableObject
    {
        //触发类型
        [ObservableProperty] private string triggerType = "循环触发";

        //触发时间
        [ObservableProperty] private int triggerTime = 300;

    }
}