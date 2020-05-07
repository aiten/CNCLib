/*
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

using System;
using System.Linq;
using System.Windows.Input;

using CNCLib.GCode.Serial;
using CNCLib.WpfClient.Helpers;

using Framework.Arduino.SerialCommunication;
using Framework.Wpf.Helpers;

namespace CNCLib.WpfClient.ViewModels.ManualControl
{
    public class ToolViewModel : DetailViewModel, IDisposable
    {
        private readonly Global _global;

        #region ctr

        public ToolViewModel(IManualControlViewModel vm, Global global) : base(vm, global)
        {
            _global = global;

            _global.Com.LocalCom.CommandQueueChanged  += OnCommandQueueChanged;
            _global.Com.RemoteCom.CommandQueueChanged += OnCommandQueueChanged;
        }

        public void Dispose()
        {
            _global.Com.LocalCom.CommandQueueChanged  -= OnCommandQueueChanged;
            _global.Com.RemoteCom.CommandQueueChanged -= OnCommandQueueChanged;
        }

        #endregion

        #region Properties

        public int PendingCommandCount { get; set; }

        public bool Pause
        {
            get => _global.Com.Current.Pause;
            set => _global.Com.Current.Pause = value;
        }

        private bool _updateAfterSendNext = false;

        private void OnCommandQueueChanged(object sender, SerialEventArgs arg)
        {
            PendingCommandCount = arg.QueueLength;

            RaisePropertyChanged(nameof(PendingCommandCount));

            if (_updateAfterSendNext)
            {
                _updateAfterSendNext = false;
                ((ManualControlViewModel)Vm).CommandHistory.RefreshAfterCommand();
            }
        }

        private void SetSendNext()
        {
            _updateAfterSendNext         = true;
            _global.Com.Current.SendNext = true;
        }

        #endregion

        #region Commands / CanCommands

        public bool CanSendSpindle()
        {
            return CanSendGCode() && _global.Machine.Spindle;
        }

        public bool CanSendCoolant()
        {
            return CanSendGCode() && _global.Machine.Coolant;
        }

        public bool CanSendLaser()
        {
            return CanSendGCode() && _global.Machine.Laser;
        }

        public void SendInfo()
        {
            RunAndUpdate(() => { _global.Com.Current.QueueCommand("?"); });
        }

        public void SendDebug()
        {
            RunAndUpdate(() => { _global.Com.Current.QueueCommand("&"); });
        }

        public void SendVersion()
        {
            RunAndUpdate(() => { _global.Com.Current.QueueCommand("@"); });
        }

        public void SendAbort()
        {
            RunAndUpdate(
                () =>
                {
                    _global.Com.Current.AbortCommands();
                    _global.Com.Current.ResumeAfterAbort();
                    _global.Com.Current.QueueCommand("!");
                });
        }

        public void SendResurrect()
        {
            RunAndUpdate(
                () =>
                {
                    _global.Com.Current.AbortCommands();
                    _global.Com.Current.ResumeAfterAbort();
                    _global.Com.Current.QueueCommand("!!!");
                });
        }

        public void ClearQueue()
        {
            RunAndUpdate(
                () =>
                {
                    _global.Com.Current.AbortCommands();
                    _global.Com.Current.ResumeAfterAbort();
                });
        }

        public void SendM03SpindleOn()
        {
            RunAndUpdate(() => { _global.Com.Current.QueueCommand("m3"); });
        }

        public void SendM05SpindleOff()
        {
            RunAndUpdate(() => { _global.Com.Current.QueueCommand("m5"); });
        }

        public void SendM07CoolantOn()
        {
            RunAndUpdate(() => { _global.Com.Current.QueueCommand("m7"); });
        }

        public void SendM09CoolantOff()
        {
            RunAndUpdate(() => { _global.Com.Current.QueueCommand("m9"); });
        }

        public void SendM106LaserOn()
        {
            RunAndUpdate(() => { _global.Com.Current.QueueCommand("m106 s255"); });
        }

        public void SendM106LaserOnMin()
        {
            RunAndUpdate(() => { _global.Com.Current.QueueCommand("m106 s1"); });
        }

        public void SendM107LaserOff()
        {
            RunAndUpdate(() => { _global.Com.Current.QueueCommand("m107"); });
        }

        public void SendM100ProbeDefault()
        {
            RunAndUpdate(() => { _global.Com.Current.QueueCommand("m100"); });
        }

        public void SendM101ProbeInvert()
        {
            RunAndUpdate(() => { _global.Com.Current.QueueCommand("m101"); });
        }

        public void ReadPosition()
        {
            RunAndUpdate(
                async () =>
                {
                    var position = (await _global.Com.Current.GetPosition(_global.Machine.GetCommandPrefix())).ToList();

                    if (position.Any())
                    {
                        SetPositions(position.First().ToArray(), 0);
                        if (position.Count > 1)
                        {
                            SetPositions(position[1].ToArray(), 1);
                        }
                    }
                });
        }

        public void WritePending()
        {
            RunInNewTask(() => { _global.Com.Current.WritePendingCommandsToFile(System.IO.Path.GetTempPath() + "PendingCommands.nc"); });
        }

        #endregion

        #region ICommand

        public ICommand SendInfoCommand             => new DelegateCommand(SendInfo,             CanSend);
        public ICommand SendDebugCommand            => new DelegateCommand(SendDebug,            CanSend);
        public ICommand SendVersionCommand          => new DelegateCommand(SendVersion,          CanSend);
        public ICommand SendAbortCommand            => new DelegateCommand(SendAbort,            CanSend);
        public ICommand SendResurrectCommand        => new DelegateCommand(SendResurrect,        CanSend);
        public ICommand SendClearQueue              => new DelegateCommand(ClearQueue,           CanSend);
        public ICommand SendM03SpindleOnCommand     => new DelegateCommand(SendM03SpindleOn,     CanSendSpindle);
        public ICommand SendM05SpindleOffCommand    => new DelegateCommand(SendM05SpindleOff,    CanSendSpindle);
        public ICommand SendM07CoolantOnCommand     => new DelegateCommand(SendM07CoolantOn,     CanSendCoolant);
        public ICommand SendM09CoolantOffCommand    => new DelegateCommand(SendM09CoolantOff,    CanSendCoolant);
        public ICommand SendM100ProbeDefaultCommand => new DelegateCommand(SendM100ProbeDefault, CanSend);
        public ICommand SendM101ProbeInvertCommand  => new DelegateCommand(SendM101ProbeInvert,  CanSend);
        public ICommand SendM106LaserOnCommand      => new DelegateCommand(SendM106LaserOn,      CanSendLaser);
        public ICommand SendM106LaserOnMinCommand   => new DelegateCommand(SendM106LaserOnMin,   CanSendLaser);
        public ICommand SendM107LaserOffCommand     => new DelegateCommand(SendM107LaserOff,     CanSendLaser);
        public ICommand ReadPositionCommand         => new DelegateCommand(ReadPosition,         CanSend);
        public ICommand WritePendingCommands        => new DelegateCommand(WritePending,         CanSend);

        public ICommand SendNextCommands =>
            new DelegateCommand(SetSendNext, () => CanSend() && Pause && PendingCommandCount > 0);

        #endregion
    }
}