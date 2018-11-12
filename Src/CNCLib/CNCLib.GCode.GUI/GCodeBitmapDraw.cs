////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using CNCLib.GCode.Commands;

using Framework.Drawing;

namespace CNCLib.GCode.GUI
{
    public class GCodeBitmapDraw : IOutputCommand
    {
        #region crt

        public GCodeBitmapDraw()
        {
            Rotate = new Rotate3D();
        }

        #endregion

        #region Properties

        public double SizeX
        {
            get => _sizeX;
            set
            {
                bool calc = Math.Abs(_sizeX - value) > double.Epsilon;
                _sizeX = value;
                if (calc)
                {
                    CalcRatio();
                }
            }
        }

        public double SizeY
        {
            get => _sizeY;
            set
            {
                bool calc = Math.Abs(_sizeY - value) > double.Epsilon;
                _sizeY = value;
                if (calc)
                {
                    CalcRatio();
                }
            }
        }

        public double SizeZ
        {
            get => _sizeZ;
            set
            {
                bool calc = Math.Abs(_sizeZ - value) > double.Epsilon;
                _sizeZ = value;
                if (calc)
                {
                    CalcRatio();
                }
            }
        }

        public bool KeepRatio
        {
            get => _keepRatio;
            set
            {
                _keepRatio = value;
                CalcRatio();
            }
        }

        public double Zoom
        {
            get => _zoom;
            set
            {
                _zoom = value;
                ReInitDraw();
            }
        }

        public double OffsetX
        {
            get => _offsetX;
            set
            {
                _offsetX = value;
                ReInitDraw();
            }
        }

        public double OffsetY
        {
            get => _offsetY;
            set
            {
                _offsetY = value;
                ReInitDraw();
            }
        }

        public double OffsetZ
        {
            get => _offsetZ;
            set
            {
                _offsetZ = value;
                ReInitDraw();
            }
        }

        public double CutterSize
        {
            get => _cutterSize;
            set
            {
                _cutterSize = value;
                ReInitDraw();
            }
        }

        public double LaserSize
        {
            get => _laserSize;
            set
            {
                _laserSize = value;
                ReInitDraw();
            }
        }

        public Color MachineColor
        {
            get => _machineColor;
            set
            {
                _machineColor = value;
                ReInitDraw();
            }
        }

        public Color LaserOnColor
        {
            get => _laserOnColor;
            set
            {
                _laserOnColor = value;
                ReInitDraw();
            }
        }

        public Color LaserOffColor
        {
            get => _laserOffColor;
            set
            {
                _laserOffColor = value;
                ReInitDraw();
            }
        }

        public Color CutColor
        {
            get => _cutColor;
            set
            {
                _cutColor = value;
                ReInitDraw();
            }
        }

        public Color CutDotColor
        {
            get => _cutDotColor;
            set
            {
                _cutDotColor = value;
                ReInitDraw();
            }
        }

        public Color CutEllipseColor
        {
            get => _cutEllipseColor;
            set
            {
                _cutEllipseColor = value;
                ReInitDraw();
            }
        }

        public Color CutArcColor
        {
            get => _cutArcColor;
            set
            {
                _cutArcColor = value;
                ReInitDraw();
            }
        }

        public Color FastMoveColor
        {
            get => _fastColor;
            set
            {
                _fastColor = value;
                ReInitDraw();
            }
        }

        public Color HelpLineColor
        {
            get => _helpLineColor;
            set
            {
                _helpLineColor = value;
                ReInitDraw();
            }
        }

        public Rotate3D Rotate
        {
            get => _rotate3D;
            set
            {
                _rotate3D = value;
                PrepareRotate();
                ReInitDraw();
            }
        }

