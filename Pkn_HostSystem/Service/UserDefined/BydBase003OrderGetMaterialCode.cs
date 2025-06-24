using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Pojo;
using Pkn_HostSystem.ViewModels.Page;

namespace Pkn_HostSystem.Service.UserDefined
{
    public class BydBase003OrderGetMaterialCode :IUserDefined
    {
        public BydOrderList BydOrderList { get; set; }
        public LogBase<BydBase003OrderGetMaterialCode> Log = new LogBase<BydBase003OrderGetMaterialCode>();

        public BydBase003OrderGetMaterialCode()
        {
            //从IOC容器中获取
            HomePageViewModel homePageViewModel = Ioc.Default.GetRequiredService<HomePageViewModel>();
            BydOrderList = homePageViewModel.HomePageModel.CurrentSelectBydOrder;
        }

        public async Task<(bool Succeed, object Return)> Main(CancellationTokenSource cts)
        {
            string message = $"\"materialCode\":\"{BydOrderList.materialCode}\",\r\n\"materialName\":\"{BydOrderList.materialName}\",";

            Log.Info($"[{TraceContext.Name}]--从工单中获取到: {message}");
            return (true, message);
        }

        public object GetPropertyValue(string key)
        {
            throw new NotImplementedException();
        }

        public string ErrorMessage()
        {
            return "自定义类BydBase003OrderGetMaterialCode错误";
        }
    }
}