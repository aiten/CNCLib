////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using CNCLib.GCode;
using CNCLib.GCode.Commands;
using CNCLib.Logic.Contracts.DTO;
using Framework.Tools.Drawing;
using Framework.Wpf.Helpers;
using Framework.Wpf.ViewModels;
using Framework.Arduino;

namespace CNCLib.Wpf.ViewModels
{
	public class PreviewViewModel : BaseViewModel
	{
		#region crt

		public PreviewViewModel()
		{
			Com.CommandSending += CommandSending;
		}

		#endregion

		#region Properties

		public Framework.Arduino.ArduinoSerialCommunication Com
		{
			get { return Framework.Tools.Pattern.Singleton<Framework.Arduino.ArduinoSerialCommunication>.Instance; }
		}

		private CommandList _commands = new CommandList();

		public CommandList Commands
		{
			get { return _commands; }
			set { SetProperty(() => _commands == value, () => _commands = value); }
		}

		private decimal _offsetX = 0;

		public decimal OffsetX
		{
			get { return _offsetX; }
			set { SetProperty(() => _offsetX == value, () => _offsetX = value); }
		}

		private decimal _offsetY = 0;
		public decimal OffsetY
		{
			get { return _offsetY; }
			set { SetProperty(() => _offsetY == value, () => _offsetY = value); }
		}

		private double _zoom = 1;
		public double Zoom
		{
			get { return _zoom; }
			set { SetProperty(() => _zoom == value, () => _zoom = value); }
		}

		private double _rotateAngle = 0.0;
		public double RotateAngle
		{
			get { return _rotateAngle; }
			set { SetProperty(() => _rotateAngle == value, () => _rotateAngle = value); }
		}
		private double[] _rotateVector = new double[] { 0, 0, 1 };
		public double[] RotateVector
		{
			get { return _rotateVector; }
			set { SetProperty(() => _rotateVector == value, () => _rotateVector = value); }
		}

		private decimal _laserSize = 0.25m;
		public decimal LaserSize
		{
			get { return _laserSize; }
			set { SetProperty(() => _laserSize == value, () => _laserSize = value); }
		}

		private decimal _cutterSize = 0;
		public decimal CutterSize
		{
			get { return _cutterSize; }
			set { SetProperty(() => _cutterSize == value, () => _cutterSize = value); }
		}

		private Color _machineColor=Colors.Black;
		public Color MachineColor
		{
			get { return _machineColor; }
			set { SetProperty(() => _machineColor == value, () => _machineColor = value); }
		}

		private Color _laserOnColor=Colors.Red;
		public Color LaserOnColor
		{
			get { return _laserOnColor; }
			set { SetProperty(() => _laserOnColor == value, () => _laserOnColor = value); }
		}

		private Color _laserOffColor = Colors.Orange;
		public Color LaserOffColor
		{
			get { return _laserOffColor; }
			set { SetProperty(() => _laserOffColor == value, () => _laserOffColor = value); }
		}

		private Color _cutColor = Colors.LightGray;
		public Color CutColor
		{
			get { return _cutColor; }
			set { SetProperty(() => _cutColor == value, () => _cutColor = value); }
		}

		private Color _cutDotColor = Colors.Blue;
		public Color CutDotColor
		{
			get { return _cutDotColor; }
			set { SetProperty(() => _cutDotColor == value, () => _cutDotColor = value); }
		}

		private Color _cutEllipseColor = Colors.Cyan;
		public Color CutEllipseColor
		{
			get { return _cutEllipseColor; }
			set { SetProperty(() => _cutEllipseColor == value, () => _cutEllipseColor = value); }
		}

		private Color _cutArcColor = Colors.Beige;
		public Color CutArcColor
		{
			get { return _cutArcColor; }
			set { SetProperty(() => _cutArcColor == value, () => _cutArcColor = value); }
		}

		private Color _fastColor = Colors.Green;
		public Color FastColor
		{
			get { return _fastColor; }
			set { SetProperty(() => _fastColor == value, () => _fastColor = value); }
		}

		private Color _helpLineColor = Colors.LightBlue;
		public Color HelpLineColor
		{
			get { return _helpLineColor; }
			set { SetProperty(() => _helpLineColor == value, () => _helpLineColor = value); }
		}

		private string _cmdHistoryFileName = @"%USERPROFILE%\Documents\Command.txt";
		public string CmdHistoryFileName
		{
			get { return _cmdHistoryFileName; }
			set { SetProperty(() => _cmdHistoryFileName == value, () => _cmdHistoryFileName = value); }
		}

		#endregion

		#region GUI-forward

		public class GetLoadInfoArg
		{
			public LoadOptions LoadOption { get; set; }
			public bool UseAzure { get; set; }
		}
		public Func<GetLoadInfoArg, bool> GetLoadInfo { get; set; }

