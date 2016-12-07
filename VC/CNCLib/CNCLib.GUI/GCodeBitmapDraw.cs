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
			Rotate = new Rotate3D();
		}

		#endregion

		#region Properties

		public double SizeX { get { return _sizeX; } set { bool calc = _sizeX != value;  _sizeX = value; if (calc) CalcRatio(); } }
		public double SizeY { get { return _sizeY; } set { bool calc = _sizeY != value;  _sizeY = value; if (calc) CalcRatio(); } }
		public double SizeZ { get { return _sizeZ; } set { bool calc = _sizeZ != value;  _sizeZ = value; if (calc) CalcRatio(); } }

		public bool KeepRatio { get { return _keepRatio; } set { _keepRatio = value; CalcRatio(); } }

		public double Zoom { get { return _zoom; } set { _zoom = value; ReInitDraw(); } }
		public double OffsetX { get { return _offsetX; } set { _offsetX = value; ReInitDraw(); } }
		public double OffsetY { get { return _offsetY; } set { _offsetY = value; ReInitDraw(); } }
		public double OffsetZ { get { return _offsetZ; } set { _offsetZ = value; ReInitDraw(); } }
		public double CutterSize { get { return _cutterSize; } set { _cutterSize = value; ReInitDraw(); } }
		public double LaserSize { get { return _laserSize; } set { _laserSize = value; ReInitDraw(); } }

		public Color MachineColor { get { return _machineColor; } set { _machineColor = value; ReInitDraw(); } }
		public Color LaserOnColor { get { return _laserOnColor; } set { _laserOnColor = value; ReInitDraw(); } }
		public Color LaserOffColor { get { return _laserOffColor; } set { _laserOffColor = value; ReInitDraw(); } }

		public Color CutColor { get { return _cutColor; } set { _cutColor = value; ReInitDraw(); } }
		public Color CutDotColor { get { return _cutDotColor; } set { _cutDotColor = value; ReInitDraw(); } }
		public Color CutEllipseColor { get { return _cutEllipseColor; } set { _cutEllipseColor = value; ReInitDraw(); } }
		public Color CutArcColor { get { return _cutArcColor; } set { _cutArcColor = value; ReInitDraw(); } }

		public Color FastMoveColor { get { return _fastColor; } set { _fastColor = value; ReInitDraw(); } }
		public Color HelpLineColor { get { return _helpLineColor; } set { _helpLineColor = value; ReInitDraw(); } }

		public Rotate3D Rotate { get { return _rotate3D; } set { _rotate3D = value; PrepareRotate(); ReInitDraw(); } } 
		//		public CommandList Commands { get { return _commands; } }

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
		double _ratioZ = 1;
		double _sizeX = 130.000;
		double _sizeY = 45.000;
		double _sizeZ = 70.000;
		double _offsetX = 0;
		double _offsetY = 0;
		double _offsetZ = 0;
		double _cutterSize = 0;
		double _laserSize = 0.254;
		Color _machineColor = Color.Black;
		Color _laserOnColor = Color.Red;
		Color _laserOffColor = Color.Orange;
		Color _cutColor = Color.White;
		Color _cutDotColor = Color.Blue;
		Color _cutEllipseColor = Color.Cyan;
		Color _cutArcColor = Color.Beige;
		Color _noMoveColor = Color.Blue;
		Color _fastColor = Color.Green;
		Color _helpLineColor = Color.LightGray;

		Rotate3D _rotate3D;

		//		CommandList _commands = new CommandList();

		private ArduinoSerialCommunication Com
		{
			get { return Framework.Tools.Pattern.Singleton<ArduinoSerialCommunication>.Instance; }
		}

		#endregion

		#region Convert Coordinats

		double AdjustX(double xx)
		{
			// x: 0...
			return xx + OffsetX;
		}

		double AdjustY(double yy)
		{
			// y: 0...
			return SizeY - (yy + OffsetY);
		}

		public Point3D FromClient(PointF pt)
		{
			// with e.g.  867
			// max pt.X = 686 , pt.x can be 0
			return new Point3D(
							AdjustX(Math.Round(pt.X / _ratioX / Zoom, 3)),
							AdjustY(Math.Round(pt.Y / _ratioY / Zoom, 3)),
							0);
		}

		double _rotateScaleX;
		double _rotateScaleY;
		double _rotateAngleX;
		double _rotateAngleY;
		double _rotateZscaleX;
		double _rotateZscaleY;

		void PrepareRotate()
		{
			var axisX = Rotate.Rotate(1.0, 0.0, 0.0);
			var axisY = Rotate.Rotate(0.0, 1.0, 0.0);
			var axisZ = Rotate.Rotate(0.0, 0.0, 1.0);
			var axisXX = axisX.X ?? 0.0;
			var axisXY = axisX.Y ?? 0.0;
			var axisYX = axisY.X ?? 0.0;
			var axisYY = axisY.Y ?? 0.0;

			_rotateAngleX = Math.Atan2(axisXY, axisXX);
			_rotateAngleY = Math.Atan2(axisYY, axisYX);
			_rotateScaleX = Math.Sqrt(axisXX * axisXX + axisXY * axisXY);
			_rotateScaleY = Math.Sqrt(axisYX * axisYX + axisYY * axisYY);

			_rotateZscaleX = axisZ.X ?? 0.0;
			_rotateZscaleY = axisZ.Y ?? 0.0;
		}

		public Point3D FromClient(PointF pt, double z)
		{
			var notrotated = FromClient(pt);
			var notrotatedX = notrotated.X ?? 0.0;
			var notrotatedY = notrotated.Y ?? 0.0;
			var notrotatedAngel = Math.Atan2(notrotatedY, notrotatedX);
			var c = Math.Sqrt(notrotatedX * notrotatedX + notrotatedY * notrotatedY);

			var anglec =  Math.PI - (_rotateAngleY - _rotateAngleX);
			var anglea = _rotateAngleY - notrotatedAngel;
			var angleb = notrotatedAngel - _rotateAngleX;
			var rateC = c / Math.Sin(anglec);

			var a = Math.Sin(anglea) * rateC;
			var b = Math.Sin(angleb) * rateC;

			return new Point3D(z* _rotateZscaleX + a / _rotateScaleX, z * _rotateZscaleY + b / _rotateScaleY, z);
		}

		PointF ToClientF(Point3D pt)
		{
			pt = _rotate3D.Rotate(pt);

			double x = (((pt.X ?? 0) - OffsetX) * Zoom) * _ratioX;
			double y = (((SizeY - (((pt.Y ?? 0)) + OffsetY))) * Zoom) * _ratioY;
			//double z = ((double)((double)(pt.Z ?? 0) - (double)OffsetZ) * Zoom) * _ratioZ;

			return new PointF((float) x, (float) y);
		}

		const double SignX = 1.0;
		const double SignY = -1.0;

		double ToClientSizeX(double X)
		{
			var pt3d = new Point3D(X, 0, 0);
			pt3d = Rotate.Rotate(pt3d);
			double x = (pt3d.X??0) * Zoom;
			return _ratioX * x;
		}
		double ToClientSizeY(double Y)
		{
			var pt3d = new Point3D(0,Y, 0);
			pt3d = Rotate.Rotate(pt3d);
			double y = (pt3d.Y ?? 0) * Zoom;
			return _ratioY * y;
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
				float cutsize = CutterSize > 0 ? (float)ToClientSizeX(CutterSize) : 2;
				float fastSize = 0.5f;

				_cutPen = new Pen(CutColor, cutsize);
				_cutPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
				_cutPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

				_cutDotPen = new Pen(CutDotColor, cutsize);
				_cutEllipsePen = new Pen(CutEllipseColor, cutsize);
				_cutArcPen = new Pen(CutArcColor, cutsize);
				_cutArcPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
				_cutArcPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

				_cutPens = new Pen[] { _cutPen, _cutDotPen, _cutEllipsePen, _cutArcPen };

				_fastPen = new Pen(FastMoveColor, fastSize);
				_noMovePen = new Pen(Color.Blue, fastSize);
				_laserCutPen = new Pen(LaserOnColor, (float) ToClientSizeX(LaserSize));
				_laserCutPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
				_laserCutPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
				_laserFastPen = new Pen(LaserOffColor, (float)(fastSize / 2.0));

				_helpLinePen = new Pen(_helpLineColor, (float)(fastSize / 2.0));
				_helpLinePen10 = new Pen(_helpLineColor, (float)(fastSize*4));

				_needReInit = false;
			}
		}

		public Bitmap DrawToBitmap(CommandList commands)
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

			var pts = new PointF[] { ToClientF(new Point3D(0, 0, 0)), ToClientF(new Point3D(0, SizeY, 0)), ToClientF(new Point3D(SizeX, SizeY, 0)), ToClientF(new Point3D(SizeX, 0, 0)) };
			g1.FillPolygon(new SolidBrush(MachineColor), pts);

			for (int i = 1; ; i++)
			{
				var x = i * 10.0;
				if (x > SizeX)	break;
				g1.DrawLine(((i % 10) == 0) ? _helpLinePen10 : _helpLinePen, ToClientF(new Point3D(i * 10.0, 0, 0)), ToClientF(new Point3D(i * 10.0, SizeY, 0)));
			}
			for (int i = 1; ; i++)
			{
				var y = i * 10.0;
				if (y > SizeY) break;
				g1.DrawLine(((i % 10) == 0) ? _helpLinePen10 : _helpLinePen, ToClientF(new Point3D(0, i * 10.0, 0)), ToClientF(new Point3D(SizeX, i * 10.0, 0)));
			}

			commands?.Paint(this, ee);

			g1.Dispose();
			return curBitmap;
		}

		private void CalcRatio()
		{
			_ratioX = RenderSize.Width / SizeX;
			_ratioY = RenderSize.Height / SizeY;

			if (KeepRatio)
			{
				if (_ratioX > _ratioY) _ratioX = _ratioY;
				else if (_ratioX < _ratioY) _ratioY = _ratioX;
				_ratioZ = _ratioX;
			}
		}

		#endregion

		#region IOutput 

		Pen _noMovePen;
		Pen _cutEllipsePen;
		Pen _cutArcPen;
		Pen _cutPen;
		Pen _cutDotPen;
		Pen[] _cutPens;
		Pen _fastPen;
		Pen _laserCutPen;
		Pen _laserFastPen;
		Pen _helpLinePen;
		Pen _helpLinePen10;

		public void DrawLine(Command cmd, object param, DrawType drawtype, Point3D ptFrom, Point3D ptTo)
		{
			if (drawtype == DrawType.NoDraw) return;

			PaintEventArgs e = (PaintEventArgs)param;

			var from = ToClientF(ptFrom);
			var to   = ToClientF(ptTo);

			if (PreDrawLineOrArc(param, drawtype, from, to))
			{
				e.Graphics.DrawLine(GetPen(drawtype, LineDrawType.Line), from, to);
			}
		}

		public void DrawEllipse(Command cmd, object param, DrawType drawtype, Point3D ptCenter, int xradius, int yradius)
		{
			if (drawtype == DrawType.NoDraw) return;

			PaintEventArgs e = (PaintEventArgs)param;
			var from = ToClientF(ptCenter);
			e.Graphics.DrawEllipse(GetPen(drawtype, LineDrawType.Ellipse), from.X - xradius / 2, from.Y - yradius / 2, xradius, yradius);
		}

		public void DrawArc(Command cmd, object param, DrawType drawtype, Point3D ptFrom, Point3D ptTo, Point3D pIJ, bool clockwise)
		{
			if (drawtype == DrawType.NoDraw) return;

			PaintEventArgs e = (PaintEventArgs)param;

			var from = ToClientF(ptFrom);
			var to   = ToClientF(ptTo);

			pIJ = Rotate.Rotate(pIJ);

			double I = pIJ.X.Value;
			double J = pIJ.Y.Value;
			double R = Math.Sqrt(I * I + J * J);

			double cx = (ptFrom.X??0.0) + I;
			double cy = (ptFrom.Y??0.0) + J;

			double startAng = ConvertRadToDeg(Math.Atan2(J, I));
			double endAng = ConvertRadToDeg(Math.Atan2(cy - (ptTo.Y??0.0), cx - (ptTo.X??0.0)));
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

			var pt3d = new Point3D(cx - R * SignX, cy - R * SignY, ptFrom.Z??0);
			PointF rcfrom = ToClientF(pt3d);
			//Point rcfrom = new Point(ToClientXInt(cx - R * SignX), ToClientYInt(cy - R * SignY));
			float RR = (float) ToClientSizeX(R * 2);
			var rec = new RectangleF(rcfrom, new SizeF(RR, RR));

			//e.Graphics.DrawRectangle(_helpLine, rec);

			if (rec.Width > 0 && rec.Height > 0)
			{
				try
				{
					e.Graphics.DrawArc(GetPen(drawtype, LineDrawType.Arc), rec.X, rec.Y, rec.Width, rec.Height, (float)startAng, (float)diffAng);
				}
				catch (OutOfMemoryException)
				{
					// ignore this Exception
				}
				catch (ArgumentException)
				{
				}
			}
		}

		private bool PreDrawLineOrArc(object param, DrawType drawtype, PointF from, PointF to)
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
			Ellipse = 2,
			Arc = 3
		};

		private Pen GetPen(DrawType moveType, LineDrawType drawtype)
		{
			switch (moveType)
			{
				default:
				case DrawType.NoMove: return _noMovePen;
				case DrawType.Fast: return _fastPen;
				case DrawType.Cut: return _cutPens[(int)drawtype];
				case DrawType.LaserFast: return _laserFastPen;
				case DrawType.LaserCut: return _laserCutPen;
			}
		}

		#endregion
	}

}
