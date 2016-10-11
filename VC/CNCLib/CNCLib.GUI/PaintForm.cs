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

using CNCLib.GCode.Commands;
using CNCLib.GCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using CNCLib.GUI.Load;
using Framework.Arduino;
using CNCLib.Logic.Contracts.DTO;

namespace CNCLib.GUI
{
	public partial class PaintForm : Form
	{
		#region Crt

		public PaintForm()
		{
			InitializeComponent();
			SetMachineSize();
            ValuesFromControl();

            _laserOnColor.Color = _gCodeCtrl.LaserOnColor;
			_laserOffColor.Color = _gCodeCtrl.LaserOffColor;

			new CNCLib.GCode.Commands.CommandFactory();
		}

		public void SetMachineSize()
		{
			_gCodeCtrl.SizeX = Settings.Instance.SizeX;
			_gCodeCtrl.SizeY = Settings.Instance.SizeY;
            EnableComControls(Com.IsConnected);
        }

        #endregion

        #region Properties

        private ArduinoSerialCommunication Com
        {
			get { return Framework.Tools.Pattern.Singleton<ArduinoSerialCommunication>.Instance; }
        }

		#endregion

		#region Utilities

		delegate void ExecuteCommands();

		void AsyncRunCommand(ExecuteCommands todo)
		{
			new Thread(() =>
			{
				Invoke(new MethodInvoker(() => 
				{
					EnableComControls(false);
				}));

				try
				{
					Com.ClearCommandHistory();

					todo();

					Com.WriteCommandHistory(@"c:\tmp\Command.txt");
				}
				finally
				{
					Invoke(new MethodInvoker(() => 
					{
						EnableComControls(true);
					}));
				}
			}
			).Start();
		}

		private void EnableComControls(bool enable)
		{
			_sendTo.Enabled = enable;
		}

		#endregion

		#region Event

		private void _sendTo_Click(object sender, EventArgs e)
        {
			AsyncRunCommand(() =>
			{
				List<String> commands = new List<string>();
				Command last = null;
				foreach (Command r in _gCodeCtrl.Commands)
				{
					string[] cmds = r.GetGCodeCommands(last != null ? last.CalculatedEndPosition : null);
					if (cmds != null)
					{
						commands.AddRange(cmds);
					}
					last = r;
				}
				Com.SendCommands(commands.ToArray());
			});
        }

        #endregion

			LoadOptions loadinfo = new LoadOptions();
			bool _useAzure = false;

			private async void _load_Click(object sender, EventArgs e)
			{
				if (loadinfo.AutoScaleSizeX == 0 || loadinfo.AutoScale == false)
				{
					loadinfo.AutoScaleSizeX = _gCodeCtrl.SizeX;
					loadinfo.AutoScaleSizeY = _gCodeCtrl.SizeY;
				}

				using (LoadOptionForm form = new LoadOptionForm())
				{
					form.LoadInfo = loadinfo;
					form.UseAzure = _useAzure;

					if (form.ShowDialog() == DialogResult.OK)
					{
						loadinfo = form.LoadInfo;
						_useAzure = form.UseAzure;

						var ld = new GCodeLoad();

						try
						{
							_load.Enabled = false;
							CommandList commands = await ld.Load(loadinfo, _useAzure);
							_gCodeCtrl.Commands.Clear();
							_gCodeCtrl.Commands.AddRange(commands);
						}
						catch (Exception ex)
						{
							throw;
						}
						finally
						{
							_load.Enabled = true;
							_redraw_Click(null, null);
						}
					}
				}
			}

		private void ValuesFromControl()
        {
            _offsetX.Text = _gCodeCtrl.OffsetX.ToString();
            _offsetY.Text = _gCodeCtrl.OffsetY.ToString();
            _zoom.Text = _gCodeCtrl.Zoom.ToString();
            _cutterSize.Text = _gCodeCtrl.CutterSize.ToString();
            _laserSize.Text = _gCodeCtrl.LaserSize.ToString();
        }

