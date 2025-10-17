using System.Globalization;
using System.Windows.Data;

namespace Pkn_HostSystem.Base.Converter
{
    public class MultiplyConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double d && parameter != null && double.TryParse(parameter.ToString(), out double factor))
                return d * factor;
            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}