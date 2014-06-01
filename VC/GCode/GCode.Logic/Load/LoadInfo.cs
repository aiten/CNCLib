using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCode.Logic.Load
{
	public class LoadInfo
	{
		public LoadInfo() { SwapXY = false; ScaleX = 1; ScaleY = 1; OfsX = 0; OfsY = 0; AutoScale = true; AutoScaleKeepRatio = true; AutoScaleBorderDistX = 0.5m; AutoScaleBorderDistY = 0.5m; }

		public String FileName { get; set; }
		public bool SwapXY { get; set; }
		public decimal ScaleX { get; set; }
		public decimal ScaleY { get; set; }
		public decimal OfsX { get; set; }
		public decimal OfsY { get; set; }

		public bool AutoScale { get; set; }
		public bool AutoScaleKeepRatio { get; set; }

		public decimal AutoScaleSizeX { get; set; }
		public decimal AutoScaleSizeY { get; set; }

		public decimal AutoScaleBorderDistX { get; set; }
		public decimal AutoScaleBorderDistY { get; set; }
	}
}
