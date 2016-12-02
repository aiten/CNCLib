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

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Framework.Wpf.Helpers
{
    public class DelegateCommandAsync<T> : ICommand
    {
        private readonly Func<CancellationToken, Task<T>> _command;
        private readonly Func<bool> _canExecute;
		private readonly CancelAsyncCommand _cancelCommand = new CancelAsyncCommand();
		private NotifyTaskCompletion<T> _execution;

		public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

		protected void RaiseCanExecuteChanged()
		{
			CommandManager.InvalidateRequerySuggested();
		}

		public DelegateCommandAsync(Func<CancellationToken, Task<T>> command, Func<bool> canExecute = null)
        {
            if (command == null)
                throw new ArgumentNullException();
            _canExecute = canExecute;
            _command = command;
        }

        public async void Execute(object parameter)
        {
			_cancelCommand.NotifyCommandStarting();
			_execution = new NotifyTaskCompletion<T>(_command(_cancelCommand.Token));
			RaiseCanExecuteChanged();
//			await _execution.TaskCompletion;
			await _execution.Task;
			_cancelCommand.NotifyCommandFinished();
			RaiseCanExecuteChanged();
        }

		public ICommand CancelCommand
		{
			get { return _cancelCommand; }
		}

		public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

		private sealed class CancelAsyncCommand : ICommand
		{
			private CancellationTokenSource _cts = new CancellationTokenSource();
			private bool _commandExecuting;

			public event EventHandler CanExecuteChanged
			{
				add { CommandManager.RequerySuggested += value; }
				remove { CommandManager.RequerySuggested -= value; }
			}

			private void RaiseCanExecuteChanged()
			{
				CommandManager.InvalidateRequerySuggested();
			}

			public CancellationToken Token { get { return _cts.Token; } }
			public void NotifyCommandStarting()
			{
				_commandExecuting = true;
				if (!_cts.IsCancellationRequested)
					return;
				_cts = new CancellationTokenSource();
				RaiseCanExecuteChanged();
			}
			public void NotifyCommandFinished()
			{
				_commandExecuting = false;
				RaiseCanExecuteChanged();
			}
			public bool CanExecute(object parameter)
			{
				return _commandExecuting && !_cts.IsCancellationRequested;
			}
			public void Execute(object parameter)
			{
				_cts.Cancel();
				RaiseCanExecuteChanged();
			}
		}
	}
}