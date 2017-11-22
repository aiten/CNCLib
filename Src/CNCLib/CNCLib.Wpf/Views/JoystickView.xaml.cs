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
using System.Windows;
using Framework.Tools.Dependency;
using Framework.Wpf.ViewModels;

namespace CNCLib.Wpf.Views
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
				vm.CloseAction = new Action(() => Close());

			Loaded += new RoutedEventHandler(async (object v, RoutedEventArgs e) =>
			{
				var vmm = DataContext as BaseViewModel;
				await vmm.Loaded();
			});
		}
	}
}
