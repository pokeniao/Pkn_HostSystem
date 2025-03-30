namespace Frame.pojo
{
    public class ModbusPojo<A>
    {
        public int address { get; set; }
        public A value { get; set; }
    }
}
