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

        //设备编号
        [ObservableProperty] private string machineId;

        [ObservableProperty] private string setMachineId;

        //工序
        [ObservableProperty] private string groupCode;

        [ObservableProperty] private string setGroupCode;

        //上传mes启用
        [ObservableProperty] private bool mesOn =false;
    }
}