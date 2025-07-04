using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Pojo;
using Pkn_HostSystem.Service.UserDefined.Interface;
using Pkn_HostSystem.ViewModels.Page;

namespace Pkn_HostSystem.Service.UserDefined
{
    public class PppBase003OrderGetMaterialCode :IUserDefined
    {
        public PppOrderList PppOrderList { get; set; }
        public LogBase<PppBase003OrderGetMaterialCode> Log = new();

        public PppBase003OrderGetMaterialCode()
        {
            //从IOC容器中获取
            HomePageViewModel homePageViewModel = Ioc.Default.GetRequiredService<HomePageViewModel>();
            PppOrderList = homePageViewModel.HomePageModel.CurrentSelectPppOrder;
        }

        public async Task<(bool Succeed, object Return)> Main(CancellationTokenSource cts)
        {
            string message = $"\"materialCode\":\"{PppOrderList.materialCode}\",\r\n\"materialName\":\"{PppOrderList.materialName}\",";

            Log.Info($"[{TraceContext.Name}]--从工单中获取到: {message}");
            return (true, message);
        }

        public string ErrorMessage()
        {
            return "自定义类PppBase003OrderGetMaterialCode错误";
        }
    }
}