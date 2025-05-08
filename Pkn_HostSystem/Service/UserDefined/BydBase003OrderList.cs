using CommunityToolkit.Mvvm.DependencyInjection;
using DynamicData;
using DynamicData.Binding;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Models.Pojo;
using Pkn_HostSystem.Server.LoadMes;
using Pkn_HostSystem.ViewModels.Page;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.Service.UserDefined
{
    public class BydBase003OrderList
    {
        public BydOrderList BydOrderList { get; set; }
        public LogBase<BydBase003OrderList> Log = new LogBase<BydBase003OrderList>();

        public BydBase003OrderList()
        {
            //从IOC容器中获取
            HomePageViewModel homePageViewModel = Ioc.Default.GetRequiredService<HomePageViewModel>();
            BydOrderList = homePageViewModel.HomePageModel.CurrentSelectBydOrder;
        }

        /// <summary>
        /// 进行一次HTTP请求
        /// </summary>
        /// <param name="Name">HTTP请求名称</param>
        /// <returns></returns>
        public async Task<(bool succeed,ObservableCollection<BydOrderList>)> GetBydOrderLists(string Name, CancellationTokenSource cts)
        {
            LoadMesPageViewModel loadMesPageViewModel = Ioc.Default.GetRequiredService<LoadMesPageViewModel>();

            LoadMesService loadMesService = new LoadMesService(loadMesPageViewModel.LoadMesPageModel.MesPojoList);
           (bool sueeced, string? response) = await loadMesService.RunOne(Name, cts);

            //判断返回的是否是JSON
            if (sueeced)
            {
                AppJsonTool<object>.TryFormatJson(response, out bool isJson);
                if (!isJson)
                {
                    Log.Info("BydBase003OrderList--请求HTTP返回Json格式错误");
                    return (false,null);
                }
            }
            else
            {
                Log.Info("BydBase003OrderList--执行发送Http请求返回结果失败");
                return (false, null);
            } 
            //解析response
            JObject jObject = JObject.Parse(response);
            var items = jObject["data"] as JArray;
            ObservableCollection<BydOrderList> bydOrderLists = null;
            if (items != null)
            {
                List<BydOrderList> list = new List<BydOrderList>();
                //筛选
                foreach (var item in items)
                {
                    string scheduleStateCode = item["scheduleStateCode"].ToString();

                    if (1 <= int.Parse(scheduleStateCode) && int.Parse(scheduleStateCode) <= 3)
                    {
                        list?.Add(item.ToObject<BydOrderList>());
                    }
                }

                bydOrderLists = new ObservableCollection<BydOrderList>(list);
            }

            //返回筛选后的结果
            return (true,bydOrderLists);
        }

        /// <summary>
        /// 获取当前选中的
        /// </summary>
        public string DynCurrentOrder(string key)
        {
            HomePageViewModel homePageViewModel = Ioc.Default.GetRequiredService<HomePageViewModel>();
            BydOrderList currentSelectBydOrder = homePageViewModel.HomePageModel.CurrentSelectBydOrder;
            //获取当前选中的对象
            if (currentSelectBydOrder != null)
            {
                if (key == "scheduleCode")
                {
                    return currentSelectBydOrder.scheduleCode;
                }

                if (key == "orderCode")
                {
                    return currentSelectBydOrder.orderCode;
                }
            }
            return null;
        }
    }
}