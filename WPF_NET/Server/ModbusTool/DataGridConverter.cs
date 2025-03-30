using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPF_NET.Server.ModbusTool;

/// <summary>
/// 返回正常的
/// </summary>
public class openTextBoxbyTrue : IValueConverter
{
    // 数据源 → UI
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            if (b)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }
        else
        {
            return value;
        }
    }
    // UI → 数据源
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value;
    }
}

/// <summary>
/// 取反
/// </summary>
public class openCheckBoxbyFalse : IValueConverter
{
    // 数据源 → UI
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            if (b)
            {

                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }
        else
        {
            return value;
        }

    }
    // UI → 数据源
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value;
    }
}