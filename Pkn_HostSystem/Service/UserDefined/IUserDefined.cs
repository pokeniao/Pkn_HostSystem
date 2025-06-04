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
        bool Main();

        /// <summary>
        /// 获取属性值,通过反射获取
        /// </summary>
        /// <returns></returns>
        object GetPropertyValue(string key);
        /// <summary>
        /// 返回错误的信息
        /// </summary>
        /// <returns></returns>
        string ErrorMessage();
    }
}