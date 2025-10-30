using CommunityToolkit.Mvvm.ComponentModel;

namespace Pkn_HostSystem.Models.Pojo
{
    public partial class VOCPojo :ObservableObject
    {
        //测试时间设置显示
        [ObservableProperty] private double setValueTestTime;
        //触发阈值设置显示
        [ObservableProperty] private double setValueTriggerMax;

        //测试时间
        [ObservableProperty] private double testTime =0;
        //触发阈值
        [ObservableProperty] private double triggerMax =0;
    }
}