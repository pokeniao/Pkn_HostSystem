using System.Globalization;
using System.Windows.Data;

namespace WPF_NET.Server.Home;

public class HomePageAllowButtonConverter: IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        bool b1 = values[1] is bool && true;
        return b1;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}