		public Action RefreshPreview { get; set; }

		#endregion

		#region private/intern

		LoadOptions loadinfo = new LoadOptions();
		bool _useAzure = false;
		bool _loadingOrSending = false;

		Command _lastCurrentCommand = null;

		private void CommandSending(object sender, ArduinoSerialCommunicationEventArgs arg)
		{
			Commands.Current = Commands.FirstOrDefault((c) => c == arg.Command.Tag);
			if (Commands.Current != _lastCurrentCommand)
			{
				_lastCurrentCommand = Commands.Current;
				RefreshPreview?.Invoke();
			}
		}

		#endregion

		#region Operations

		public void SendTo()
		{
			new Task(() =>
			{
				_loadingOrSending = true;

				try
				{
					Com.ClearCommandHistory();
					Command last = null;
					Commands.ForEach((cmd) =>
					{
						string[] cmds = cmd.GetGCodeCommands(last != null ? last.CalculatedEndPosition : null);
						if (cmds != null)
						{
							foreach (string str in cmds)
							{
								foreach (var c in Com.QueueCommand(str))
								{
									c.Tag = cmd;
								}
							}
						}
						last = cmd;
					});
				}
				finally
				{
					_loadingOrSending = false;
				}
			}
			).Start();
		}

		public async Task Load()
		{
			if (loadinfo.AutoScaleSizeX == 0 || loadinfo.AutoScale == false)
			{
				loadinfo.AutoScaleSizeX = Settings.Instance.SizeX;
				loadinfo.AutoScaleSizeY = Settings.Instance.SizeY;
			}

			var arg = new GetLoadInfoArg() { LoadOption = loadinfo, UseAzure = _useAzure };
			if ((GetLoadInfo?.Invoke(arg)).GetValueOrDefault(false))
			{
				loadinfo = arg.LoadOption;
				_useAzure = arg.UseAzure;

				var ld = new GCodeLoad();

				try
				{
					_loadingOrSending = true;
					Commands = await ld.Load(loadinfo, _useAzure);
				}
				catch (Exception ex)
				{
					MessageBox?.Invoke("Load failed with error: " + ex.Message, "CNCLib", MessageBoxButton.OK, MessageBoxImage.Stop);
				}
				finally
				{
					_loadingOrSending = false;
					CommandManager.InvalidateRequerySuggested();
				}
			}
		}

		public bool CanSendTo()
		{
			return !_loadingOrSending && Com.IsConnected && Commands.Count > 0;
		}

		public bool CanLoad()
		{
			return _loadingOrSending == false;
		}

		public bool CanResetView()
		{
			return true;
		}

		public void ResetViewXY()
		{
			Zoom = 1;
			// set Zoom first, set Zoom adjusts OffsetX/Y
			OffsetX = 0;
			OffsetY = 0;
			RotateAngle = 0;
		}

		public void ResetViewXZ()
		{
			Zoom = 1;
			// set Zoom first, set Zoom adjusts OffsetX/Y
			OffsetX = 0;
			OffsetY = 0;
			RotateVector = new double[] { 1, 1, 1 };
			RotateAngle = Math.PI*4.0/3.0;
		}

		public void ResetViewYZ()
		{
			Zoom = 1;
			// set Zoom first, set Zoom adjusts OffsetX/Y
			OffsetX = 0;
			OffsetY = 0;
			RotateVector = new double[] { 1, 0, 0 };
			RotateAngle = -Math.PI / 2.0;
		}

		public void GotoPos(Point3D pt)
		{
			Com.QueueCommand(string.Format(@"g0 x{0} y{1}", (pt.X??0.0).ToString(CultureInfo.InvariantCulture), (pt.Y?? 0.0).ToString(CultureInfo.InvariantCulture)));
		}
		public bool CanGotoPos(Point3D pt)
		{
			return !_loadingOrSending && Com.IsConnected;
		}

		#endregion

		#region Commands

		private ICommand _LoadCommand;
		public ICommand LoadCommand { get { return _LoadCommand ?? (_LoadCommand = new DelegateCommand(async () => await Load(), CanLoad)); } }
		public ICommand SendToCommand => new DelegateCommand(SendTo, CanSendTo);
		public ICommand ResetViewCommandXY => new DelegateCommand(ResetViewXY, CanResetView);
		public ICommand GotoPosCommand => new DelegateCommand<Point3D>(GotoPos, CanGotoPos);
		public ICommand ResetViewCommandXZ => new DelegateCommand(ResetViewXZ, CanResetView);
		public ICommand ResetViewCommandYZ => new DelegateCommand(ResetViewYZ, CanResetView);

		#endregion
	}
}
