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

using Framework.Tools;
using CNCLib.GCode.Load;
using CNCLib.GCode.Commands;
using CNCLib.GCode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using CNCLib.GUI.Load;
using CNCLib.Arduino;

namespace CNCLib.GUI
{
    public partial class PaintForm : Form
	{

		#region Crt

		public PaintForm()
		{
			InitializeComponent();
			SetMachineSize();

			new CNCLib.GCode.Commands.CommandFactory();
		}

		public void SetMachineSize()
		{
			_plotterCtrl.SizeX = Settings.Instance.SizeX;
			_plotterCtrl.SizeY = Settings.Instance.SizeY;
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
			_home.Enabled = enable;
			_paintfrom.Enabled = enable;
			_paintSelected.Enabled = enable;
			_abort.Enabled = !enable;
			_paintfrom.Enabled = enable;
		}

		#endregion

		#region Event

		private void _abort_Click(object sender, EventArgs e)
		{
			Com.AbortCommands();
		}

		private void _sendTo_Click(object sender, EventArgs e)
        {
			AsyncRunCommand(() =>
			{
				List<String> commands = new List<string>();
				Command last = null;
				foreach (Command r in _plotterCtrl.Commands)
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
        private void _paintfrom_Click(object sender, EventArgs e)
        {
			AsyncRunCommand(() =>
			{
				List<String> commands = new List<string>();
				int idx = 0;
				int count = int.Parse(_paintcount.Text);
				Command last = null;

				foreach (Command r in _plotterCtrl.Commands)
				{
					if (_plotterCtrl.SelectedCommand <= idx && count > 0)
					{
						string[] cmds = r.GetGCodeCommands(last != null ? last.CalculatedEndPosition : null);
						if (cmds != null)
						{
							commands.AddRange(cmds);
						}
						count--;
					}
					idx++;
					last = r;
				}

				Com.SendCommands(commands.ToArray());
			});
        }
        private void _paintSelected_Click(object sender, EventArgs e)
        {
			AsyncRunCommand(() =>
			{
				List<String> commands = new List<string>();
				int idx = 0;
				Command last = null;
				foreach (Command r in _plotterCtrl.Commands)
				{
					if (_plotterCtrl.SelectedCommand == idx)
					{
						string[] cmds = r.GetGCodeCommands(last != null ? last.CalculatedEndPosition : null);
						if (cmds != null)
						{
							commands.AddRange(cmds);
						}
					}
					idx++;
					last = r;
				}

				Com.SendCommands(commands.ToArray());
				SetSelShape(_plotterCtrl.SelectedCommand + 1);
			});
        }

        private void _home_Click(object sender, EventArgs e)
        {
			AsyncRunCommand(() =>
			{
				Com.SendCommand("g28");
			});
        }

        private void _save_Click(object sender, EventArgs e)
        {
            using (StreamWriter sw = new StreamWriter(@"c:\tmp\test.GCode"))
            {
				Command last = null;
				foreach (Command r in _plotterCtrl.Commands)
                {
					string[] cmds = r.GetGCodeCommands(last != null ? last.CalculatedEndPosition : null);
                    if (cmds != null)
                    {
                        foreach (String str in cmds)
                        {
                            sw.WriteLine(str);
                        }
                    }
					last = r;
				}
			}
        }

		#endregion

		LoadInfo loadinfo = new LoadInfo();

        private void _load_Click(object sender, EventArgs e)
        {
			loadinfo.AutoScaleSizeX = _plotterCtrl.SizeX;
			loadinfo.AutoScaleSizeY = _plotterCtrl.SizeY;

            using (LoadOptionForm form = new LoadOptionForm())
            {
                form.LoadInfo = loadinfo;

				DialogResult res = form.ShowDialog();

				if (res == DialogResult.OK)
                {
                    loadinfo = form.LoadInfo;
                    LoadHPGL load = new LoadHPGL();
                    load.LoadOptions = loadinfo;
                    load.LoadHPLG(_plotterCtrl.Commands);
					_redraw_Click(null, null);
                }
				else if (res == DialogResult.Yes)
				{
					loadinfo = form.LoadInfo;
					LoadGCode load = new LoadGCode();
					load.LoadOptions = loadinfo;
					try
					{
						load.Load(_plotterCtrl.Commands);
					}
					catch (Exception ex)
					{
						MessageBox.Show("Load Failed! " + ex.Message);
					}
					_redraw_Click(null, null);
				}
			}
        }

        private void SetSelShape(int idx)
        {
            if (idx >= 0 && idx < _plotterCtrl.Commands.Count)
            {
                _plotterCtrl.SelectedCommand = idx;
                _selidxlbl.Text = idx.ToString();
            }
        }

        private void _selPrev_Click(object sender, EventArgs e)
        {
            SetSelShape(_plotterCtrl.SelectedCommand-1);
        }

        private void _selNext_Click(object sender, EventArgs e)
        {
            SetSelShape(_plotterCtrl.SelectedCommand + 1);
        }
        private void _selFirst_Click(object sender, EventArgs e)
        {
            SetSelShape(0);
        }

        private void _selLast_Click(object sender, EventArgs e)
        {
            SetSelShape(_plotterCtrl.Commands.Count - 1);
        }

		private void _redraw_Click(object sender, EventArgs e)
		{
			decimal val;
			if (decimal.TryParse(_zoom.Text, out val))
			{
				_plotterCtrl.Zoom = val;
			}
			if (decimal.TryParse(_offsetX.Text, out val))
			{
				_plotterCtrl.OffsetX = val;
			}
			if (decimal.TryParse(_offsetY.Text, out val))
			{
				_plotterCtrl.OffsetY = val;
			}
		}

		private void _plotterCtrl_GCodeMousePosition(object o, GCoderUserControlEventArgs info)
		{
			_coord.Text = info.GCodePosition.X.ToString() + " : " + info.GCodePosition.Y.ToString();
		}

		private void PaintForm_Load(object sender, EventArgs e)
		{
			EnableComControls(Com.IsConnected);
		}
    }
}
