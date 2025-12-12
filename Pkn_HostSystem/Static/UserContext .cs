using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Base.Enum;

namespace Pkn_HostSystem.Static
{
    public class UserContext:ObservableObject
    {
        // ① 创建一个唯一实例，只会创建一次
        private static UserContext _current = new UserContext();

        // ② 提供一个只读入口，用来访问这个唯一实例
        public static UserContext Current => _current;

        // 实例的属性
        private LoginPermissionEnum _permission = LoginPermissionEnum.Lv0;
        public LoginPermissionEnum Permission
        {
            get => _permission;
            set => SetProperty(ref _permission, value);
        }
    }
}