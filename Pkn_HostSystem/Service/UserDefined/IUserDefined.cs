using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Models.Pojo;
using Pkn_HostSystem.ViewModels.Page;
using System.Reflection;

namespace Pkn_HostSystem.Service.UserDefined
{
    public interface IUserDefined
    {


        /// <summary>
        /// 执行主入口
        /// </summary>
        /// <returns></returns>
       Task<(bool Succeed, object Return)>  Main(CancellationTokenSource cts);

        /// <summary>
        /// 返回错误的信息
        /// </summary>
        /// <returns></returns>
        string ErrorMessage();
    }
}