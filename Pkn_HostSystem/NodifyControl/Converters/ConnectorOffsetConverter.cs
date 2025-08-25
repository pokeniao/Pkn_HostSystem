using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Pkn_HostSystem.NodifyControl.Converters
{
    public class ConnectorOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //将parameter转换为double类型
            double[] offsets = parameter.ToString()?.Split(',').Select( s => double.TryParse(s, out double d)? d :0).ToArray();

            if (offsets.Length < 2 && offsets.Length >0)
            {
                offsets = [offsets[0], offsets[0]];
            }
            else if (offsets.Length != 2)
            {
                offsets = [0, 0];
            }

            if (value is Size s)
            {
                return new Size((s.Width + offsets[0]) / 2, (s.Height + offsets[1]) / 2);
            }

            return new Size(offsets[0] / 2, offsets[1] / 2);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //将parameter转换为double类型
            double[] offsets = parameter.ToString()?.Split(',').Select(s => double.TryParse(s, out double d) ? d : 0).ToArray();

            if (offsets.Length < 2 && offsets.Length > 0)
            {
                offsets = [offsets[0], offsets[0]];
            }
            else if (offsets.Length != 2)
            {
                offsets = [0, 0];
            }

            if (value is Size s)
            {
                return new Size((s.Width + offsets[0]) / 2, (s.Height + offsets[1]) / 2);
            }

            return new Size(offsets[0] / 2, offsets[1] / 2);
        }
    }
}
