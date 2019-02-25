/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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

using System.Windows;

using Framework.Dependency;
using Framework.Wpf.ViewModels;

namespace CNCLib.WpfClient.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class JoystickView : Window
    {
        public JoystickView()
        {
            var vm = Dependency.Resolve<ViewModels.JoystickViewModel>();
            DataContext = vm;

            InitializeComponent();

            if (vm.CloseAction == null)
            {
                vm.CloseAction = Close;
            }

            Loaded += async (v, e) =>
            {
                var vmm = DataContext as BaseViewModel;
                if (vmm != null)
                {
                    await vmm.Loaded();
                }
            };
        }
    }
}