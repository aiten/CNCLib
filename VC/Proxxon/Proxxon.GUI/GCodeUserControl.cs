////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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
using Proxxon.GCode;
using Framework.Tools;
using Proxxon.GCode.Commands;
using Framework.Logic;

namespace Proxxon.GUI
{
	#region EventArg

	public class GCoderUserControlEventArgs : EventArgs
	{
		public GCoderUserControlEventArgs()
		{
		}
		public SpaceCoordinate GCodePosition { get; set; }
	}

	#endregion

	public partial class GCodeUserControl : UserControl , IOutputCommand
	{
		#region crt

		public GCodeUserControl()
		{
			SizeX = 130.000m;
			SizeY = 45.000m;
			OffsetX = 0;
			OffsetY = 0;

			SelectedLinesize = 1;
			SelectedColor = Color.Black;

			SelectedCommand = -1;

			InitializeComponent();
		}

		#endregion

		#region Properties

		public decimal SizeX { get; set; }
		public decimal SizeY { get; set; }

		public decimal Zoom { get { return _zoom; } set { _zoom = value; Invalidate(); } }
		public decimal OffsetX { get { return _offsetX; } set { _offsetX = value; Invalidate(); } }
		public decimal OffsetY { get { return _offsetY; } set { _offsetY = value; Invalidate(); } }

		public CommandList Commands { get { return _commands; } }

		public int SelectedLinesize { get; set; }

		public Color SelectedColor { get; set; }

		public int SelectedCommand
		{
			get { return _selectedCommand >= Commands.Count ? -1 : _selectedCommand; }
			set
			{
				_selectedCommand = value;
				if (_selectedCommand >= Commands.Count)
					_selectedCommand = Commands.Count - 1;
				if (_selectedCommand < -1)
					_selectedCommand = -1;
				Invalidate();
			}
		}

		public delegate void GCodeEventHandler(object o, GCoderUserControlEventArgs info);

		public event GCodeEventHandler GCodeMousePosition;

		#endregion

		#region private Members

		int _selectedCommand = -1;
		decimal _zoom = 1;
		decimal _offsetX = 0;
		decimal _offsetY = 0;

		CommandList _commands = new CommandList();

		private ArduinoSerialCommunication Com
		{
			get { return Framework.Tools.Singleton<ArduinoSerialCommunication>.Instance; }
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

		SpaceCoordinate FromClient(Point pt)
		{
			// with e.g.  867
			// max pt.X = 686 , pt.x can be 0
			return new SpaceCoordinate(
							AdjustX(Tools.MulDiv(SizeX, pt.X, ClientSize.Width - 1, 3)),
							AdjustY(Tools.MulDiv(SizeY, pt.Y, ClientSize.Height - 1, 3)),
							0);
		}

		Point ToClient(SpaceCoordinate pt)
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
		private SpaceCoordinate _mouseDown;
		private decimal _mouseDownOffsetX;
		private decimal _mouseDownOffsetY;

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
				SpaceCoordinate c = FromClient(e.Location);
				decimal newX = _mouseDownOffsetX - (c.X.Value - _mouseDown.X.Value);
				decimal newY = _mouseDownOffsetY + (c.Y.Value - _mouseDown.Y.Value);
				_offsetX = newX; _offsetY = newY;
				//RecalcClientCoord();
			}
		}

		private void PlotterUserControl_MouseUp(object sender, MouseEventArgs e)
		{
			if (_isdragging)
			{
				Invalidate();
			}
			_isdragging = false;
		}

		#endregion

		#region private

		private void PlotterUserControl_Paint(object sender, PaintEventArgs e)
		{
			_commands.Paint(this, e);
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

		Pen _normalLine = new Pen(Color.Black, 3);
		Pen _falseLine  = new Pen(Color.Black, 1);
		Pen _NoMove		= new Pen(Color.Blue, 1);

		public void DrawLine(Command cmd, object param, Command.MoveType movetype, SpaceCoordinate ptFrom, SpaceCoordinate ptTo)
		{
			PaintEventArgs e = (PaintEventArgs) param;

			Point from = ToClient(ptFrom);
			Point to   = ToClient(ptTo);


			if (from.Equals(to))
			{
				e.Graphics.DrawEllipse(GetPen(movetype), from.X, from.Y, 4, 4);
			}
			else
			{
				e.Graphics.DrawLine(GetPen(movetype), from, to);
			}
		}
		public void DrawEllipse(Command cmd, object param, Command.MoveType movetype, SpaceCoordinate ptFrom, int xradius, int yradius)
		{
			PaintEventArgs e = (PaintEventArgs)param;
			Point from = ToClient(ptFrom);
			e.Graphics.DrawEllipse(GetPen(movetype), from.X, from.Y, xradius, yradius);
		}

		private Pen GetPen(Command.MoveType moveType)
		{
			switch (moveType)
			{
				default:
				case Command.MoveType.NoMove: return _NoMove;
				case Command.MoveType.Fast: return _falseLine;
				case Command.MoveType.Normal: return _normalLine;
			}
		}

		#endregion	

	}
}
