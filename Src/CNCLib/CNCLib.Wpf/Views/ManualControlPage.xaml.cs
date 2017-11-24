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

using System.Windows.Controls;
using CNCLib.Wpf.ViewModels;
using Framework.Tools.Dependency;
using Framework.Wpf.View;

namespace CNCLib.Wpf.Views
{
    /// <summary>
    /// Interaction logic for ManualControl.xaml
    /// </summary>
    public partial class ManualControlPage : Page
    {
        public ManualControlPage()
        {
            var vm = Dependency.Resolve<ManualControlViewModel>();
            DataContext = vm;

            InitializeComponent();

            vm.SD.DefaulInitForBaseViewModel();
            this.DefaulInitForBaseViewModel();
        }

        public bool IsConnected
        {
            get
            {
                var vm = DataContext as ManualControlViewModel;
                if (vm != null) return vm.Com.IsConnected;
                return false;
            }
        }
    }
}
