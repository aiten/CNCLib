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

namespace CNCLib.WpfClient.ViewModels.ManualControl;

using System.Collections.ObjectModel;
using System.Windows.Input;

using Framework.Arduino.SerialCommunication;
using Framework.Wpf.Helpers;

public class DirectCommandViewModel : DetailViewModel
{
    private readonly Global _global;

    public DirectCommandViewModel(IManualControlViewModel vm, Global global) : base(vm, global)
    {
        _global = global;
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