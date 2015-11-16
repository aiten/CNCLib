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

using System.Linq;
using System.Windows.Input;
using Framework.Wpf.Helpers;
using System.Globalization;
using CNCLib.Wpf.Helpers;
using Framework.Arduino;

namespace CNCLib.Wpf.ViewModels.ManualControl
{
	public class AxisViewModel : DetailViewModel
	{
		public AxisViewModel(IManualControlViewModel vm) : base(vm) 	{}

		#region Properties

		public int AxisIndex { get; set; }
		public string AxisName { get { return Global.Instance.Machine.GetAxisName(AxisIndex); } }
		public decimal Size { get { return Global.Instance.Machine.GetSize(AxisIndex); }  }
		public decimal ProbeSize { get { return Global.Instance.Machine.GetProbeSize(AxisIndex); } }
		public bool HomeIsMax { get; set; }

		private string _param = "0";
		public decimal ParamDec { get { return decimal.Parse(Param); } }
		public string Param
		{
			get { return _param; }
			set { SetProperty(ref _param, value); }
		}

		private string _pos = "";
		public string Pos
		{
			get { return _pos; }
			set { SetProperty(ref _pos, value); }
		}
		private string _relPos = "";
		public string RelPos
		{
			get { return _relPos; }
			set { SetProperty(ref _relPos, value); }
		}

		public bool Enabled { get { return Global.Instance.Machine.Axis > AxisIndex && Size > 0m; } }

		#endregion

		#region Commands / CanCommands
		private void SendMoveCommand(string dist) { AsyncRunCommand(() => { Com.SendCommand("g91 g0" + AxisName + dist + " g90"); }); }

		private void SendProbeCommand(string axisname, decimal probesize)
		{
			AsyncRunCommand(() =>
			{
				string probdist = Global.Instance.Machine.ProbeDist.ToString(CultureInfo.InvariantCulture);
				string probdistup = Global.Instance.Machine.ProbeDistUp.ToString(CultureInfo.InvariantCulture);
				string probfeed = Global.Instance.Machine.ProbeFeed.ToString(CultureInfo.InvariantCulture);

				Com.SendCommand("g91 g31 " + axisname + "-" + probdist + " F" + probfeed + " g90");
				if ((Com.LastCommand.ReplyType & ArduinoSerialCommunication.EReplyType.ReplyError) == 0)
				{
					Com.SendCommand("g92 " + axisname + (-probesize).ToString(CultureInfo.InvariantCulture));
					Com.SendCommand("g91 g0" + axisname + probdistup + " g90");
				}
			});
		}

		public void SendRefMove() { AsyncRunCommand(() => { Com.SendCommand("g28 " + AxisName + "0"); }); }
		public void SendG92() { AsyncRunCommand(() => { Com.SendCommand("g92 " + AxisName + ParamDec.ToString(CultureInfo.InvariantCulture)); }); }
		public void SendG31() { SendProbeCommand(AxisName, ProbeSize); }
		public void SendHome() 
		{ 
			AsyncRunCommand(() => 
			{ 
				if (HomeIsMax) 
					Com.SendCommand("g53 g0"+ AxisName+"#" + (5161+AxisIndex).ToString()); 
				else
					Com.SendCommand("g53 g0" + AxisName + "0");
			}); 
		}

		public bool CanSendCommand()
		{
			return CanSend() && Enabled;
		}

		#endregion

		#region ICommands

		public ICommand SendPlus100Command { get { return new DelegateCommand(() => SendMoveCommand("100"), () => CanSendCommand() && Size >= 100.0m); } }
		public ICommand SendPlus10Command { get { return new DelegateCommand(() => SendMoveCommand("10"), () => CanSendCommand() && Size >= 10.0m); } }
		public ICommand SendPlus1Command { get { return new DelegateCommand(() => SendMoveCommand("1"), () => CanSendCommand() && Size >= 1.0m); } }
		public ICommand SendPlus01Command { get { return new DelegateCommand(() => SendMoveCommand("0.1"), () => CanSendCommand() && Size >= 0.1m); } }
		public ICommand SendPlus001Command { get { return new DelegateCommand(() => SendMoveCommand("0.01"), () => CanSendCommand() && Size >= 0.01m); } }
		public ICommand SendMinus100Command { get { return new DelegateCommand(() => SendMoveCommand("-100"), () => CanSendCommand() && Size >= 100.0m); } }
		public ICommand SendMinus10Command { get { return new DelegateCommand(() => SendMoveCommand("-10"), () => CanSendCommand() && Size >= 10.0m); } }
		public ICommand SendMinus1Command { get { return new DelegateCommand(() => SendMoveCommand("-1"), () => CanSendCommand() && Size >= 1.0m); } }
		public ICommand SendMinus01Command { get { return new DelegateCommand(() => SendMoveCommand("-0.1"), () => CanSendCommand() && Size >= 0.1m); } }
		public ICommand SendMinus001Command { get { return new DelegateCommand(() => SendMoveCommand("-0.01"), () => CanSendCommand() && Size >= 0.01m); } }
		public ICommand SendRefMoveCommand { get { return new DelegateCommand(SendRefMove, CanSendCommand); } }
		public ICommand SendG92Command { get { return new DelegateCommand(SendG92,  () => { decimal dummy; return CanSendCommand() && decimal.TryParse(Param,out dummy); }); } }
		public ICommand SendG31Command { get { return new DelegateCommand(SendG31, CanSendCommand); } }
		public ICommand SendHomeCommand { get { return new DelegateCommand(SendHome, CanSendCommand); } }

		#endregion
	}
}
