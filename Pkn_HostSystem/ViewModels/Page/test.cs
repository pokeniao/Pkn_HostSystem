using S7.Net;

namespace Pkn_HostSystem.ViewModels.Page
{
    public class test
    {
        public void test1()
        {
            //机架号 , 槽号
            new Plc(CpuType.S71200, "192.168.0.1", 0, 1);
        }
    }
}