        public Size RenderSize
        {
            get => _renderSize;
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

        Size   _renderSize;
        bool   _keepRatio  = true;
        double _zoom       = 1;
        double _ratioX     = 1;
        double _ratioY     = 1;
        double _ratioZ     = 1;
        double _sizeX      = 130.000;
        double _sizeY      = 45.000;
        double _sizeZ      = 70.000;
        double _offsetX    = 0;
        double _offsetY    = 0;
        double _offsetZ    = 0;
        double _cutterSize = 0;
        double _laserSize  = 0.254;

        Color _machineColor    = Color.Black;
        Color _laserOnColor    = Color.Red;
        Color _laserOffColor   = Color.Orange;
        Color _cutColor        = Color.White;
        Color _cutDotColor     = Color.Blue;
        Color _cutEllipseColor = Color.Cyan;
        Color _cutArcColor     = Color.Beige;
        Color _noMoveColor     = Color.Blue;
        Color _fastColor       = Color.Green;
        Color _helpLineColor   = Color.LightGray;

        Rotate3D _rotate3D;

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
            return new Point3D(AdjustX(Math.Round(pt.X / _ratioX / Zoom, 3)), AdjustY(Math.Round(pt.Y / _ratioY / Zoom, 3)), 0);
        }

        double _rotateScaleX;
        double _rotateScaleY;
        double _rotateAngleX;
        double _rotateAngleY;
        double _rotateZscaleX;
        double _rotateZscaleY;

        void PrepareRotate()
        {
            var    axisX  = Rotate.Rotate(1.0, 0.0, 0.0);
            var    axisY  = Rotate.Rotate(0.0, 1.0, 0.0);
            var    axisZ  = Rotate.Rotate(0.0, 0.0, 1.0);
            double axisXX = axisX.X0;
            double axisXY = axisX.Y0;
            double axisYX = axisY.X0;
            double axisYY = axisY.Y0;

            _rotateAngleX = Math.Atan2(axisXY, axisXX);
            _rotateAngleY = Math.Atan2(axisYY, axisYX);
            _rotateScaleX = Math.Sqrt(axisXX * axisXX + axisXY * axisXY);
            _rotateScaleY = Math.Sqrt(axisYX * axisYX + axisYY * axisYY);

            _rotateZscaleX = axisZ.X0;
            _rotateZscaleY = axisZ.Y0;
        }

        public Point3D FromClient(PointF pt, double z)
        {
            var    notRotated      = FromClient(pt);
            double notRotatedX     = notRotated.X0;
            double notRotatedY     = notRotated.Y0;
            double notRotatedAngel = Math.Atan2(notRotatedY, notRotatedX);
            double c               = Math.Sqrt(notRotatedX * notRotatedX + notRotatedY * notRotatedY);

            double anglec = Math.PI - (_rotateAngleY - _rotateAngleX);
            double anglea = _rotateAngleY - notRotatedAngel;
            double angleb = notRotatedAngel - _rotateAngleX;
            double rateC  = c / Math.Sin(anglec);

            double a = Math.Sin(anglea) * rateC;
            double b = Math.Sin(angleb) * rateC;

            return new Point3D(z * _rotateZscaleX + a / _rotateScaleX, z * _rotateZscaleY + b / _rotateScaleY, z);
        }

        PointF ToClientF(Point3D pt)
        {
            pt = _rotate3D.Rotate(pt);

            double x = ((pt.X0 - OffsetX) * Zoom) * _ratioX;
            double y = (((SizeY - (pt.Y0 + OffsetY))) * Zoom) * _ratioY;
            //double z = ((double)((double)(pt.Z ?? 0) - (double)OffsetZ) * Zoom) * _ratioZ;

            return new PointF((float) x, (float) y);
        }

        double ToClientSizeX(double X)
        {
            var pt3D = new Point3D(X, 0, 0);
            pt3D = Rotate.Rotate(pt3D);
            double x = pt3D.X0 * Zoom;
            return _ratioX * x;
        }

        double ToClientSizeY(double Y)
        {
            var pt3d = new Point3D(0, Y, 0);
            pt3d = Rotate.Rotate(pt3d);
            double y = pt3d.Y0 * Zoom;
            return _ratioY * y;
        }

        double ToClientSizeZ(double Z)
        {
            var pt3d = new Point3D(0, 0, Z);
            pt3d = Rotate.Rotate(pt3d);
            double z = pt3d.Z0 * Zoom;
            return _ratioZ * z;
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
                float fastSize = 0.5f;

                _helpLinePen   = new Pen(_helpLineColor, (float) (fastSize / 2.0));
                _helpLinePen10 = new Pen(_helpLineColor, (float) (fastSize * 4));

                InitPen(_selected, c => c);
                InitPen(_dithered, c => Color.FromArgb(25, c));

                _needReInit = false;
            }
        }

