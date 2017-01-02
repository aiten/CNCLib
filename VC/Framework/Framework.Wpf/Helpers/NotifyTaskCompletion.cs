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
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

// see https://msdn.microsoft.com/magazine/dn605875

namespace Framework.Wpf.Helpers
{
    public sealed class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
	{
		public NotifyTaskCompletion(Task<TResult> task)
		{
			Task = task;
			if (!task.IsCompleted)
			{
				var _ = WatchTaskAsync(task);
			}
		}

		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void OnPropertyChanged<TProperty>(Expression<Func<TProperty>> projection)
		{
			var memberExpression = (MemberExpression)projection.Body;
			OnPropertyChanged(memberExpression.Member.Name);
		}


		private async Task WatchTaskAsync(Task task)
		{
			try
			{
				await task;
			}
			catch
			{
			}

			OnPropertyChanged(() => Status);
			OnPropertyChanged(() => IsCompleted);
			OnPropertyChanged(() => IsNotCompleted);
			if (task.IsCanceled)
			{
				OnPropertyChanged(() => IsCanceled);
			}
			else if (task.IsFaulted)
			{
				OnPropertyChanged(() => IsFaulted);
				OnPropertyChanged(() => Exception);
				OnPropertyChanged(() => InnerException);
				OnPropertyChanged(() => ErrorMessage);
			}
			else
			{
				OnPropertyChanged(() => IsSuccessfullyCompleted);
				OnPropertyChanged(() => Result);
			}
		}
		public Task<TResult> Task { get; private set; }
		public TResult Result
		{
			get
			{
				return (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default(TResult);
			}
		}
		public TaskStatus Status { get { return Task.Status; } }
		public bool IsCompleted { get { return Task.IsCompleted; } }
		public bool IsNotCompleted { get { return !Task.IsCompleted; } }
		public bool IsSuccessfullyCompleted
		{
			get
			{
				return Task.Status == TaskStatus.RanToCompletion;
			}
		}
		public bool IsCanceled { get { return Task.IsCanceled; } }
		public bool IsFaulted { get { return Task.IsFaulted; } }
		public AggregateException Exception { get { return Task.Exception; } }
		public Exception InnerException
		{
			get
			{
				return (Exception == null) ? null : Exception.InnerException;
			}
		}
		public string ErrorMessage
		{
			get
			{
				return (InnerException == null) ? null : InnerException.Message;
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
