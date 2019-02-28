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
using System.Collections.ObjectModel;
using System.Windows.Input;

using CNCLib.WpfClient.Models;

using Framework.Wpf.Helpers;

namespace CNCLib.WpfClient.ViewModels.ManualControl
{
    public class CommandHistoryViewModel : DetailViewModel
    {
        private readonly Global _global;

        public CommandHistoryViewModel(IManualControlViewModel vm, Global global) : base(vm, global)
        {
            _global = global ?? throw new ArgumentNullException(); ;
        }

        public const string CommandHistoryFile = @"%USERPROFILE%\Documents\Command.txt";

        #region Properties

        private ObservableCollection<SentCNCCommand> _commandHistoryCollection;

        public ObservableCollection<SentCNCCommand> CommandHistoryCollection
        {
            get => _commandHistoryCollection;
            set => SetProperty(ref _commandHistoryCollection, value);
        }

        #endregion

        #region Commands / CanCommands

        public void RefreshAfterCommand()
        {
            RefreshCommandHistory();
        }

        public void RefreshCommandHistory()
        {
            var results = new ObservableCollection<SentCNCCommand>();

            foreach (var rc in _global.Com.Current.CommandHistoryCopy)
            {
                DateTime sentTime = rc.SentTime ?? DateTime.Today;

                results.Add(
                    new SentCNCCommand
                    {
                        CommandDate = sentTime,
                        CommandText = rc.CommandText,
                        Result      = rc.ResultText
                    });
            }

            CommandHistoryCollection = results;
        }

        public void ClearCommandHistory()
        {
            _global.Com.Current.ClearCommandHistory();
            RefreshCommandHistory();
        }

        #endregion

        #region ICommand

        public ICommand RefreshHistoryCommand => new DelegateCommand(RefreshCommandHistory, CanSend);
        public ICommand ClearHistoryCommand   => new DelegateCommand(ClearCommandHistory,   CanSend);

        #endregion
    }
}