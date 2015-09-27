using System;
using System.Windows.Data;

namespace CNCLib.Wpf.Converters
{
    public class IsBiggerThanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var x = System.Convert.ToDouble(parameter);
            var v = System.Convert.ToDouble(value);
            return (v > x);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
