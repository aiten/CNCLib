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

		public double SizeX { get { return _sizeX; } set { _sizeX = value; CalcRatio(); } }
		public double SizeY { get { return _sizeY; } set { _sizeY = value; CalcRatio(); } }
		public double SizeZ { get { return _sizeZ; } set { _sizeZ = value; CalcRatio(); } }

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

		public Rotate3D Rotate { get { return _rotate3D; } set { _rotate3D = value; ReInitDraw(); } }
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

		Rotate3D _rotate3D = new Rotate3D();

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

		public Point3D FromClient(Point pt)
		{
			// with e.g.  867
			// max pt.X = 686 , pt.x can be 0
			return new Point3D(
							AdjustX(Math.Round(pt.X / _ratioX / Zoom, 3)),
							AdjustY(Math.Round(pt.Y / _ratioY / Zoom, 3)),
							0);
		}

		PointF ToClientF(Point3D pt)
		{
			pt = _rotate3D.Rotate(pt);

			double x = (((pt.X ?? 0) - OffsetX) * Zoom) * _ratioX;
			double y = (((SizeY - (((pt.Y ?? 0)) + OffsetY))) * Zoom) * _ratioY;
			//double z = ((double)((double)(pt.Z ?? 0) - (double)OffsetZ) * Zoom) * _ratioZ;

			return new PointF((float) x, (float) y);
		}
		Point ToClient(Point3D pt)
		{
			var p = ToClientF(pt);
			return new Point((int)Math.Round(p.X, 0), (int)Math.Round(p.Y, 0));
		}
		/*
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
				double ToClientZ(double val)
				{
					double z = (double)(((double)SizeZ - (val + (double)OffsetZ))) * Zoom;
					return _ratioZ * z;
				}
				int ToClientXInt(double val)
				{
					return (int)Math.Round(ToClientX(val), 0);
				}
				int ToClientYInt(double val)
				{
					return (int)Math.Round(ToClientY(val), 0);
				}
		*/
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

				_cutPens = new Pen[] { _cutPen, _cutDotPen, _cutEllipsePen, _cutArcPen };

				_fastPen = new Pen(FastMoveColor, fastSize);
				_noMovePen = new Pen(Color.Blue, fastSize);
				_laserCutPen = new Pen(LaserOnColor, (float) ToClientSizeX(LaserSize));
				_laserCutPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
				_laserCutPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
				_laserFastPen = new Pen(LaserOffColor, (float)(fastSize / 2.0));

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

			var from = ToClient(new Point3D(0, SizeY, 0));
			var to = ToClient(new Point3D(SizeX, 0, 0));

			var pts = new PointF[] { ToClientF(new Point3D(0, 0, 0)), ToClientF(new Point3D(0, SizeY, 0)), ToClientF(new Point3D(SizeX, SizeY, 0)), ToClientF(new Point3D(SizeX, 0, 0)) };
			g1.FillPolygon(new SolidBrush(MachineColor), pts);

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

			double cx = ptFrom.X.Value + I;
			double cy = ptFrom.Y.Value + J;

			double startAng = ConvertRadToDeg(Math.Atan2(J, I));
			double endAng = ConvertRadToDeg(Math.Atan2(cy - ptTo.Y.Value, cx - ptTo.X.Value));
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
