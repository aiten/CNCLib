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
using Plotter.GUI.Load;
using Plotter.GUI.Shapes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Framework.Arduino.SerialCommunication;

namespace Plotter.GUI
{
    public partial class PaintForm : Form
	{

		#region crt / properties

		public PaintForm()
        {
            InitializeComponent();
        }
        private HPGLSerial Com
        {
            get { return Framework.Tools.Pattern.Singleton<HPGLSerial>.Instance; }
        }

		public int SizeXHPGL { get { return _plotterCtrl.SizeXHPGL; } set { _plotterCtrl.SizeXHPGL = value;  } }
		public int SizeYHPGL { get { return _plotterCtrl.SizeYHPGL; } set { _plotterCtrl.SizeYHPGL = value; } }

		#endregion

		#region Communication

		delegate void ExecuteCommands();

		void AsyncRunCommand(ExecuteCommands todo)
		{
			if (Com.IsConnected)
			{
				new Task(() =>
				{
					Invoke(new MethodInvoker(() =>
					{
						EnableComControls(false);
						_plotterCtrl.ReadOnly = true;
						_load.Enabled = false;
						_clear.Enabled = false;
						_undo.Enabled = false;
						_del.Enabled = false;
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
							_load.Enabled = false;
							_plotterCtrl.ReadOnly = false;
							_clear.Enabled = true;
							_undo.Enabled = true;
							_del.Enabled = true;
						}));
					}
				}
				).Start();
			}
		}

		private void EnableComControls(bool enable)
		{
			_paintAgain.Enabled = enable;
			_home.Enabled = enable;
			_paintfrom.Enabled = enable;
			_paintSelected.Enabled = enable;
			_abort.Enabled = !enable;
			_paintfrom.Enabled = enable;
			_plot.Enabled = enable;
		}

		#endregion

		#region Communication Events

		private void _abort_Click(object sender, EventArgs e)
		{
			Com.AbortCommands();
		}

        private void _paintAgain_Click(object sender, EventArgs e)
        {
			AsyncRunCommand(() =>
			{
				List<String> commands = new List<string>();
				foreach (Shape r in _plotterCtrl.Shapes)
				{
					string[] cmds = r.GetHPGLCommands();
					if (cmds != null)
					{
						commands.AddRange(cmds);
					}
				}
				Com.SendCommandsAsync(commands.ToArray());
			});
        }
        private void _paintfrom_Click(object sender, EventArgs e)
        {
			AsyncRunCommand(() =>
			{
				List<String> commands = new List<string>();
				int idx = 0;
				int count = int.Parse(_paintcount.Text);

				foreach (Shape r in _plotterCtrl.Shapes)
				{
					if (_plotterCtrl.SelectedShape <= idx && count > 0)
					{
						string[] cmds = r.GetHPGLCommands();
						if (cmds != null)
						{
							commands.AddRange(cmds);
						}
						count--;
					}
					idx++;
				}
	            Com.SendCommandsAsync(commands.ToArray());
			});
		}
        private void _paintSelected_Click(object sender, EventArgs e)
        {
			AsyncRunCommand(() =>
			{
				List<String> commands = new List<string>();
				int idx = 0;
				foreach (Shape r in _plotterCtrl.Shapes)
				{
					if (_plotterCtrl.SelectedShape == idx)
					{
						string[] cmds = r.GetHPGLCommands();
						if (cmds != null)
						{
							commands.AddRange(cmds);
						}
					}
					idx++;
				}
		        Com.SendCommandsAsync(commands.ToArray());
	            SetSelShape(_plotterCtrl.SelectedShape + 1);
			});
		}

        private void _clear_Click(object sender, EventArgs e)
        {
            _plotterCtrl.Shapes.Clear();
            _plotterCtrl.Invalidate();
        }

        private void _home_Click(object sender, EventArgs e)
        {
			AsyncRunCommand(() =>
			{
	            Com.SendCommand("PU 0 0");
			});
		}

		#endregion

		#region Load/Save

		static string _fileNameSave = @"c:\tmp\testc.hpgl";

		private void _save_Click(object sender, EventArgs e)
        {
			using (SaveFileDialog form = new SaveFileDialog())
			{
				form.FileName = _fileNameSave;
				if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					_fileNameSave = form.FileName;
				}
			}