        void InitPen(PenSet set, Func<Color, Color> colorConverter)
        {
            float cutSize  = CutterSize > 0 ? (float) ToClientSizeX(CutterSize) : 2;
            float fastSize = 0.5f;

            set._cutPen = new Pen(colorConverter(CutColor), cutSize)
            {
                StartCap = LineCap.Round,
                EndCap   = LineCap.Round
            };

            set._cutDotPen     = new Pen(colorConverter(CutDotColor),     cutSize);
            set._cutEllipsePen = new Pen(colorConverter(CutEllipseColor), cutSize);
            set._cutArcPen = new Pen(colorConverter(CutArcColor), cutSize)
            {
                StartCap = LineCap.Round,
                EndCap   = LineCap.Round
            };

            set._cutPens = new[] { set._cutPen, set._cutDotPen, set._cutEllipsePen, set._cutArcPen };

            set._fastPen   = new Pen(colorConverter(FastMoveColor), fastSize);
            set._noMovePen = new Pen(colorConverter(Color.Blue),    fastSize);
            set._laserCutPen = new Pen(colorConverter(LaserOnColor), (float) ToClientSizeX(LaserSize))
            {
                StartCap = LineCap.Round,
                EndCap   = LineCap.Round
            };
            set._laserFastPen = new Pen(colorConverter(LaserOffColor), (float) (fastSize / 2.0));
        }

        public Bitmap DrawToBitmap(CommandList commands)
        {
            InitPen();

            var curBitmap = new Bitmap(RenderSize.Width, RenderSize.Height);

            Graphics g1 = Graphics.FromImage(curBitmap);
            g1.InterpolationMode  = InterpolationMode.NearestNeighbor;
            g1.SmoothingMode      = SmoothingMode.None;
            g1.PixelOffsetMode    = PixelOffsetMode.None;
            g1.CompositingQuality = CompositingQuality.HighSpeed;
            g1.TextRenderingHint  = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;

            var ee = new PaintEventArgs(g1, new Rectangle());

            var pts = new[]
            {
                ToClientF(new Point3D(0, 0, 0)), ToClientF(new Point3D(0, SizeY, 0)), ToClientF(new Point3D(SizeX, SizeY, 0)), ToClientF(new Point3D(SizeX, 0, 0))
            };
            g1.FillPolygon(new SolidBrush(MachineColor), pts);

            // draw axis

            g1.DrawLine(new Pen(Color.Red,   0.25f), ToClientF(new Point3D(-SizeX * 0.1, 0,            0)),            ToClientF(new Point3D(SizeX * 1.1, 0,           0)));
            g1.DrawLine(new Pen(Color.Green, 0.25f), ToClientF(new Point3D(0,            -SizeY * 0.1, 0)),            ToClientF(new Point3D(0,           SizeY * 1.1, 0)));
            g1.DrawLine(new Pen(Color.Blue,  0.25f), ToClientF(new Point3D(0,            0,            -SizeZ * 0.1)), ToClientF(new Point3D(0,           0,           SizeZ * 1.1)));

            for (int i = 1;; i++)
            {
                double x = i * 10.0;
                if (x > SizeX)
                {
                    break;
                }

                g1.DrawLine(((i % 10) == 0) ? _helpLinePen10 : _helpLinePen, ToClientF(new Point3D(i * 10.0, 0, 0)), ToClientF(new Point3D(i * 10.0, SizeY, 0)));
            }

            for (int i = 1;; i++)
            {
                double y = i * 10.0;
                if (y > SizeY)
                {
                    break;
                }

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
                if (_ratioX > _ratioY)
                {
                    _ratioX = _ratioY;
                }
                else if (_ratioX < _ratioY)
                {
                    _ratioY = _ratioX;
                }

                _ratioZ = _ratioX;
            }
        }

        #endregion

        #region IOutput 

        private class PenSet
        {
            public Pen   _noMovePen;
            public Pen   _cutEllipsePen;
            public Pen   _cutArcPen;
            public Pen   _cutPen;
            public Pen   _cutDotPen;
            public Pen[] _cutPens;
            public Pen   _fastPen;
            public Pen   _laserCutPen;
            public Pen   _laserFastPen;
        };

        public Pen _helpLinePen;
        public Pen _helpLinePen10;

        readonly PenSet _dithered = new PenSet();
        readonly PenSet _selected = new PenSet();

