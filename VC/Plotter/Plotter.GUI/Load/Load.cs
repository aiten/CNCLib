using Framework.Tools;
using Plotter.GUI.Shapes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		bool _lastIsPenUp;
		SpaceCoordinate _last=new SpaceCoordinate();
        Color _color;

		SpaceCoordinate _minpt;
		SpaceCoordinate _maxpt;


        public LoadInfo LoadOptions  { get; set; }

		private void InitLoad()
		{
			_last = new SpaceCoordinate();
			_minpt = new SpaceCoordinate() { X = int.MaxValue, Y = int.MaxValue };
			_maxpt = new SpaceCoordinate() { X = int.MinValue, Y = int.MinValue };
			_stream = new CommandStream();
			_IsPenUp = true;
			_lastIsPenUp = false;
			_color = Color.Black;
		}

        public void LoadHPGL(List<Shape> shapes)
		{
			InitLoad();

			if (LoadOptions.AutoScale)
			{
				using (StreamReader sr = new StreamReader(LoadOptions.FileName))
				{
					string line;
					while ((line = sr.ReadLine()) != null)
					{
						_stream.Line = line;
						if (!Command(shapes,true))
						{
							break;
						}
					}
				}
				LoadOptions.OfsX = -(_minpt.X.Value - LoadOptions.AutoScaleBorderDistX);
				LoadOptions.OfsY = -(_minpt.Y.Value - LoadOptions.AutoScaleBorderDistY);
				decimal sizex = _maxpt.X.Value - _minpt.X.Value + 2 * LoadOptions.AutoScaleBorderDistX;
				decimal sizey = _maxpt.Y.Value - _minpt.Y.Value + 2 * LoadOptions.AutoScaleBorderDistY;

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
                    if (_stream.IsInt())
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

                    while (_stream.IsInt())
                    {
                        SpaceCoordinate pt = GetSpaceCoordiante(cmdidx == 3);
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

        private SpaceCoordinate GetSpaceCoordiante(bool isRelativPoint)
        {
			SpaceCoordinate pt = new SpaceCoordinate();
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
		private void AdjustOrig(ref SpaceCoordinate pt)
		{
			if (LoadOptions.SwapXY)
			{
				decimal tmp = pt.X.Value;
				pt.X = pt.Y;
				pt.Y = -tmp;
			}
		}

        private void Adjust(ref SpaceCoordinate pt,bool isRelativPoint)
        {
            if (!isRelativPoint)
            {
                pt.X += LoadOptions.OfsX;
                pt.Y += LoadOptions.OfsY;
            }

            if (LoadOptions.ScaleX != 0)
				pt.X = Math.Round(pt.X.Value * LoadOptions.ScaleX, 3);
            if (LoadOptions.ScaleY != 0)
				pt.Y = Math.Round(pt.Y.Value * LoadOptions.ScaleY, 3);
        }

    }
}
