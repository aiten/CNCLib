////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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
using System.Drawing;
using System.Windows.Forms;
using Framework.Tools.Drawing;
using CNCLib.GCode.Commands;
using Framework.Arduino;
using System.Drawing.Drawing2D;

namespace CNCLib.GUI
{
	public partial class GCodeBitmapDraw : IOutputCommand
	{
		#region crt

		public GCodeBitmapDraw()
		{
		}

		#endregion

		#region Properties

		public decimal SizeX { get { return _sizeX; } set { _sizeX = value; CalcRatio(); } }
		public decimal SizeY { get { return _sizeY; } set { _sizeY = value; CalcRatio(); } }

		public bool KeepRatio { get { return _keepRatio; } set { _keepRatio = value; CalcRatio(); } }

		public double Zoom { get { return _zoom; } set { _zoom = value; ReInitDraw(); } }
		public decimal OffsetX { get { return _offsetX; } set { _offsetX = value; ReInitDraw(); } }
		public decimal OffsetY { get { return _offsetY; } set { _offsetY = value; ReInitDraw(); } }
		public decimal CutterSize { get { return _cutterSize; } set { _cutterSize = value; ReInitDraw(); } }
		public decimal LaserSize { get { return _laserSize; } set { _laserSize = value; ReInitDraw(); } }

		public Color MachineColor { get { return _machineColor; } set { _machineColor = value; ReInitDraw(); } }
		public Color LaserOnColor { get { return _laserOnColor; } set { _laserOnColor = value; ReInitDraw(); } }
		public Color LaserOffColor { get { return _laserOffColor; } set { _laserOffColor = value; ReInitDraw(); } }
		public Color CutColor { get { return _cutColor; } set { _cutColor = value; ReInitDraw(); } }


		public CommandList Commands { get { return _commands; } }

		public Size RenderSize
		{
			get { return _renderSize; }
			set
			{
				if (_renderSize.Height != 0 && value.Width > 0 && value.Height > 0)
				{
					//RecalcClientCoord();
				}
				_renderSize = value;
				CalcRatio();
			}
		}

		#endregion

		#region private Members

		bool _needReInit = true;

		Size _renderSize;
		bool _keepRatio = true;
		double _zoom = 1;
		double _ratioX = 1;
		double _ratioY = 1;
		decimal _sizeX = 130.000m;
		decimal _sizeY = 45.000m;
		decimal _offsetX = 0;
		decimal _offsetY = 0;
		decimal _cutterSize = 0;
		decimal _laserSize = 0.254m;
		Color _machineColor = Color.Black;
		Color _laserOnColor = Color.Red;
		Color _laserOffColor = Color.Orange;
		Color _cutColor = Color.White;
		Color _cutDotColor = Color.Blue;
		Color _cutEllipseColor = Color.Cyan;
		Color _noMoveColor = Color.Blue;
		Color _fastLineColor = Color.Green;

		CommandList _commands = new CommandList();

		private ArduinoSerialCommunication Com
		{
			get { return Framework.Tools.Pattern.Singleton<ArduinoSerialCommunication>.Instance; }
		}

		#endregion

		#region Convert Coordinats

		decimal AdjustX(decimal xx)
		{
			// x: 0...
			return xx + OffsetX;
		}

		decimal AdjustY(decimal yy)
		{
			// y: 0...
			return SizeY - (yy + OffsetY);
		}

		public Point3D FromClient(Point pt)
		{
			// with e.g.  867
			// max pt.X = 686 , pt.x can be 0
			return new Point3D(
							AdjustX((decimal)Math.Round(pt.X / _ratioX / Zoom, 3)),
							AdjustY((decimal)Math.Round(pt.Y / _ratioY / Zoom, 3)),
							0);
		}

		Point ToClient(Point3D pt)
		{
			return new Point(
				ToClientXInt((double)(pt.X ?? 0)),
				ToClientYInt((double)(pt.Y ?? 0)));
		}

