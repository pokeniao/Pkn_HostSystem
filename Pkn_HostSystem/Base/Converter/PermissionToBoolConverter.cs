using Pkn_HostSystem.Base.Enum;
using System.Globalization;
using System.Windows.Data;

namespace Pkn_HostSystem.Base.Converter
{
    /// <summary>
    /// 按键登入权限判断转换
    /// </summary>
    public class PermissionToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currentRole = (LoginPermissionEnum)value;
            var requiredRole = (LoginPermissionEnum)System.Enum.Parse(typeof(LoginPermissionEnum), parameter.ToString());
            return currentRole >= requiredRole;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

}