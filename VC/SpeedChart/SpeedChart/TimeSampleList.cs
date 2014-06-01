using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedChart
{
	public class TimeSampleList : List<TimeSample>
	{
		public void ReadFiles(string filename)
		{
			Clear();

			using (StreamReader r = new StreamReader(filename))
			{
				string line;;

				while (!string.IsNullOrEmpty(line = r.ReadLine()))
				{
					Add(TimeSample.Parse(line));
				}
			}
		}

		public int DistFrom { get { return this.Min(x => x.Dist); } }
		public int DistTo { get { return this.Max(x => x.Dist); } }
		public int MaxSpeed { get { return this.Max(x => x.SysSpeed);} }
	}
}
