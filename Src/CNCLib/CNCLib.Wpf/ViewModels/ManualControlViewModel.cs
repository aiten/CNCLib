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
using Framework.Wpf.ViewModels;
using CNCLib.Wpf.ViewModels.ManualControl;
using System.Threading.Tasks;
using Framework.Arduino;

namespace CNCLib.Wpf.ViewModels
{
    public class ManualControlViewModel : BaseViewModel, IManualControlViewModel
	{
		#region crt

		public ManualControlViewModel()
		{
			AxisX = new AxisViewModel(this)
			{
				AxisIndex = 0
			};
			AxisY = new AxisViewModel(this)
			{
				AxisIndex = 1
			};
			AxisZ = new AxisViewModel(this)
			{
				AxisIndex = 2,
				HomeIsMax = true
			};
			AxisA = new AxisViewModel(this)
			{
				AxisIndex = 3
			};
			AxisB = new AxisViewModel(this)
			{
				AxisIndex = 4
			};
			AxisC = new AxisViewModel(this)
			{
				AxisIndex = 5
			};

            Move = new MoveViewModel(this) { };

            CommandHistory = new CommandHistoryViewModel(this) { };

			SD = new SDViewModel(this) { };

			DirectCommand = new DirectCommandViewModel(this) { };

			Shift = new ShiftViewModel(this) { };

			Tool = new ToolViewModel(this) { };

			Rotate = new RotateViewModel(this) { };

            Custom = new CustomViewModel(this) { };

			Com.CommandQueueEmpty += OnCommandQueueEmpty;
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

        #region MoveVM

        public MoveViewModel Move { get; private set; }

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

        #region CustomVM

        public CustomViewModel Custom { get; private set; }

        #endregion

        #region Properties

        public Framework.Arduino.SerialCommunication.ISerial Com => Framework.Tools.Pattern.Singleton<Framework.Arduino.SerialCommunication.Serial>.Instance;

	    public bool Connected => Com.IsConnected;

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
		public void RunAndUpdate(Action todo)
		{
			todo();
		}

		private void OnCommandQueueEmpty(object sender, Framework.Arduino.SerialCommunication.SerialEventArgs arg)
		{
			CommandHistory.RefreshAfterCommand();
		}

		public void RunInNewTask(Action todo)
		{
			Task.Run(() =>
			{
				RunAndUpdate(todo);
			}
			);
		}

		#endregion
	}
}