		double ToClientX(double val)
		{
			double x = (double)(val - (double)OffsetX) * Zoom;
			return _ratioX * x;
		}
		double ToClientY(double val)
		{
			double y = (double)(((double)SizeY - (val + (double)OffsetY))) * Zoom;
			return _ratioY * y;
		}
		int ToClientXInt(double val)
		{
			return (int)Math.Round(ToClientX(val), 0);
		}
		int ToClientYInt(double val)
		{
			return (int)Math.Round(ToClientY(val), 0);
		}

		const double SignX = 1.0;
		const double SignY = -1.0;

		int ToClientSizeX(double X)
		{
			double x = X * Zoom;
			return (int)Math.Round(_ratioX * x, 0);
		}
		int ToClientSizeY(double Y)
		{
			double y = Y * Zoom;
			return (int)Math.Round(_ratioY * y, 0);
		}

		#endregion

		#region private

		void ReInitDraw()
		{
			_needReInit = true;
		}

		void InitPen()
		{
			if (_needReInit)
			{
				Color cutColor = MachineColor == Color.White ? Color.Black : Color.White;
				float cutsize = CutterSize > 0 ? (float)ToClient(new Point3D(OffsetX + CutterSize, 0m, 0m)).X : 2;
				float fastSize = 0.5f;

				_cutLinePen = new Pen(cutColor, cutsize);
				_cutLinePen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
				_cutLinePen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

				_cutDotPen = new Pen(_cutDotColor, cutsize);
				_cutEllipsePen = new Pen(_cutEllipseColor, cutsize);

				_cutPen = new Pen[] { _cutLinePen, _cutDotPen, _cutEllipsePen };

				_fastLinePen = new Pen(_fastLineColor, fastSize);
				_noMovePen = new Pen(Color.Blue, fastSize);
				_laserCutLinePen = new Pen(LaserOnColor, ToClient(new Point3D(OffsetX + LaserSize, 0m, 0m)).X);
				_laserCutLinePen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
				_laserCutLinePen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
				_laserFastLinePen = new Pen(LaserOffColor, (float)(fastSize / 2.0));

				_needReInit = false;
			}
		}

		public Bitmap DrawToBitmap()
		{
			InitPen();

			Bitmap curBitmap = new Bitmap(RenderSize.Width, RenderSize.Height);

			Graphics g1 = Graphics.FromImage(curBitmap);
			g1.InterpolationMode = InterpolationMode.NearestNeighbor;
			g1.SmoothingMode = SmoothingMode.None;
			g1.PixelOffsetMode = PixelOffsetMode.None;
			g1.CompositingQuality = CompositingQuality.HighSpeed;
			g1.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;

			var ee = new PaintEventArgs(g1, new Rectangle());

			Point from = ToClient(new Point3D(0, SizeY, 0));
			Point to = ToClient(new Point3D(SizeX, 0, 0m));
			Size sz = new Size(to.X - from.X, to.Y - from.Y);
			Rectangle rc = new Rectangle(from, sz);

			g1.FillRectangle(new SolidBrush(MachineColor), rc);

			_commands.Paint(this, ee);

			g1.Dispose();
			return curBitmap;
		}

		private void CalcRatio()
		{
			_ratioX = RenderSize.Width / (double)SizeX;
			_ratioY = RenderSize.Height / (double)SizeY;

			if (KeepRatio)
			{
				if (_ratioX > _ratioY) _ratioX = _ratioY;
				else if (_ratioX < _ratioY) _ratioY = _ratioX;
			}
		}

		#endregion

		#region IOutput 

		Pen _noMovePen;
		Pen _cutEllipsePen;
		Pen _cutLinePen;
		Pen _cutDotPen;
		Pen[] _cutPen;
		Pen _fastLinePen;
		Pen _laserCutLinePen;
		Pen _laserFastLinePen;

		public void DrawLine(Command cmd, object param, DrawType drawtype, Point3D ptFrom, Point3D ptTo)
		{
			if (drawtype == DrawType.NoDraw) return;

			PaintEventArgs e = (PaintEventArgs)param;

			Point from = ToClient(ptFrom);
			Point to = ToClient(ptTo);

			if (PreDrawLineOrArc(param, drawtype, from, to))
			{
				e.Graphics.DrawLine(GetPen(drawtype, LineDrawType.Line), from, to);
			}
		}