			using (StreamWriter sw = new StreamWriter(_fileNameSave))
            {
                foreach (Shape r in _plotterCtrl.Shapes)
                {
                    string[] cmds = r.GetHPGLCommands();
                    if (cmds != null)
                    {
                        foreach (String str in cmds)
                        {
                            sw.WriteLine(str);
                        }
                    }
                }
            }
        }

        Load.Load.LoadInfo loadinfo = new Load.Load.LoadInfo();

        private void _load_Click(object sender, EventArgs e)
        {
            using (LoadOptionForm form = new LoadOptionForm())
            {
				loadinfo.AutoScaleSizeX = _plotterCtrl.SizeXHPGL;
				loadinfo.AutoScaleSizeY = _plotterCtrl.SizeYHPGL;

                form.LoadInfo = loadinfo;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    loadinfo = form.LoadInfo;
                    Load.Load load = new Load.Load();
                    load.LoadOptions = loadinfo;
                    load.LoadHPGL(_plotterCtrl.Shapes);
                    _plotterCtrl.RecalcClientCoord();
                    _plotterCtrl.Invalidate();
                }
            }
        }

		#endregion

		#region ShapeSelect

		private void _polyLine_CheckedChanged(object sender, EventArgs e)
        {
            _plotterCtrl.CreateNewShape = "PolyLine";
        }

        private void _rectangle_CheckedChanged(object sender, EventArgs e)
        {
            _plotterCtrl.CreateNewShape = "Rectangle";
        }

        private void _line_CheckedChanged(object sender, EventArgs e)
        {
            _plotterCtrl.CreateNewShape = "Line";
        }

        private void _triangle_CheckedChanged(object sender, EventArgs e)
        {
            _plotterCtrl.CreateNewShape = "Triangle";
        }
        private void _circle_CheckedChanged(object sender, EventArgs e)
        {
            _plotterCtrl.CreateNewShape = "Ellipse";
        }

        private void _dot_CheckedChanged(object sender, EventArgs e)
        {
            _plotterCtrl.CreateNewShape = "Dot";
        }

		#endregion
		private void PaintForm_Load(object sender, EventArgs e)
		{
			EnableComControls(Com.IsConnected);
		}

		private void _undo_Click(object sender, EventArgs e)
        {
            if (_plotterCtrl.Shapes.Count > 0)
            {
                _plotterCtrl.Shapes.RemoveAt(_plotterCtrl.Shapes.Count - 1);
                _plotterCtrl.Invalidate();
            }
        }
		private void _del_Click(object sender, EventArgs e)
		{
			if (_plotterCtrl.SelectedShape != -1)
			{
				_plotterCtrl.Shapes.RemoveAt(_plotterCtrl.SelectedShape);
				_plotterCtrl.SelectedShape--;
				_plotterCtrl.Invalidate();
			}
		}

        private void SetSelShape(int idx)
        {
            if (idx >= 0 && idx < _plotterCtrl.Shapes.Count)
            {
                _plotterCtrl.SelectedShape = idx;
                _selidxlbl.Text = idx.ToString();
            }
        }

        private void _selPrev_Click(object sender, EventArgs e)
        {
            SetSelShape(_plotterCtrl.SelectedShape-1);
        }

        private void _selNext_Click(object sender, EventArgs e)
        {
            SetSelShape(_plotterCtrl.SelectedShape + 1);
        }
        private void _selFirst_Click(object sender, EventArgs e)
        {
            SetSelShape(0);
        }

        private void _selLast_Click(object sender, EventArgs e)
        {
            SetSelShape(_plotterCtrl.Shapes.Count - 1);
        }

		#region PlotterControl events

		private void _plotterCtrl_ShapeCreating(object o, PlotterUserControlEventArgs info)
		{
			if (_plot.Checked && (info.Shape is PolyLine))
			{
				AsyncRunCommand(() =>
				{
					Com.SendCommandsAsync(info.Shape.GetHPGLCommands());
				});
			}
		}

		private void _plotterCtrl_ShapeCreated(object o, PlotterUserControlEventArgs info)
		{
			if (_plot.Checked && !(info.Shape is PolyLine))
			{
				AsyncRunCommand(() =>
				{
					Com.SendCommandsAsync(info.Shape.GetHPGLCommands());
				});
			}
		}

		private void _plotterCtrl_PolylineAddPoint(object o, PlotterUserControlEventArgs info)
		{
			if (_plot.Checked)
			{
				AsyncRunCommand(() =>
				{
					Com.SendCommandsAsync(new string[] { info.PolyLinePoint.HPGLCommand });
				});
			}
		}

		private void _plotterCtrl_HpglMousePosition(object o, PlotterUserControlEventArgs info)
		{
			_coord.Text = // e.Location.X.ToString() + ":" + e.Location.Y.ToString() + "(" + 
								 info.HpglPosition.X.ToString() + " : " + info.HpglPosition.Y.ToString() + "  =>  " +
								 Math.Round(info.HpglPosition.X / 40.0, 1) + " : " + Math.Round(info.HpglPosition.Y / 40.0, 1) + "mm";

		}

		#endregion

	}
}
