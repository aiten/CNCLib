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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Framework.Wpf.ViewModels;

namespace Framework.Wpf.View
{
    public static class BaseViewModelExtensions
    {
        public static void DefaulInitForBaseViewModel(this Window view)
        {
            var vm = view.DataContext as BaseViewModel;

            if (vm != null)
            {
                if (vm.CloseAction == null)
                    vm.CloseAction = new Action(() => view.Close());

                if (vm.DialogOKAction == null)
                    vm.DialogOKAction = new Action(() => { view.DialogResult = true; view.Close(); });

                if (vm.DialogCancelAction == null)
                    vm.DialogCancelAction = new Action(() => { view.DialogResult = false; view.Close(); });

                if (vm.MessageBox == null)
                {
                    vm.MessageBox = new Func<string, string, MessageBoxButton, MessageBoxImage, MessageBoxResult>((messageBoxText, caption, button, icon) =>
                    {
                        return MessageBox.Show(messageBoxText, caption, button, icon);
                    });
                }

                view.Loaded += new RoutedEventHandler(async (object v, RoutedEventArgs e) =>
                {
                    var vmm = view.DataContext as BaseViewModel;
                    await vmm.Loaded();
                });
            }
        }
        public static void DefaulInitForBaseViewModel(this Page view)
        {
            var vm = view.DataContext as BaseViewModel;

            if (vm != null)
            {
/*
                if (vm.CloseAction == null)
                    vm.CloseAction = new Action(() => view.Close());

                if (vm.DialogOKAction == null)
                    vm.DialogOKAction = new Action(() => { view.DialogResult = true; view.Close(); });

                if (vm.DialogCancelAction == null)
                    vm.DialogCancelAction = new Action(() => { view.DialogResult = false; view.Close(); });
*/
                if (vm.MessageBox == null)
                {
                    vm.MessageBox = new Func<string, string, MessageBoxButton, MessageBoxImage, MessageBoxResult>((messageBoxText, caption, button, icon) =>
                    {
                        return MessageBox.Show(messageBoxText, caption, button, icon);
                    });
                }

                view.Loaded += new RoutedEventHandler(async (object v, RoutedEventArgs e) =>
                {
                    var vmm = view.DataContext as BaseViewModel;
                    await vmm.Loaded();
                });
            }
        }
    }
}
