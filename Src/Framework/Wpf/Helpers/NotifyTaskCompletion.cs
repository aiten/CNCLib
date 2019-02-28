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

// see https://msdn.microsoft.com/magazine/dn605875

namespace Framework.Wpf.Helpers
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

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

        public TResult Result => Task.Status == TaskStatus.RanToCompletion ? Task.Result : default(TResult);

        public TaskStatus Status         => Task.Status;
        public bool       IsCompleted    => Task.IsCompleted;
        public bool       IsNotCompleted => !Task.IsCompleted;

        public bool                              IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;
        public bool                              IsCanceled              => Task.IsCanceled;
        public bool                              IsFaulted               => Task.IsFaulted;
        public AggregateException                Exception               => Task.Exception;
        public Exception                         InnerException          => Exception?.InnerException;
        public string                            ErrorMessage            => InnerException?.Message;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}