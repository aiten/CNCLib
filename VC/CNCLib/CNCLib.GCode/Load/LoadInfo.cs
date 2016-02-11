////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  http://www.gnu.org/licenses/
*/

using System;

namespace CNCLib.GCode.Load
{
	public class LoadInfo
	{
		public String FileName { get; set; }

        public String GCodeWriteToFileName { get; set; } = @"c:\tmp\test.gcode";

        public bool SwapXY { get; set; } = false;
        public decimal ScaleX { get; set; } = 1;
        public decimal ScaleY { get; set; } = 1;
        public decimal OfsX { get; set; } = 0;
        public decimal OfsY { get; set; } = 0;

        public bool AutoScale { get; set; } = false;
        public bool AutoScaleKeepRatio { get; set; } = true;

        public decimal AutoScaleSizeX { get; set; } = 0;
        public decimal AutoScaleSizeY { get; set; } = 0;

        public decimal AutoScaleBorderDistX { get; set; } = 0.5m;
		public decimal AutoScaleBorderDistY { get; set; } = 0.5m;

        public enum PenType
        {
            ZMove,
            CommandString
        };

        public PenType PenMoveType { get; set; } = PenType.ZMove;

        public bool PenPosInParameter { get; set; } = true;
        public decimal PenPosUp { get; set; } = 1m;
        public decimal PenPosDown { get; set; } = -0.5m;

        public decimal? PenMoveSpeed { get; set; } = 500m;
        public decimal? PenDownSpeed { get; set; }

        public string PenDownCommandString { get; set; } = "M106";
        public string PenUpCommandString { get; set; } = "M107";

        public decimal LaserSize { get; set; } = 0.254m;

        public String ImageWriteToFileName { get; set; } = @"c:\tmp\image.bmp";

        public Byte GrayThreshold { get; set; } = 127;

        public decimal? ImageDPIX { get; set; }
        public decimal? ImageDPIY { get; set; }
    }
}
