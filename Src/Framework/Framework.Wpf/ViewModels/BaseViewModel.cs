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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Framework.Wpf.Helpers;

namespace Framework.Wpf.ViewModels
{
    public class BaseViewModel : BindableBase
    {

        #region ModalDialogs

        /// <summary>
        /// Close this window an set DialogResult to true
        /// </summary>
        public Action DialogOKAction { get; set; }
        public Action DialogCancelAction { get; set; }

        protected virtual bool CanDialogOK()
        {
            return true;
        }
        protected virtual bool CanDialogCancel()
        {
            return true;
        }
        protected virtual void DialogOK()
        {
            DialogOKAction?.Invoke();
        }
        protected virtual void DialogCancel()
        {
            DialogCancelAction?.Invoke();
        }

        public ICommand DialogOKCommand => new DelegateCommand(DialogOK, CanDialogOK);
        public ICommand DialogCancelCommand => new DelegateCommand(DialogCancel, CanDialogCancel);

        #endregion

        public bool IsDesignTime => System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());

        #region GUI Forwards

        public Func<string, string, MessageBoxButton, MessageBoxImage, MessageBoxResult>MessageBox{ get; set; }

        public Func<string, bool, string> BrowseFileNameFunc { get; set; }

        /// <summary>
        /// Close this window
        /// </summary>
		public Action CloseAction { get; set; }

        #endregion

        public virtual void Cleanup()
		{

		}
		public virtual async Task Loaded()
		{
			await Task.FromResult(0);
		}
    }
}