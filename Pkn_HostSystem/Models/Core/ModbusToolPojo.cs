using CommunityToolkit.Mvvm.ComponentModel;

namespace Pkn_HostSystem.Models.Core;

/// <summary>
/// ModbusToolPojo
/// </summary>
/// <typeparam name="A"></typeparam>
public partial class ModbusToolPojo<T> :ObservableObject
{
    /// <summary>
    /// 起始地址
    /// </summary>
    [ObservableProperty] private  int address;

    /// <summary>
    /// 
    /// </summary>
    [ObservableProperty] private T value;
    /// <summary>
    /// 用于判断当前值是否是布尔类型
    /// </summary>
    public bool valueIsBool { get; set; }
}