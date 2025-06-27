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
using Pkn_HostSystem.Service.LoadMes;
using Pkn_HostSystem.ViewModels.Page;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Pkn_HostSystem.Service.UserDefined
{
    public class PppBase003OrderList : IUserDefined
    {
        public PppOrderList PppOrderList { get; set; }
        public LogBase<PppBase003OrderList> Log = new LogBase<PppBase003OrderList>();

        public PppBase003OrderList()
        {
            //从IOC容器中获取
            HomePageViewModel homePageViewModel = Ioc.Default.GetRequiredService<HomePageViewModel>();
            PppOrderList = homePageViewModel.HomePageModel.CurrentSelectPppOrder;
        }


        /// <summary>
        /// 主入口
        /// </summary>
        /// <returns></returns>
        public async Task<(bool Succeed, object Return)> Main(CancellationTokenSource cts)
        {
            string message = $"\"materialCode\":\"{PppOrderList.materialCode}\",\r\n\"workOrderNumber\":\"{PppOrderList.orderCode}\",\r\n\"scheduleNumber\":\"{PppOrderList.scheduleCode}\",";

            Log.Info($"[{TraceContext.Name}]--从工单中获取到: {message}");
            return (true, message);
        }

        /// <summary>
        /// 返回的错误信息
        /// </summary>
        /// <returns></returns>
        public string ErrorMessage()
        {
            return "自定义类PppBase003OrderList错误";
        }


        #region 自定义方法

        /// <summary>
        /// 进行一次HTTP请求
        /// </summary>
        /// <param name="Name">HTTP请求名称</param>
        /// <returns></returns>
        public async Task<(bool succeed, ObservableCollection<PppOrderList>)> GetPppOrderLists(string Name,
            CancellationTokenSource cts)
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
                    Log.Info("PppBase003OrderList--请求HTTP返回Json格式错误");
                    return (false, null);
                }
            }
            else
            {
                Log.Info("PppBase003OrderList--执行发送Http请求返回结果失败");
                return (false, null);
            }

            //解析response
            JObject jObject = JObject.Parse(response);
            var items = jObject["data"] as JArray;
            ObservableCollection<PppOrderList> pppOrderLists = null;
            if (items != null)
            {
                List<PppOrderList> list = new List<PppOrderList>();
                //筛选
                foreach (var item in items)
                {
                    string scheduleStateCode = item["scheduleStateCode"].ToString();

                    if (1 <= int.Parse(scheduleStateCode) && int.Parse(scheduleStateCode) <= 3)
                    {
                        list?.Add(item.ToObject<PppOrderList>());
                    }
                }

                pppOrderLists = new ObservableCollection<PppOrderList>(list);
            }

            //返回筛选后的结果
            return (true, pppOrderLists);
        }

        #endregion
    }
}