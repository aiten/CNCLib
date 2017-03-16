using System;
using System.Globalization;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CreateThermistorLookupBeta
{
	class Program
	{

		static void Main(string[] args)
		{
			var c = new ThermistorLookup();

			var list = c.GetLookup((double ofs) => Math.Max(0.05,Math.Abs(Math.Pow(Math.Abs(ofs - 0.5),1.8))));
			foreach(var l in list)
			{
				Console.WriteLine("{{ {0}, {1} }}, // {2}", l.X.ToString(CultureInfo.InvariantCulture), l.Y.ToString(CultureInfo.InvariantCulture), l.Maxdiff.ToString(CultureInfo.InvariantCulture));
			}

			if (false)
			{
				for (int i = 0; i < 1024; i++)
				{
					double temp = c.Calc(i);

					Console.Write(i);
					//				Console.Write(":");
					//				Console.Write(current_resistance);
					Console.Write("=>");
					Console.Write(temp);
					Console.WriteLine();
				}
			}
			Console.ReadLine();
		}
	}
}
