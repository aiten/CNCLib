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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Framework.Wpf.ViewModels;
using Framework.Wpf.Helpers;
using Proxxon.Wpf.Models;
using System.Threading;
using System.IO;
using Framework.Logic;
using System.Globalization;
using Proxxon.Wpf.ViewModels.ManualControl;

namespace Proxxon.Wpf.ViewModels
{
	public class ManualControlViewModel : BaseViewModel, IManualControlViewModel
	{
		#region crt

		public ManualControlViewModel()
		{
			_AxisX = new AxisViewModel(this)
			{
				AxisName = "X",
				AxisIndex = 0,
				Size = Global.Instance.Machine.SizeX,
				ProbeSize = Global.Instance.Machine.ProbeSizeX
			};
			_AxisY = new AxisViewModel(this)
			{
				AxisName = "Y",
				AxisIndex = 1,
				Size = Global.Instance.Machine.SizeY,
				ProbeSize = Global.Instance.Machine.ProbeSizeY
			};
			_AxisZ = new AxisViewModel(this)
			{
				AxisName = "Z",
				AxisIndex = 2,
				HomeIsMax = true,
				Size = Global.Instance.Machine.SizeZ,
				ProbeSize = Global.Instance.Machine.ProbeSizeZ
			};
			_AxisA = new AxisViewModel(this)
			{
				AxisName = "A",
				AxisIndex = 3,
				Size = Global.Instance.Machine.SizeA,
				ProbeSize = 0m
			};
			_AxisB = new AxisViewModel(this)
			{
				AxisName = "B",
				AxisIndex = 4,
				Size = Global.Instance.Machine.SizeB,
				ProbeSize = 0m
			};
			_AxisC = new AxisViewModel(this)
			{
				AxisName = "C",
				AxisIndex = 5,
				Size = Global.Instance.Machine.SizeC,
				ProbeSize = 0m
			};
	
			_CommandHistory = new CommandHistoryViewModel(this) { };

			_sd = new SDViewModel(this) { };

			_directCommand = new DirectCommandViewModel(this) { };

			_shift = new ShiftViewModel(this) { };

			_tool = new ToolViewModel(this) { };

			_rotate = new RotateViewModel(this) { };
		}

		#endregion

		#region AxisVM

		private AxisViewModel _AxisX;
		public AxisViewModel AxisX { get { return _AxisX; } }

		private AxisViewModel _AxisY;
		public AxisViewModel AxisY { get { return _AxisY; } }

		private AxisViewModel _AxisZ;
		public AxisViewModel AxisZ { get { return _AxisZ; } }

		private AxisViewModel _AxisA;
		public AxisViewModel AxisA { get { return _AxisA; } }

		private AxisViewModel _AxisB;
		public AxisViewModel AxisB { get { return _AxisB; } }
		
		private AxisViewModel _AxisC;
		public AxisViewModel AxisC { get { return _AxisC; } }

		#endregion

		#region CommandHistoryVM

		private CommandHistoryViewModel _CommandHistory;
		public CommandHistoryViewModel CommandHistory { get { return _CommandHistory; } }

		#endregion

		#region SD_VM

		private SDViewModel _sd;
		public SDViewModel SD { get { return _sd; } }

		#endregion

		#region DirectCommandVM

		private DirectCommandViewModel _directCommand;
		public DirectCommandViewModel DirectCommand { get { return _directCommand; } }

		#endregion

		#region ShiftVM

		private ShiftViewModel _shift;
		public ShiftViewModel Shift { get { return _shift; } }

		#endregion

		#region ToolVM


		private ToolViewModel _tool;
		public ToolViewModel Tool { get { return _tool; } }

		#endregion

		#region RotateVM


		private RotateViewModel _rotate;
		public RotateViewModel Rotate { get { return _rotate; } }

		#endregion

		#region Properties

		public Framework.Logic.ArduinoSerialCommunication Com
        {
			get { return Framework.Tools.Singleton<Framework.Logic.ArduinoSerialCommunication>.Instance; }
        }

        public bool Connected
        {
            get { return Com.IsConnected; }
        }

		#endregion

		#region Interface Implementation

		public void SetPositions(string[] positions)
		{
			if (positions.Length >= 1) AxisX.Pos = positions[0];
			if (positions.Length >= 2) AxisY.Pos = positions[1];
			if (positions.Length >= 3) AxisZ.Pos = positions[2];
			if (positions.Length >= 4) AxisA.Pos = positions[3];
			if (positions.Length >= 5) AxisB.Pos = positions[4];
			if (positions.Length >= 6) AxisC.Pos = positions[5];
		}

		public void AsyncRunCommand(Action todo)
		{
			new Thread(() =>
			{
				try
				{
					todo();
					Com.WriteCommandHistory(CommandHistoryViewModel.CommandHistoryFile);
				}
				finally
				{
					CommandHistory.RefreshAfterCommand();
				}
			}
			).Start();
		}

		#endregion
	}
}
