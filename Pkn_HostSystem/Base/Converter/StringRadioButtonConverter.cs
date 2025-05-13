using System.Globalization;
using System.Windows.Data;

namespace Pkn_HostSystem.Base.Converter
{
    public class StringRadioButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == parameter?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value) return parameter.ToString();
            return Binding.DoNothing;
        }
    }
}