        public void DrawLine(Command cmd, object param, DrawType drawType, Point3D ptFrom, Point3D ptTo)
        {
            if (drawType == DrawType.NoDraw)
            {
                return;
            }

            var e = (PaintEventArgs) param;

            var from = ToClientF(ptFrom);
            var to   = ToClientF(ptTo);

            if (PreDrawLineOrArc(param, drawType, from, to))
            {
                e.Graphics.DrawLine(GetPen(drawType, LineDrawType.Line), from, to);
            }
        }

        void Line(object param, Pen pen, Point3D ptFrom, Point3D ptTo)
        {
            var e = (PaintEventArgs) param;

            var from = ToClientF(ptFrom);
            var to   = ToClientF(ptTo);

            if (from.Equals(to) == false)
            {
                e.Graphics.DrawLine(pen, from, to);
            }
        }

        public void DrawEllipse(Command cmd, object param, DrawType drawType, Point3D ptCenter, int xradius, int yradius)
        {
            if (drawType == DrawType.NoDraw)
            {
                return;
            }

            var e    = (PaintEventArgs) param;
            var from = ToClientF(ptCenter);
            e.Graphics.DrawEllipse(GetPen(drawType, LineDrawType.Ellipse), from.X - xradius / 2, from.Y - yradius / 2, xradius, yradius);
        }

        public void DrawArc(Command cmd, object param, DrawType drawType, Point3D ptFrom, Point3D ptTo, Point3D ptIIJ, bool clockwise, Pane pane)
        {
            if (drawType == DrawType.NoDraw)
            {
                return;
            }

            switch (pane)
            {
                default:
                case Pane.XYPane:
                    Arc(cmd, param, drawType, ptFrom, ptTo, ptIIJ.X0, ptIIJ.Y0, 0, 1, 2, clockwise);
                    break;
                case Pane.XZPane:
                    Arc(cmd, param, drawType, ptFrom, ptTo, ptIIJ.X0, ptIIJ.Z0, 0, 2, 1, clockwise);
                    break;
            }
        }

        const double ARCCORRECTION = (10.0 * Math.PI / 180.0); // every 10grad
        const double SEGMENTS_K    = 4.0;
        const double SEGMENTS_D    = 18.0; // 20 Grad 