        private void ValuesToControl()
        {
            decimal valDec;
            double valDbl;
            if (double.TryParse(_zoom.Text, out valDbl))
            {
                _gCodeCtrl.Zoom = valDbl;
            }
            if (decimal.TryParse(_offsetX.Text, out valDec))
            {
                _gCodeCtrl.OffsetX = valDec;
            }
            if (decimal.TryParse(_offsetY.Text, out valDec))
            {
                _gCodeCtrl.OffsetY = valDec;
            }
            if (double.TryParse(_laserSize.Text, out valDbl))
            {
                _gCodeCtrl.LaserSize = valDbl;
            }
            if (double.TryParse(_cutterSize.Text, out valDbl))
            {
                _gCodeCtrl.CutterSize = valDbl;
            }
        }
        private void _redraw_Click(object sender, EventArgs e)
        {
            ValuesToControl();
        }

        private void _plotterCtrl_GCodeMousePosition(object o, GCoderUserControlEventArgs info)
		{
			_coord.Text = decimal.Round(info.GCodePosition.X.Value,3).ToString() + " : " + decimal.Round(info.GCodePosition.Y.Value,3).ToString();
		}

		private void PaintForm_Load(object sender, EventArgs e)
		{
			EnableComControls(Com.IsConnected);
		}

        private void _zoomOut_Click(object sender, EventArgs e)
        {
            _gCodeCtrl.Zoom = _gCodeCtrl.Zoom * 0.9;
            _zoom.Text = _gCodeCtrl.Zoom.ToString();
        }

        private void _zoomIn_Click(object sender, EventArgs e)
        {
            _gCodeCtrl.Zoom = _gCodeCtrl.Zoom / 0.9;
            _zoom.Text = _gCodeCtrl.Zoom.ToString();
        }

        private void _ofsXMin_Click(object sender, EventArgs e)
        {
            _gCodeCtrl.OffsetX = _gCodeCtrl.OffsetX - 10;
            _offsetX.Text = _gCodeCtrl.OffsetX.ToString();
        }

        private void _ofsXPlus_Click(object sender, EventArgs e)
        {
            _gCodeCtrl.OffsetX = _gCodeCtrl.OffsetX + 10;
            _offsetX.Text = _gCodeCtrl.OffsetX.ToString();
        }

        private void _ofsYMin_Click(object sender, EventArgs e)
        {
            _gCodeCtrl.OffsetY = _gCodeCtrl.OffsetY - 10;
            _offsetY.Text = _gCodeCtrl.OffsetY.ToString();
        }

        private void _ofsYPlus_Click(object sender, EventArgs e)
        {
            _gCodeCtrl.OffsetY = _gCodeCtrl.OffsetY + 10;
            _offsetY.Text = _gCodeCtrl.OffsetY.ToString();
        }

        private void _gCodeCtrl_ZoomOffsetChanged(object o, GCoderUserControlEventArgs info)
        {
            ValuesFromControl();
        }

        private void _machineColor_SelectedIndexChanged(object sender, EventArgs e)
		{
			_gCodeCtrl.MachineColor = _colorCB.Color;
		}
        private void _laserOnColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            _gCodeCtrl.LaserOnColor = _laserOnColor.Color;
        }
		private void _laserOffColor_SelectedIndexChanged(object sender, EventArgs e)
		{
			_gCodeCtrl.LaserOffColor = _laserOffColor.Color;
		}

		private void CheckKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                ValuesToControl();
                e.Handled = true;
            }
        }
        private void _zoom_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckKeyPress(e);
        }

        private void _offsetX_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckKeyPress(e);
        }

        private void _offsetY_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckKeyPress(e);
        }

        private void _laserSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckKeyPress(e);
        }

        private void _cutterSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckKeyPress(e);
        }
    }
}
