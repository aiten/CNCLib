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
using Framework.Wpf.ViewModels;
using System.Threading;
using CNCLib.Wpf.ViewModels.ManualControl;

namespace CNCLib.Wpf.ViewModels
{
	public class ManualControlViewModel : BaseViewModel, IManualControlViewModel
	{
		#region crt

		public ManualControlViewModel()
		{
			AxisX = new AxisViewModel(this)
			{
				AxisName = "X",
				AxisIndex = 0,
				Size = Global.Instance.Machine.SizeX,
				ProbeSize = Global.Instance.Machine.ProbeSizeX
			};
			AxisY = new AxisViewModel(this)
			{
				AxisName = "Y",
				AxisIndex = 1,
				Size = Global.Instance.Machine.SizeY,
				ProbeSize = Global.Instance.Machine.ProbeSizeY
			};
			AxisZ = new AxisViewModel(this)
			{
				AxisName = "Z",
				AxisIndex = 2,
				HomeIsMax = true,
				Size = Global.Instance.Machine.SizeZ,
				ProbeSize = Global.Instance.Machine.ProbeSizeZ
			};
			AxisA = new AxisViewModel(this)
			{
				AxisName = "A",
				AxisIndex = 3,
				Size = Global.Instance.Machine.SizeA,
				ProbeSize = 0m
			};
			AxisB = new AxisViewModel(this)
			{
				AxisName = "B",
				AxisIndex = 4,
				Size = Global.Instance.Machine.SizeB,
				ProbeSize = 0m
			};
			AxisC = new AxisViewModel(this)
			{
				AxisName = "C",
				AxisIndex = 5,
				Size = Global.Instance.Machine.SizeC,
				ProbeSize = 0m
			};
	
			CommandHistory = new CommandHistoryViewModel(this) { };

			SD = new SDViewModel(this) { };

			DirectCommand = new DirectCommandViewModel(this) { };

			Shift = new ShiftViewModel(this) { };

			Tool = new ToolViewModel(this) { };

			Rotate = new RotateViewModel(this) { };
		}

		#endregion

		#region AxisVM

		public AxisViewModel AxisX { get; private set; }

		public AxisViewModel AxisY { get; private set; }

		public AxisViewModel AxisZ { get; private set; }

		public AxisViewModel AxisA { get; private set; }

		public AxisViewModel AxisB { get; private set; }
		
		public AxisViewModel AxisC { get; private set; }

		#endregion

		#region CommandHistoryVM

		public CommandHistoryViewModel CommandHistory { get; private set; }

		#endregion

		#region SD_VM

		public SDViewModel SD { get; private set; } 

		#endregion

		#region DirectCommandVM

		public DirectCommandViewModel DirectCommand { get; private set; }

		#endregion

		#region ShiftVM

		public ShiftViewModel Shift { get; private set; }

		#endregion

		#region ToolVM


		public ToolViewModel Tool { get; private set; } 

		#endregion

		#region RotateVM


		public RotateViewModel Rotate { get; private set; }

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

		public void SetPositions(string[] positions, int positionIdx)
		{
			if (positionIdx == 0)
			{
				if (positions.Length >= 1) AxisX.Pos = positions[0];
				if (positions.Length >= 2) AxisY.Pos = positions[1];
				if (positions.Length >= 3) AxisZ.Pos = positions[2];
				if (positions.Length >= 4) AxisA.Pos = positions[3];
				if (positions.Length >= 5) AxisB.Pos = positions[4];
				if (positions.Length >= 6) AxisC.Pos = positions[5];
			}
			else if(positionIdx == 1)
			{
				if (positions.Length >= 1) AxisX.RelPos = positions[0];
				if (positions.Length >= 2) AxisY.RelPos = positions[1];
				if (positions.Length >= 3) AxisZ.RelPos = positions[2];
				if (positions.Length >= 4) AxisA.RelPos = positions[3];
				if (positions.Length >= 5) AxisB.RelPos = positions[4];
				if (positions.Length >= 6) AxisC.RelPos = positions[5];
			}
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
