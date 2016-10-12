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
using System.Diagnostics;

namespace CNCLib.GUI
{
	public partial class GCodeUserControl : UserControl
	{
		#region crt

		public GCodeUserControl()
		{
            InitializeComponent();
            SetStyle(ControlStyles.DoubleBuffer, true);
        }

        #endregion

        #region Properties

        public decimal SizeX { get { return _bitmapDraw.SizeX; } set { _bitmapDraw.SizeX = value; ReInitDraw(); } }
        public decimal SizeY { get { return _bitmapDraw.SizeY; } set { _bitmapDraw.SizeY = value; ReInitDraw(); } }

        public bool KeepRatio { get { return _bitmapDraw.KeepRatio; } set { _bitmapDraw.KeepRatio = value; ReInitDraw(); } }

        public double Zoom { get { return _bitmapDraw.Zoom; } set { _bitmapDraw.Zoom = value; ReInitDraw(); } }
		public decimal OffsetX { get { return _bitmapDraw.OffsetX; } set { _bitmapDraw.OffsetX = value; ReInitDraw(); } }
		public decimal OffsetY { get { return _bitmapDraw.OffsetY; } set { _bitmapDraw.OffsetY = value; ReInitDraw(); } }
        public decimal CutterSize { get { return _bitmapDraw.CutterSize; } set { _bitmapDraw.CutterSize = value; ReInitDraw(); } }
        public decimal LaserSize { get { return _bitmapDraw.LaserSize; } set { _bitmapDraw.LaserSize = value; ReInitDraw(); } }

        public Color MachineColor { get { return _bitmapDraw.MachineColor; } set { _bitmapDraw.MachineColor = value; ReInitDraw(); } }
        public Color LaserOnColor { get { return _bitmapDraw.LaserOnColor; } set { _bitmapDraw.LaserOnColor = value; ReInitDraw(); } }
		public Color LaserOffColor { get { return _bitmapDraw.LaserOffColor; } set { _bitmapDraw.LaserOffColor = value; ReInitDraw(); } }

		public CommandList Commands { get { return _bitmapDraw.Commands; } }

		public delegate void GCodeEventHandler(object sender, GCoderUserControlEventArgs e);

		public event GCodeEventHandler GCodeMousePosition;
        public event GCodeEventHandler ZoomOffsetChanged;

		#endregion

		#region private Members

		private GCodeBitmapDraw _bitmapDraw = new GCodeBitmapDraw();

		private ArduinoSerialCommunication Com
		{
			get { return Framework.Tools.Pattern.Singleton<ArduinoSerialCommunication>.Instance; }
		}

		#endregion

        #region Drag/Drop

        private bool _isdragging = false;
        private Point3D _mouseDown;
        private decimal _mouseDownOffsetX;
        private decimal _mouseDownOffsetY;
        private Stopwatch _sw = new Stopwatch();

        private void GCodeUserControl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                Zoom *= 1.1;
            else
                Zoom /= 1.1;

            OnZoomOffsetChanged();
            Invalidate();
        }

        private void GCodeUserControl_MouseDown(object sender, MouseEventArgs e)
        {
            {
                if (!_isdragging)
                {
                    _mouseDown = _bitmapDraw.FromClient(e.Location);
                    _mouseDownOffsetX = OffsetX;
                    _mouseDownOffsetY = OffsetY;
                    _sw.Start();
                }
                _isdragging = true;
            }
        }

        private void GCodeUserControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (GCodeMousePosition != null)
			{
				GCodeMousePosition(this, new GCoderUserControlEventArgs() { GCodePosition = _bitmapDraw.FromClient(e.Location) });
			}
            if (_isdragging)
            {
                OffsetX = _mouseDownOffsetX;
                OffsetY = _mouseDownOffsetY;
                Point3D c = _bitmapDraw.FromClient(e.Location);
                decimal newX = _mouseDownOffsetX - (c.X.Value - _mouseDown.X.Value);
                decimal newY = _mouseDownOffsetY + (c.Y.Value - _mouseDown.Y.Value);
                OffsetX = newX;
                OffsetY = newY;
                OnZoomOffsetChanged();
                if (_sw.ElapsedMilliseconds > 300)
                {
                    _sw.Start();
                    Invalidate();
                }
            }
        }

        private void GCodeUserControl_MouseUp(object sender, MouseEventArgs e)
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

        private void ReInitDraw()
        {
            Invalidate();
        }

        private void GCodeUserControl_Paint(object sender, PaintEventArgs e)
		{
			if (_bitmapDraw.RenderSize.Height == 0 || _bitmapDraw.RenderSize.Width == 0)
				return;

			var curBitmap = _bitmapDraw.DrawToBitmap();

            e.Graphics.DrawImage(curBitmap, 0, 0);
            curBitmap.Dispose();
       }

		private void GCodeUserControl_Resize(object sender, EventArgs e)
		{
			_bitmapDraw.RenderSize = Size;
            Invalidate();
		}

        #endregion
	}
}
