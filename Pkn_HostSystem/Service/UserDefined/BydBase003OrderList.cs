using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Models.Pojo;

namespace Pkn_HostSystem.Service.UserDefined
{
    public class BydBase003OrderList
    {
        public BydOrderList BydOrderList { get; set; }


        public BydBase003OrderList()
        {
            //从IOC容器中获取
            BydOrderList = Ioc.Default.GetRequiredService<BydOrderList>();
            //获取不到在Ioc容器中创建
            if (BydOrderList == null)
            {
                Ioc.Default.ConfigureServices(
                    new ServiceCollection().AddSingleton<BydOrderList>().BuildServiceProvider()
                );
                BydOrderList = Ioc.Default.GetRequiredService<BydOrderList>();
            }
        }

    }
}