		public void DrawEllipse(Command cmd, object param, DrawType drawtype, Point3D ptCenter, int xradius, int yradius)
		{
			if (drawtype == DrawType.NoDraw) return;

			PaintEventArgs e = (PaintEventArgs)param;
			Point from = ToClient(ptCenter);
			e.Graphics.DrawEllipse(GetPen(drawtype, LineDrawType.Ellipse), from.X - xradius / 2, from.Y - yradius / 2, xradius, yradius);
		}

		public void DrawArc(Command cmd, object param, DrawType drawtype, Point3D ptFrom, Point3D ptTo, Point3D pIJ, bool clockwise)
		{
			if (drawtype == DrawType.NoDraw) return;

			PaintEventArgs e = (PaintEventArgs)param;

			Point from = ToClient(ptFrom);
			Point to = ToClient(ptTo);

			//e.Graphics.DrawLine(_helpLine, from, to);

			double I = (double)pIJ.X.Value;
			double J = (double)pIJ.Y.Value;
			double R = Math.Sqrt(I * I + J * J);

			double cx = (double)ptFrom.X.Value + I;
			double cy = (double)ptFrom.Y.Value + J;

			double startAng = ConvertRadToDeg(Math.Atan2(J, I));
			double endAng = ConvertRadToDeg(Math.Atan2(cy - (double)ptTo.Y.Value, cx - (double)ptTo.X.Value));
			double diffAng = (endAng - startAng);
			if (startAng > endAng)
				diffAng += 360;

			if (clockwise == false)
			{
				startAng = endAng;
				diffAng = 360 - diffAng;
				while (diffAng > 360)
					diffAng -= 360;

				while (diffAng < -360)
					diffAng += 360;
			}

			Point rcfrom = new Point(ToClientXInt(cx - R * SignX), ToClientYInt(cy - R * SignY));
			int RR = ToClientSizeX(R * 2);
			Rectangle rec = new Rectangle(rcfrom, new Size(RR, RR));

			//e.Graphics.DrawRectangle(_helpLine, rec);

			if (rec.Width > 0 && rec.Height > 0)
			{
				try
				{
					e.Graphics.DrawArc(GetPen(drawtype, LineDrawType.Line), rec.X, rec.Y, rec.Width, rec.Height, (float)startAng, (float)diffAng);
				}
				catch (OutOfMemoryException)
				{
					// ignore this Exception
				}
			}
		}

		private bool PreDrawLineOrArc(object param, DrawType drawtype, Point from, Point to)
		{
			PaintEventArgs e = (PaintEventArgs)param;

			if (from.Equals(to))
			{
				if (drawtype == DrawType.LaserCut)
					e.Graphics.DrawEllipse(GetPen(drawtype, LineDrawType.Dot), from.X, from.Y, 1, 1);
				else
					e.Graphics.DrawEllipse(GetPen(drawtype, LineDrawType.Dot), from.X, from.Y, 4, 4);

				return false;
			}
			return true;
		}

		static double ConvertRadToDeg(double rad)
		{
			double deg = -rad * 180.0 / Math.PI + 180.0;
			while (deg < 0)
				deg += 360;

			while (deg > 360)
				deg -= 360;

			return deg;
		}

		enum LineDrawType
		{
			Line = 0,
			Dot = 1,
			Ellipse = 2
		};

		private Pen GetPen(DrawType moveType, LineDrawType drawtype)
		{
			switch (moveType)
			{
				default:
				case DrawType.NoMove: return _noMovePen;
				case DrawType.Fast: return _fastLinePen;
				case DrawType.Cut: return _cutPen[(int)drawtype];
				case DrawType.LaserFast: return _laserFastLinePen;
				case DrawType.LaserCut: return _laserCutLinePen;
			}
		}

		#endregion
	}
}
