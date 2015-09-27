using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CNCLib.Wpf.Converters
{
	[ValueConversion(typeof(bool), typeof(bool))]
	public class InvertBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool booleanValue = (bool)value;
			return !booleanValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool booleanValue = (bool)value;
			return !booleanValue;
		}
	}
}
