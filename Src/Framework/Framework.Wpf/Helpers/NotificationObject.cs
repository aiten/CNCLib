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
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Framework.Wpf.Helpers
{
	public class NotificationObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		// Set Property if value is different
		protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null) where T : IComparable
		{
			if (Equals(storage, value)) return false;	// ref equal
			if (storage != null && storage.CompareTo(value) == 0) return false;	// logical equal

            storage = value;
			RaisePropertyChanged(propertyName);
			return true;
		}

        protected bool SetProperty(Func<bool> equal, Action action,  [CallerMemberName] string propertyName = null)
        {
            if (equal())
            {
                return false;
            }

            action();
            RaisePropertyChanged(propertyName);

            return true;
        }

		protected void OnProperty(Action action, [CallerMemberName] string propertyName = null)
		{
			action();
			RaisePropertyChanged(propertyName);
		}

		protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

		protected void OnPropertyChanged<TProperty>(Expression<Func<TProperty>> projection)
		{
			var memberExpression = (MemberExpression)projection.Body;
			RaisePropertyChanged(memberExpression.Member.Name);
		}
	}
}
