﻿/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/

namespace CNCLib.WpfClient.ViewModels
{
    using System;
    using System.Threading.Tasks;

    using CNCLib.WpfClient.ViewModels.ManualControl;

    using Framework.Wpf.ViewModels;

    public class ManualControlViewModel : BaseViewModel, IManualControlViewModel
    {
        private readonly Global _global;

        #region crt

        public ManualControlViewModel(Global global)
        {
            _global = global;

            AxisX = new AxisViewModel(this, global)
            {
                AxisIndex = 0
            };
            AxisY = new AxisViewModel(this, global)
            {
                AxisIndex = 1
            };
            AxisZ = new AxisViewModel(this, global)
            {
                AxisIndex = 2,
                HomeIsMax = true
            };
            AxisA = new AxisViewModel(this, global)
            {
                AxisIndex = 3
            };
            AxisB = new AxisViewModel(this, global)
            {
                AxisIndex = 4
            };
            AxisC = new AxisViewModel(this, global)
            {
                AxisIndex = 5
            };

            Move = new MoveViewModel(this, global);

            CommandHistory = new CommandHistoryViewModel(this, global);

            SD = new SDViewModel(this, global);

            DirectCommand = new DirectCommandViewModel(this, global);

            Shift = new ShiftViewModel(this, global);

            Tool = new ToolViewModel(this, global);

            Rotate = new RotateViewModel(this, global);

            Custom = new CustomViewModel(this, global);

            WorkOffset = new WorkOffsetViewModel(this, global);

            _global.Com.LocalCom.CommandQueueEmpty  += OnCommandQueueEmpty;
            _global.Com.RemoteCom.CommandQueueEmpty += OnCommandQueueEmpty;
        }

        #endregion

        #region AxisVM

        public AxisViewModel AxisX { get; }

        public AxisViewModel AxisY { get; }

        public AxisViewModel AxisZ { get; }

        public AxisViewModel AxisA { get; }

        public AxisViewModel AxisB { get; }

        public AxisViewModel AxisC { get; }

        #endregion

        #region MoveVM

        public MoveViewModel Move { get; }

        #endregion

        #region CommandHistoryVM

        public CommandHistoryViewModel CommandHistory { get; }

        #endregion

        #region SD_VM

        public SDViewModel SD { get; }

        #endregion

        #region DirectCommandVM

        public DirectCommandViewModel DirectCommand { get; }

        #endregion

        #region ShiftVM

        public ShiftViewModel Shift { get; }

        #endregion

        #region WorkOffsetVM

        public WorkOffsetViewModel WorkOffset { get; }

        #endregion

        #region ToolVM

        public ToolViewModel Tool { get; }

        #endregion

        #region RotateVM

        public RotateViewModel Rotate { get; }

        #endregion

        #region CustomVM

        public CustomViewModel Custom { get; }

        #endregion

        #region Properties

        public bool Connected => _global.Com.Current.IsConnected;

        #endregion

        #region Interface Implementation

        public void SetPositions(decimal[] positions, int positionIdx)
        {
            if (positionIdx == 0)
            {
                if (positions.Length >= 1)
                {
                    AxisX.Pos = positions[0].ToString();
                }

                if (positions.Length >= 2)
                {
                    AxisY.Pos = positions[1].ToString();
                }

                if (positions.Length >= 3)
                {
                    AxisZ.Pos = positions[2].ToString();
                }

                if (positions.Length >= 4)
                {
                    AxisA.Pos = positions[3].ToString();
                }

                if (positions.Length >= 5)
                {
                    AxisB.Pos = positions[4].ToString();
                }

                if (positions.Length >= 6)
                {
                    AxisC.Pos = positions[5].ToString();
                }
            }
            else if (positionIdx == 1)
            {
                if (positions.Length >= 1)
                {
                    AxisX.RelPos = positions[0].ToString();
                }

                if (positions.Length >= 2)
                {
                    AxisY.RelPos = positions[1].ToString();
                }

                if (positions.Length >= 3)
                {
                    AxisZ.RelPos = positions[2].ToString();
                }

                if (positions.Length >= 4)
                {
                    AxisA.RelPos = positions[3].ToString();
                }

                if (positions.Length >= 5)
                {
                    AxisB.RelPos = positions[4].ToString();
                }

                if (positions.Length >= 6)
                {
                    AxisC.RelPos = positions[5].ToString();
                }
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
            Task.Run(() => { RunAndUpdate(todo); });
        }

        #endregion
    }
}