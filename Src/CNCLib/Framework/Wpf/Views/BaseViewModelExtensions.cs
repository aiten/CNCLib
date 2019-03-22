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

namespace Framework.Wpf.Views
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;

    using ViewModels;

    public static class BaseViewModelExtensions
    {
        public static void DefaultInitForBaseViewModel(this BaseViewModel vm)
        {
            if (vm.MessageBox == null)
            {
                vm.MessageBox = MessageBox.Show;
            }

            if (vm.BrowseFileNameFunc == null)
            {
                vm.BrowseFileNameFunc = (filename, saveFile) =>
                {
                    Microsoft.Win32.FileDialog dlg;
                    if (saveFile)
                    {
                        dlg = new Microsoft.Win32.SaveFileDialog();
                    }
                    else
                    {
                        dlg = new Microsoft.Win32.OpenFileDialog();
                    }

                    dlg.FileName = filename;
                    string dir = Path.GetDirectoryName(filename);
                    if (!string.IsNullOrEmpty(dir))
                    {
                        dlg.InitialDirectory = dir;
                        dlg.FileName         = Path.GetFileName(filename);
                    }

                    if (dlg.ShowDialog() ?? false)
                    {
                        return dlg.FileName;
                    }

                    return null;
                };
            }
        }

        public static void DefaultInitForBaseViewModel(this Window view)
        {
            var vm = view.DataContext as BaseViewModel;

            if (vm != null)
            {
                vm.DefaultInitForBaseViewModel();

                var closeAction = new Action(view.Close);
                var dialogOkAction = new Action(
                    () =>
                    {
                        view.DialogResult = true;
                        view.Close();
                    });
                var dialogCancelAction = new Action(
                    () =>
                    {
                        view.DialogResult = false;
                        view.Close();
                    });
                var loadedEvent = new RoutedEventHandler(
                    async (v, e) =>
                    {
                        var vmm = view.DataContext as BaseViewModel;
                        if (vmm != null)
                        {
                            await vmm.Loaded();
                        }
                    });

                RoutedEventHandler unloadedEvent = null;

                unloadedEvent = (v, e) =>
                {
                    vm.CloseAction        =  null;
                    vm.DialogOKAction     =  null;
                    vm.DialogCancelAction =  null;
                    view.Loaded           -= loadedEvent;
                    view.Unloaded         -= unloadedEvent;
                };

                vm.CloseAction        = closeAction;
                vm.DialogOKAction     = dialogOkAction;
                vm.DialogCancelAction = dialogCancelAction;

                view.Loaded   += loadedEvent;
                view.Unloaded += unloadedEvent;
            }
        }

        public static void DefaultInitForBaseViewModel(this Page view)
        {
            var vm = view.DataContext as BaseViewModel;

            if (vm != null)
            {
                vm.DefaultInitForBaseViewModel();

                view.Loaded += async (v, e) =>
                {
                    var vmm = view.DataContext as BaseViewModel;
                    if (vmm != null)
                    {
                        await vmm.Loaded();
                    }
                };
            }
        }
    }
}