        static double Hypot(double dx, double dy)
        {
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private void Arc(Command cmd, object param, DrawType drawType, Point3D ptFrom, Point3D ptTo, double offset0, double offset1, int axis_0, int axis_1, int axis_linear, bool isClockwise)
        {
            Pen pen = GetPen(drawType, LineDrawType.Arc);

            var    current      = new Point3D(ptFrom.X0, ptFrom.Y0, ptFrom.Z0);
            var    last         = new Point3D(ptFrom.X0, ptFrom.Y0, ptFrom.Z0);
            double center_axis0 = current[axis_0].Value + offset0;
            double center_axis1 = current[axis_1].Value + offset1;

            double linear_travel_max = (ptTo[axis_linear] ?? 0.0) - current[axis_linear].Value;
/*
			mm1000_t dist_linear[NUM_AXIS] = { 0 };

			for (axis_t x = 0; x<NUM_AXIS; x++)
			{
				if (x != axis_0 && x != axis_1)
				{
					dist_linear[x] = to[x] - current[x];
					if (dist_linear[x] > linear_travel_max)
						linear_travel_max = dist_linear[x];
				}
		}
*/
            double radius = Hypot(offset0, offset1);

            double r_axis0  = -offset0; // Radius vector from center to current location
            double r_axis1  = -offset1;
            double rt_axis0 = (ptTo[axis_0] ?? 0.0) - center_axis0;
            double rt_axis1 = (ptTo[axis_1] ?? 0.0) - center_axis1;

            // CCW angle between position and target from circle center. Only one atan2() trig computation required.
            double angular_travel = Math.Atan2(r_axis0 * rt_axis1 - r_axis1 * rt_axis0, r_axis0 * rt_axis0 + r_axis1 * rt_axis1);

            if (Math.Abs(angular_travel) < double.Epsilon)
            {
                // 360Grad
                if (isClockwise)
                {
                    angular_travel = -2.0 * Math.PI;
                }
                else
                {
                    angular_travel = 2.0 * Math.PI;
                }
            }
            else
            {
                if (angular_travel < 0.0)
                {
                    angular_travel += 2.0 * Math.PI;
                }

                if (isClockwise)
                {
                    angular_travel -= 2.0 * Math.PI;
                }
            }

            if (Hypot(angular_travel * radius, Math.Abs(linear_travel_max)) < 0.001)
            {
                return;
            }

            // difference to Grbl => use dynamic calculation of segements => suitable for small r
            //
            // segments for full circle => (CONST_K * r * M_PI * b + CONST_D)		(r in mm, b ...2?)

            uint segments = (uint) Math.Abs(Math.Floor(((2 * SEGMENTS_K * Math.PI)) * radius + SEGMENTS_D) * angular_travel / (2.0 * Math.PI));

            if (segments > 1)
            {
                double theta_per_segment = angular_travel / segments;

                int arc_correction = (int) (ARCCORRECTION / theta_per_segment);
                if (arc_correction < 0)
                {
                    arc_correction = -arc_correction;
                }

                // Vector rotation matrix values
                double cos_T = 1.0 - 0.5 * theta_per_segment * theta_per_segment;
                double sin_T = theta_per_segment;

                uint i;
                uint count = 0;

                for (i = 1; i < segments; i++)
                {
                    if (count < arc_correction)
                    {
                        // Apply vector rotation matrix 
                        double r_axisi = r_axis0 * sin_T + r_axis1 * cos_T;
                        r_axis0 = r_axis0 * cos_T - r_axis1 * sin_T;
                        r_axis1 = r_axisi;
                        count++;
                    }
                    else
                    {
                        // Arc correction to radius vector. Computed only every N_ARC_CORRECTION increments.
                        // Compute exact location by applying transformation matrix from initial radius vector(=-offset).
                        double cos_Ti = Math.Cos(i * theta_per_segment);
                        double sin_Ti = Math.Sin(i * theta_per_segment);
                        r_axis0 = -offset0 * cos_Ti + offset1 * sin_Ti;
                        r_axis1 = -offset0 * sin_Ti - offset1 * cos_Ti;
                        count   = 0;
                    }

                    // Update arc_target location

                    current[axis_0]      = center_axis0 + r_axis0;
                    current[axis_1]      = center_axis1 + r_axis1;
                    current[axis_linear] = (ptTo[axis_linear] ?? 0.0) - linear_travel_max * (segments - i) / segments;

                    Line(param, pen, last, current);

                    last.X = current.X;
                    last.Y = current.Y;
                    last.Z = current.Z;
                }
            }

            // Ensure last segment arrives at target location.
            Line(param, pen, last, ptTo);
        }

        private bool PreDrawLineOrArc(object param, DrawType drawType, PointF from, PointF to)
        {
            var e = (PaintEventArgs) param;

            if (from.Equals(to))
            {
                if ((drawType & DrawType.Laser) == DrawType.Laser)
                {
                    e.Graphics.DrawEllipse(GetPen(drawType, LineDrawType.Dot), @from.X, @from.Y, 1, 1);
                }
                else
                {
                    e.Graphics.DrawEllipse(GetPen(drawType, LineDrawType.Dot), @from.X, @from.Y, 4, 4);
                }

                return false;
            }

            return true;
        }

        static double ConvertRadToDeg(double rad)
        {
            double deg = -rad * 180.0 / Math.PI + 180.0;
            while (deg < 0)
            {
                deg += 360;
            }

            while (deg > 360)
            {
                deg -= 360;
            }

            return deg;
        }

        enum LineDrawType
        {
            Line    = 0,
            Dot     = 1,
            Ellipse = 2,
            Arc     = 3
        }

        private Pen GetPen(DrawType moveType, LineDrawType drawType)
        {
            PenSet set = (moveType & DrawType.Selected) == DrawType.Selected ? _selected : _dithered;

            if ((moveType & DrawType.Draw) == 0)
            {
                return set._noMovePen;
            }

            bool isCut   = (moveType & DrawType.Cut) == DrawType.Cut;
            bool isLaser = (moveType & DrawType.Laser) == DrawType.Laser;

            if (isLaser)
            {
                return isCut ? set._laserCutPen : set._laserFastPen;
            }

            return isCut ? set._cutPens[(int) drawType] : set._fastPen;
        }

        #endregion
    }
}