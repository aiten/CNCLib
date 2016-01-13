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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CNCLib.GCode;
using Framework.Tools.Drawing;
using CNCLib.GCode.Commands;
using Framework.Arduino;
using System.Drawing.Drawing2D;

namespace CNCLib.GUI
{
	public partial class GCodeUserControl : UserControl , IOutputCommand
	{
		#region crt

		public GCodeUserControl()
		{
			SizeX = 130.000m;
			SizeY = 45.000m;
			OffsetX = 0;
			OffsetY = 0;

            InitializeComponent();

            SetStyle(ControlStyles.DoubleBuffer, true);
        }

        #endregion

        #region Properties

        public decimal SizeX { get; set; }
		public decimal SizeY { get; set; }

		public decimal Zoom { get { return _zoom; } set { _zoom = value; Invalidate(); } }
		public decimal OffsetX { get { return _offsetX; } set { _offsetX = value; Invalidate(); } }
		public decimal OffsetY { get { return _offsetY; } set { _offsetY = value; Invalidate(); } }

		public CommandList Commands { get { return _commands; } }

		public delegate void GCodeEventHandler(object o, GCoderUserControlEventArgs info);

		public event GCodeEventHandler GCodeMousePosition;
        public event GCodeEventHandler ZoomOffsetChanged;

        #endregion

        #region private Members

        decimal _zoom = 1;
		decimal _offsetX = 0;
		decimal _offsetY = 0;

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

		Point3D FromClient(Point pt)
		{
			// with e.g.  867
			// max pt.X = 686 , pt.x can be 0
			return new Point3D(
							AdjustX(Tools.MulDiv(SizeX, pt.X, ClientSize.Width - 1, 3))/Zoom,
							AdjustY(Tools.MulDiv(SizeY, pt.Y, ClientSize.Height - 1, 3) / Zoom),
							0);
		}

		Point ToClient(Point3D pt)
		{
			decimal ptx = pt.X.HasValue ? pt.X.Value : 0;
			decimal pty = pt.Y.HasValue ? pt.Y.Value : 0;
			decimal x = (ptx - OffsetX) * Zoom;
			decimal y = (SizeY - (pty + OffsetY)) * Zoom;
			return new Point((int)Tools.MulDiv(x, ClientSize.Width, SizeX, 0), (int)Tools.MulDiv(y, ClientSize.Height, SizeY, 0));
		}

        #endregion

        #region Drag/Drop

        private bool _isdragging = false;
        private Point3D _mouseDown;
        private decimal _mouseDownOffsetX;
        private decimal _mouseDownOffsetY;

        private void PlotterUserControl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                _zoom *= 1.1m;
            else
                _zoom /= 1.1m;

            OnZoomOffsetChanged();
            Invalidate();
        }

        private void PlotterUserControl_MouseDown(object sender, MouseEventArgs e)
        {
            {
                if (!_isdragging)
                {
                    _mouseDown = FromClient(e.Location);
                    _mouseDownOffsetX = _offsetX;
                    _mouseDownOffsetY = _offsetY;
                }
                _isdragging = true;
            }
        }

        private void PlotterUserControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (GCodeMousePosition != null)
			{
				GCodeMousePosition(this, new GCoderUserControlEventArgs() { GCodePosition = FromClient(e.Location) });
			}
            if (_isdragging)
            {
                _offsetX = _mouseDownOffsetX;
                _offsetY = _mouseDownOffsetY;
                Point3D c = FromClient(e.Location);
                decimal newX = _mouseDownOffsetX - (c.X.Value - _mouseDown.X.Value);
                decimal newY = _mouseDownOffsetY + (c.Y.Value - _mouseDown.Y.Value);
                _offsetX = newX;
                _offsetY = newY;
                OnZoomOffsetChanged();
            }
        }

        private void PlotterUserControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isdragging)
            {
                Invalidate();
                OnZoomOffsetChanged();
            }
            _isdragging = false;
        }

        private void OnZoomOffsetChanged()
        {
            if (ZoomOffsetChanged != null)
            {
                ZoomOffsetChanged(this, new GCoderUserControlEventArgs() );
            }
        }

        #endregion

        #region private

        private void PlotterUserControl_Paint(object sender, PaintEventArgs e)
		{
            if (_normalLine == null)
               _normalLine = new Pen(BackColor == Color.White ? Color.Black : Color.White, 2);

            //Create a Bitmap object with the size of the form
            Bitmap curBitmap = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            //Create a temporary Graphics object from the bitmap
            Graphics g1 = Graphics.FromImage(curBitmap);
            g1.InterpolationMode = InterpolationMode.NearestNeighbor;
            g1.SmoothingMode = SmoothingMode.None;
            g1.PixelOffsetMode = PixelOffsetMode.None;
            g1.CompositingQuality = CompositingQuality.HighSpeed;
            g1.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;

            //Draw lines on the temporary Graphics object

            var ee = new PaintEventArgs(g1, new Rectangle());
            _commands.Paint(this, ee);

            //Call DrawImage of Graphics and draw bitmap
            e.Graphics.DrawImage(curBitmap, 0, 0);
            //Dispose of objects
            g1.Dispose();
            curBitmap.Dispose();
       }

        private Size _lastsize;
		private void PlotterUserControl_Resize(object sender, EventArgs e)
		{
			if (_lastsize.Height != 0 && Size.Width > 0 && Size.Height > 0)
			{
				//RecalcClientCoord();
			}
			_lastsize = Size;
			Invalidate();
		}

        #endregion


        #region IOutput 

        Pen _normalLine; // = new Pen(Color.White, 2);
		Pen _falseLine  = new Pen(Color.Green, 1);
		Pen _NoMove		= new Pen(Color.Blue, 1);
        Pen _lasernormalLine = new Pen(Color.Red, 2);
        Pen _laserfalseLine = new Pen(Color.Orange, 1);

        public void DrawLine(Command cmd, object param, DrawType drawtype, Point3D ptFrom, Point3D ptTo)
		{
            if (drawtype == DrawType.NoDraw) return;

			PaintEventArgs e = (PaintEventArgs) param;

			Point from = ToClient(ptFrom);
			Point to   = ToClient(ptTo);

			if (from.Equals(to))
			{
				e.Graphics.DrawEllipse(GetPen(drawtype), from.X, from.Y, 4, 4);
			}
			else
			{
				e.Graphics.DrawLine(GetPen(drawtype), from, to);
			}
		}
		public void DrawEllipse(Command cmd, object param, DrawType drawtype, Point3D ptFrom, int xradius, int yradius)
		{
            if (drawtype == DrawType.NoDraw) return;

			PaintEventArgs e = (PaintEventArgs)param;
			Point from = ToClient(ptFrom);
			e.Graphics.DrawEllipse(GetPen(drawtype), from.X, from.Y, xradius, yradius);
		}

		private Pen GetPen(DrawType moveType)
		{
			switch (moveType)
			{
				default:
				case DrawType.NoMove: return _NoMove;
				case DrawType.Fast: return _falseLine;
				case DrawType.Normal: return _normalLine;
                case DrawType.LaserFast: return _laserfalseLine;
                case DrawType.LaserNormal: return _lasernormalLine;
            }
        }

		#endregion	

	}
}
