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

namespace Framework.Wpf.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    using Helpers;

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

        public ICommand DialogOKCommand     => new DelegateCommand(DialogOK,     CanDialogOK);
        public ICommand DialogCancelCommand => new DelegateCommand(DialogCancel, CanDialogCancel);

        #endregion

        public bool IsDesignTime => System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());

        #region GUI Forwards

        public Func<string, string, MessageBoxButton, MessageBoxImage, MessageBoxResult> MessageBox { get; set; }

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