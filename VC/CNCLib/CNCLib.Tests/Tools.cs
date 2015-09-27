using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNCLib.Tests
{
	static class Tools
	{
		public static bool CompareProperties<T>(this T dest, T src)
		{
			foreach (PropertyDescriptor item in TypeDescriptor.GetProperties(src))
			{
				if ((item.PropertyType == typeof(string) || item.PropertyType.IsValueType))
				{
					IComparable s = item.GetValue(src) as IComparable;
					IComparable d = item.GetValue(dest) as IComparable;

					if (s != null && d != null && s.CompareTo(d) != 0)
						return false;
				}
			}
			return true;
		}
	}
}
