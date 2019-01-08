////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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
using System.Windows.Input;
using System.Collections.ObjectModel;

using Framework.Arduino.SerialCommunication;
using Framework.Wpf.Helpers;

namespace CNCLib.Wpf.ViewModels.ManualControl
{
    public class DirectCommandViewModel : DetailViewModel
    {
        private readonly Global _global;

        public DirectCommandViewModel(IManualControlViewModel vm, Global global) : base(vm, global)
        {
            _global = global ?? throw new ArgumentNullException(); ;
        }

        #region Properties

        #endregion

        #region DirectCommand

        private string _directCommand;

        public string DirectCommand
        {
            get => _directCommand;
            set => SetProperty(ref _directCommand, value);
        }

        private void AddDirectCommandHistory(string cmd)
        {
            if (_directCommandHistory == null)
            {
                _directCommandHistory = new ObservableCollection<string>();
            }

            _directCommandHistory.Add(cmd);
            DirectCommandHistory = _directCommandHistory;
        }

        private ObservableCollection<string> _directCommandHistory;

        public ObservableCollection<string> DirectCommandHistory
        {
            get => _directCommandHistory;
            set
            {
                SetProperty(ref _directCommandHistory, value);
                RaisePropertyChanged(nameof(DirectCommandHistory));
            }
        }

        #endregion

        #region Commands / CanCommands

        public void SendDirect()
        {
            RunAndUpdate(() => { _global.Com.Current.QueueCommand(DirectCommand); });
            AddDirectCommandHistory(DirectCommand);
        }

        public bool CanSendDirectCommand()
        {
            return Connected && !string.IsNullOrEmpty(DirectCommand);
        }

        #endregion

        #region ICommand

        public ICommand SendDirectCommand => new DelegateCommand(SendDirect, CanSendDirectCommand);

        #endregion
    }
}