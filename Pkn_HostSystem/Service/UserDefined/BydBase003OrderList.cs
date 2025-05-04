using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Models.Pojo;

namespace Pkn_HostSystem.Service.UserDefined
{
    public class PppBase003OrderList
    {
        public PppOrderList PppOrderList { get; set; }


        public PppBase003OrderList()
        {
            //从IOC容器中获取
            PppOrderList = Ioc.Default.GetRequiredService<PppOrderList>();
            //获取不到在Ioc容器中创建
            if (PppOrderList == null)
            {
                Ioc.Default.ConfigureServices(
                    new ServiceCollection().AddSingleton<PppOrderList>().BuildServiceProvider()
                );
                PppOrderList = Ioc.Default.GetRequiredService<PppOrderList>();
            }
        }

    }
}