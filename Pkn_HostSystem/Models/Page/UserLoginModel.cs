using CommunityToolkit.Mvvm.ComponentModel;

namespace Pkn_HostSystem.Models.Page
{
    public partial class UserLoginModel:ObservableObject
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [ObservableProperty] private string name;

        /// <summary>
        /// id
        /// </summary>
        [ObservableProperty] private string id;
        /// <summary>
        /// 部门
        /// </summary>
        [ObservableProperty] private string emp;

        /// <summary>
        /// 登入状态
        /// </summary>
        [ObservableProperty] private bool loginState;
    }
}