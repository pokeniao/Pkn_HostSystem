using Pkn_HostSystem.Base.Enum;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Pkn_HostSystem.Base.Converter
{
    public class LoginVisibilityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            //parameter 参数传入当前的等级
            LoginPermissionEnum loginPermissionEnum = parameter is LoginPermissionEnum
                ? (LoginPermissionEnum)parameter
                : LoginPermissionEnum.UnLogIn;

            UserLoginEnum userLogin = value is UserLoginEnum ? (UserLoginEnum)value : UserLoginEnum.NoLogged;

            switch (loginPermissionEnum)
            {
                case LoginPermissionEnum.UnLogIn:
                    if (userLogin == UserLoginEnum.NoLogged)
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Hidden;
                    }

                    break;
                case LoginPermissionEnum.LogIn:
                    if (userLogin == UserLoginEnum.Developer || userLogin == UserLoginEnum.Manager ||
                        userLogin == UserLoginEnum.Operator)
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Hidden;
                    }

                    break;
                case LoginPermissionEnum.Manager:
                    if (userLogin == UserLoginEnum.Manager)
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Hidden;
                    }

                    break;
                case LoginPermissionEnum.Developer:
                    if (userLogin == UserLoginEnum.Developer)
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Hidden;
                    }

                    break;
                case LoginPermissionEnum.Operator:
                    if (userLogin == UserLoginEnum.Operator)
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Hidden;
                    }

                    break;
            }

            return Visibility.Collapsed;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}