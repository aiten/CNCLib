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
using System.ComponentModel;
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
				Task _ = WatchTaskAsync(task);
			}
		}

		private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

/*
		private void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> projection)
		{
			var memberExpression = (MemberExpression)projection.Body;
			RaisePropertyChanged(memberExpression.Member.Name);
		}
*/

		private async Task WatchTaskAsync(Task task)
		{
			try
			{
				await task;
			}
		    catch
		    {
		        // ignored
		    }

		    RaisePropertyChanged(nameof(Status));
			RaisePropertyChanged(nameof(IsCompleted));
			RaisePropertyChanged(nameof(IsNotCompleted));
			if (task.IsCanceled)
			{
				RaisePropertyChanged(nameof(IsCanceled));
			}
			else if (task.IsFaulted)
			{
				RaisePropertyChanged(nameof(IsFaulted));
				RaisePropertyChanged(nameof(Exception));
				RaisePropertyChanged(nameof(InnerException));
				RaisePropertyChanged(nameof(ErrorMessage));
			}
			else
			{
				RaisePropertyChanged(nameof(IsSuccessfullyCompleted));
				RaisePropertyChanged(nameof(Result));
			}
		}
		public Task<TResult> Task { get; }
		public TResult Result => (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default(TResult);
	    public TaskStatus Status => Task.Status;
	    public bool IsCompleted => Task.IsCompleted;
	    public bool IsNotCompleted => !Task.IsCompleted;

	    public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;
	    public bool IsCanceled => Task.IsCanceled;
	    public bool IsFaulted => Task.IsFaulted;
	    public AggregateException Exception => Task.Exception;
	    public Exception InnerException => Exception?.InnerException;
	    public string ErrorMessage => InnerException?.Message;
	    public event PropertyChangedEventHandler PropertyChanged;
	}
}
