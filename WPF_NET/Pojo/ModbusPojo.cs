namespace WPF_NET.Pojo;

public class ModbusPojo<A>
{
    public int address { get; set; }
    public A value { get; set; }

    public bool valueIsBool { get; set; }
}