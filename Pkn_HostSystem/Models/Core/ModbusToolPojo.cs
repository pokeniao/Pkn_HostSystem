namespace Pkn_HostSystem.Models.Core;

/// <summary>
/// ModbusToolPojo
/// </summary>
/// <typeparam name="A"></typeparam>
public class ModbusToolPojo<A>
{
    /// <summary>
    /// 起始地址
    /// </summary>
    public int address { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public A value { get; set; }
    /// <summary>
    /// 用于判断当前值是否是布尔类型
    /// </summary>
    public bool valueIsBool { get; set; }
}