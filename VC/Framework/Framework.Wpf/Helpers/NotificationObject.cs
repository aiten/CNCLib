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
		protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null) where T : IComparable
		{
			if (object.Equals(storage, value)) return false;	// ref equal
			if (storage != null && storage.CompareTo(value) == 0) return false;	// logical equal

			storage = value;
			this.OnPropertyChanged(propertyName);
			return true;
		}

		// AssignProperty, value may be the same
		protected bool AssignProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
		{
			storage = value;
			this.OnPropertyChanged(propertyName);
			return true;
		}

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var eventHandler = this.PropertyChanged;
			if (eventHandler != null)
			{
				eventHandler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		protected void OnPropertyChanged<TProperty>(Expression<Func<TProperty>> projection)
		{
			var memberExpression = (MemberExpression)projection.Body;
			OnPropertyChanged(memberExpression.Member.Name);
		}
	}
}
