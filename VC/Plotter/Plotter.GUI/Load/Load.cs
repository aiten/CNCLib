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

//using CNCLib.GCode;
using Framework.Tools;
using Framework.Tools.Drawing;
using Plotter.GUI.Shapes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plotter.GUI.Load
{
    public class Load
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
		};

        CommandStream _stream;
        bool _IsPenUp;
		Point3D _last =new Point3D();
        Color _color;

		Point3D _minpt;
		Point3D _maxpt;


        public LoadInfo LoadOptions  { get; set; }

		private void InitLoad()
		{
			_last = new Point3D();
			_minpt = new Point3D() { X = int.MaxValue, Y = int.MaxValue };
			_maxpt = new Point3D() { X = int.MinValue, Y = int.MinValue };
			_stream = new CommandStream();
			_IsPenUp = true;
			_color = Color.Black;
		}

        public void LoadHPGL(List<Shape> shapes)
		{
			try
			{
				using (StreamReader sr = new StreamReader(LoadOptions.FileName))
				{

				}
			}
			catch (IOException e)
			{
				MessageBox.Show("cannot load from file");
				return;
			}
			catch (ArgumentException e)
			{
				MessageBox.Show("cannot load from file");
				return;
			}

			InitLoad();

			if (LoadOptions.AutoScale)
			{
				using (StreamReader sr = new StreamReader(LoadOptions.FileName))
				{
					string line;
					while ((line = sr.ReadLine()) != null)
					{
						_stream.Line = line;
						if (!Command(shapes, true))
						{
							break;
						}
					}
				}

				LoadOptions.OfsX = -((decimal) _minpt.X.Value - LoadOptions.AutoScaleBorderDistX);
				LoadOptions.OfsY = -((decimal) _minpt.Y.Value - LoadOptions.AutoScaleBorderDistY);
				decimal sizex = (decimal) _maxpt.X.Value - (decimal) _minpt.X.Value + 2 * LoadOptions.AutoScaleBorderDistX;
				decimal sizey = (decimal) _maxpt.Y.Value - (decimal) _minpt.Y.Value + 2 * LoadOptions.AutoScaleBorderDistY;

				LoadOptions.ScaleX = LoadOptions.AutoScaleSizeX / sizex;
				LoadOptions.ScaleY = LoadOptions.AutoScaleSizeY / sizey;

				if (LoadOptions.AutoScaleKeepRatio)
				{
					LoadOptions.ScaleX =
					LoadOptions.ScaleY = Math.Min(LoadOptions.ScaleX, LoadOptions.ScaleY);
				}
			}

			InitLoad();
			shapes.Clear();

            using (StreamReader sr = new StreamReader(LoadOptions.FileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    _stream.Line = line;
                    if (!Command(shapes,false))
                    {
                        shapes.Clear();
                        break;
                    }
                }
            }
        }

        Color[] _pencolor = new Color[] { Color.Black, Color.Blue, Color.Red, Color.Green, Color.Yellow, Color.Orange, Color.Brown, Color.Cyan };
        
		private bool Command(List<Shape> shapes, bool analyse)
        {
            string[] cmds = new string[] { "PU", "PD", "PA", "PR", "SP" };
            while (!_stream.IsEOF())
            {
                int cmdidx = _stream.IsCommand(cmds);

                if (cmdidx==4)
                {
                    if (_stream.IsNumber())
                    {
                        int coloridx = _stream.GetInt();
                        if (coloridx >= 1 && coloridx <= 8)
                            _color = _pencolor[coloridx-1];
                    }
                }
                else if (cmdidx >= 0)
                {
                    switch (cmdidx)
                    {
                        case 0: _IsPenUp = true; break;
                        case 1: _IsPenUp = false; break;
                    }

                    while (_stream.IsNumber())
                    {
						Point3D pt = GetSpaceCoordiante(cmdidx == 3);
                        if (cmdidx == 3)  // move rel
                        {
                            pt.X += _last.X;
                            pt.Y += _last.Y;
                        }
                        
                        _last = pt;

						if (!analyse)
						{
							if (_IsPenUp)
							{
								PolyLine r = new PolyLine();

								r.ForgroundColor = _color;
								r.LineSize = 1;
								r.HPGLStart =
								r.HPGLEnd = pt;

								shapes.Add(r);
							}
							else
							{
								PolyLine r = (shapes.Count > 0) ? (PolyLine)shapes.Last() : null;
								if (r == null)
								{
									r = new PolyLine();

									r.ForgroundColor = _color;
									r.LineSize = 1;
									r.HPGLStart =
									r.HPGLEnd = pt;

									shapes.Add(r);

								}
								r.Points.Add(new PolyLine.LinePoint() { HPGLPos = pt });
							}
						}
                        _stream.IsCommand(",");
                    }
                }
                else
                {
                    // skip command
                    _stream.SkipEndCommand();
                }
            }
            return true;
        }

        private Point3D GetSpaceCoordiante(bool isRelativPoint)
        {
			Point3D pt = new Point3D();
            pt.X = _stream.GetInt();
			_stream.IsCommand(",") ;
			pt.Y = _stream.GetInt();

			AdjustOrig(ref pt);

			if (pt.X != 0 || pt.Y != 0)
			{
				if (_minpt.X > pt.X) _minpt.X = pt.X;
				if (_minpt.Y > pt.Y) _minpt.Y = pt.Y;

				if (_maxpt.X < pt.X) _maxpt.X = pt.X;
				if (_maxpt.Y < pt.Y) _maxpt.Y = pt.Y;
			}

            Adjust(ref pt, isRelativPoint);
            return pt;
        }
		private void AdjustOrig(ref Point3D pt)
		{
			if (LoadOptions.SwapXY)
			{
				var tmp = pt.X.Value;
				pt.X = pt.Y;
				pt.Y = -tmp;
			}
		}

        private void Adjust(ref Point3D pt,bool isRelativPoint)
        {
            if (!isRelativPoint)
            {
                pt.X += (double) LoadOptions.OfsX;
                pt.Y += (double)LoadOptions.OfsY;
            }

            if (LoadOptions.ScaleX != 0)
				pt.X = Math.Round(pt.X.Value * (double)LoadOptions.ScaleX, 3);
            if (LoadOptions.ScaleY != 0)
				pt.Y = Math.Round(pt.Y.Value * (double)LoadOptions.ScaleY, 3);
        }
    }
}
