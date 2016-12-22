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
		//public decimal ProbeSize { get { return Global.Instance.Machine.GetProbeSize(AxisIndex); } }
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
		private void SendMoveCommand(string dist) { RunInNewTask(() => { Com.SendCommand("g91 g0" + AxisName + dist + " g90"); }); }

		private void SendProbeCommand(int axisindex)
		{
			RunInNewTask(() =>
			{
				new MachineGCodeHelper().SendProbeCommandAsync(AxisIndex).GetAwaiter().GetResult();
			});
		}

		public void SendRefMove() { RunInNewTask(() => { Com.SendCommand("g28 " + AxisName + "0"); }); }
		public void SendG92() { RunInNewTask(() => { Com.SendCommand("g92 " + AxisName + ParamDec.ToString(CultureInfo.InvariantCulture)); }); }
		public void SendG31() { SendProbeCommand(AxisIndex); }
		public void SendHome() 
		{ 
			RunInNewTask(() => 
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

		public ICommand SendPlus100Command => new DelegateCommand(() => SendMoveCommand("100"), () => CanSendCommand() && Size >= 100.0m);
		public ICommand SendPlus10Command => new DelegateCommand(() => SendMoveCommand("10"), () => CanSendCommand() && Size >= 10.0m);
		public ICommand SendPlus1Command => new DelegateCommand(() => SendMoveCommand("1"), () => CanSendCommand() && Size >= 1.0m);
		public ICommand SendPlus01Command => new DelegateCommand(() => SendMoveCommand("0.1"), () => CanSendCommand() && Size >= 0.1m);
		public ICommand SendPlus001Command => new DelegateCommand(() => SendMoveCommand("0.01"), () => CanSendCommand() && Size >= 0.01m);
		public ICommand SendMinus100Command => new DelegateCommand(() => SendMoveCommand("-100"), () => CanSendCommand() && Size >= 100.0m);
		public ICommand SendMinus10Command => new DelegateCommand(() => SendMoveCommand("-10"), () => CanSendCommand() && Size >= 10.0m);
		public ICommand SendMinus1Command => new DelegateCommand(() => SendMoveCommand("-1"), () => CanSendCommand() && Size >= 1.0m);
		public ICommand SendMinus01Command => new DelegateCommand(() => SendMoveCommand("-0.1"), () => CanSendCommand() && Size >= 0.1m);
		public ICommand SendMinus001Command => new DelegateCommand(() => SendMoveCommand("-0.01"), () => CanSendCommand() && Size >= 0.01m);
		public ICommand SendRefMoveCommand => new DelegateCommand(SendRefMove, CanSendCommand);
		public ICommand SendG92Command => new DelegateCommand(SendG92,  () => { decimal dummy; return CanSendCommand() && decimal.TryParse(Param,out dummy); });
		public ICommand SendG31Command => new DelegateCommand(SendG31, CanSendCommand);
		public ICommand SendHomeCommand => new DelegateCommand(SendHome, CanSendCommand);

		#endregion
	}
}
