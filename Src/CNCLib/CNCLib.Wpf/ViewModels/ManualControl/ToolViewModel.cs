////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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
using System.Linq;
using System.Windows.Input;
using Framework.Wpf.Helpers;
using Framework.Arduino.SerialCommunication;
using CNCLib.Wpf.Helpers;
using System.Globalization;

namespace CNCLib.Wpf.ViewModels.ManualControl
{
    public class ToolViewModel : DetailViewModel, IDisposable
    {
        #region ctr

        public ToolViewModel(IManualControlViewModel vm) : base(vm)
        {
            Global.Instance.Com.LocalCom.CommandQueueChanged  += OnCommandQueueChanged;
            Global.Instance.Com.RemoteCom.CommandQueueChanged += OnCommandQueueChanged;
        }

        public void Dispose()
        {
            Global.Instance.Com.LocalCom.CommandQueueChanged  -= OnCommandQueueChanged;
            Global.Instance.Com.RemoteCom.CommandQueueChanged -= OnCommandQueueChanged;
        }

        #endregion

        #region Properties

        public int PendingCommandCount { get; set; }

        public bool Pause { get => Global.Instance.Com.Current.Pause; set => Global.Instance.Com.Current.Pause = value; }

        private bool _updateAfterSendNext = false;

        private void OnCommandQueueChanged(object sender, Framework.Arduino.SerialCommunication.SerialEventArgs arg)
        {
            PendingCommandCount = arg.QueueLenght;

            RaisePropertyChanged(nameof(PendingCommandCount));

            if (_updateAfterSendNext)
            {
                _updateAfterSendNext = false;
                ((ManualControlViewModel) Vm).CommandHistory.RefreshAfterCommand();
            }
        }

        private void SetSendNext()
        {
            _updateAfterSendNext                 = true;
            Global.Instance.Com.Current.SendNext = true;
        }

        #endregion

        #region Commands / CanCommands

        public bool CanSendSpindle()
        {
            return CanSendGCode() && Global.Instance.Machine.Spindle;
        }

        public bool CanSendCoolant()
        {
            return CanSendGCode() && Global.Instance.Machine.Coolant;
        }

        public bool CanSendLaser()
        {
            return CanSendGCode() && Global.Instance.Machine.Laser;
        }

        public void SendInfo()
        {
            RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand("?"); });
        }

        public void SendDebug()
        {
            RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand("&"); });
        }

        public void SendAbort()
        {
            RunAndUpdate(() =>
            {
                Global.Instance.Com.Current.AbortCommands();
                Global.Instance.Com.Current.ResumeAfterAbort();
                Global.Instance.Com.Current.QueueCommand("!");
            });
        }

        public void SendResurrect()
        {
            RunAndUpdate(() =>
            {
                Global.Instance.Com.Current.AbortCommands();
                Global.Instance.Com.Current.ResumeAfterAbort();
                Global.Instance.Com.Current.QueueCommand("!!!");
            });
        }

        public void ClearQueue()
        {
            RunAndUpdate(() =>
            {
                Global.Instance.Com.Current.AbortCommands();
                Global.Instance.Com.Current.ResumeAfterAbort();
            });
        }

        public void SendM03SpindleOn()
        {
            RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand("m3"); });
        }

        public void SendM05SpindleOff()
        {
            RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand("m5"); });
        }

        public void SendM07CoolandOn()
        {
            RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand("m7"); });
        }

        public void SendM09CoolandOff()
        {
            RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand("m9"); });
        }

        public void SendM106LaserOn()
        {
            RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand("m106 s255"); });
        }

        public void SendM106LaserOnMin()
        {
            RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand("m106 s1"); });
        }

        public void SendM107LaserOff()
        {
            RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand("m107"); });
        }

        public void SendM100ProbeDefault()
        {
            RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand("m100"); });
        }

        public void SendM101ProbeInvert()
        {
            RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand("m101"); });
        }

        private decimal[] Convert(string[] list)
        {
            var ret = new decimal[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                decimal val;
                if (decimal.TryParse(list[i], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out val))
                {
                    ret[i] = val;
                }
            }

            return ret;
        }

        string TrimMsg(string msg, string replace)
        {
            return msg.Replace("ok", "").Replace(" ", "").Replace(replace, "").Replace(">", "");
        }

        decimal[] Convert(string msg, string replace)
        {
            return Convert(TrimMsg(msg, replace).Split(':', ','));
        }

        decimal[] TryConvert(string[] tags, string txt)
        {
            string tag = tags.FirstOrDefault((s) => s.StartsWith(txt));
            if (tag != null)
            {
                return Convert(TrimMsg(tag, txt).Split(':', ','));
            }

            return null;
        }


        public void ReadPosition()
        {
            RunAndUpdate(async () =>
            {
                string message = await Global.Instance.Com.Current.SendCommandAndReadOKReplyAsync(MachineGCodeHelper.PrepareCommand("?"), 10 * 1000);

                if (!string.IsNullOrEmpty(message))
                {
                    if (message.Contains("MPos:"))
                    {
                        // new or grbl format
                        message = message.Replace("ok", "").Replace("<", "").Replace(">", "").Trim();

                        string[] tags = message.Split('|');

                        var mpos = TryConvert(tags, "MPos:");
                        if (mpos != null)
                        {
                            SetPositions(mpos, 0);

                            var wco = TryConvert(tags, "WCO:");
                            if (wco != null)
                            {
                                for (int i = 0; i < wco.Length; i++)
                                {
                                    mpos[i] -= wco[i];
                                }
                            }

                            SetPositions(mpos, 1);
                        }
                    }
                    else
                    {
                        decimal[] mpos = Convert(message, "dummy");
                        SetPositions(mpos, 0);

                        message = await Global.Instance.Com.Current.SendCommandAndReadOKReplyAsync(MachineGCodeHelper.PrepareCommand("m114 s1"), 10 * 1000);

                        if (!string.IsNullOrEmpty(message))
                        {
                            decimal[] rpos = Convert(message, "dummy");
                            SetPositions(rpos, 1);
                        }
                    }
                }
            });
        }

        public void WritePending()
        {
            RunInNewTask(() => { Global.Instance.Com.Current.WritePendingCommandsToFile(System.IO.Path.GetTempPath() + "PendingCommands.nc"); });
        }

        #endregion

        #region ICommand

        public ICommand SendInfoCommand             => new DelegateCommand(SendInfo,             CanSend);
        public ICommand SendDebugCommand            => new DelegateCommand(SendDebug,            CanSend);
        public ICommand SendAbortCommand            => new DelegateCommand(SendAbort,            CanSend);
        public ICommand SendResurrectCommand        => new DelegateCommand(SendResurrect,        CanSend);
        public ICommand SendClearQueue              => new DelegateCommand(ClearQueue,           CanSend);
        public ICommand SendM03SpindleOnCommand     => new DelegateCommand(SendM03SpindleOn,     CanSendSpindle);
        public ICommand SendM05SpindleOffCommand    => new DelegateCommand(SendM05SpindleOff,    CanSendSpindle);
        public ICommand SendM07CoolandOnCommand     => new DelegateCommand(SendM07CoolandOn,     CanSendCoolant);
        public ICommand SendM09CoolandOffCommand    => new DelegateCommand(SendM09CoolandOff,    CanSendCoolant);
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