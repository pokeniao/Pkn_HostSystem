namespace Pkn_HostSystem.Pojo.Page.ModbusTool;

/// <summary>
/// ModbusPojo
/// </summary>
/// <typeparam name="A"></typeparam>
public class ModbusPojo<A>
{
    public int address { get; set; }
    public A value { get; set; }

    public bool valueIsBool { get; set; }
}