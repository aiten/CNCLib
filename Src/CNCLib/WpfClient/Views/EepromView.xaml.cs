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

using CNCLib.WpfClient.Models;

using Framework.Dependency;
using Framework.Wpf.Views;

using Xceed.Wpf.Toolkit.PropertyGrid;

namespace CNCLib.WpfClient.Views
{
    /// <summary>
    /// Interaction logic for EepromView.xaml
    /// </summary>
    public partial class EepromView : Window
    {
        public EepromView()
        {
            var vm = Dependency.Resolve<ViewModels.EepromViewModel>();
            DataContext = vm;

            InitializeComponent();

            this.DefaultInitForBaseViewModel();
        }

        private void PropertyGrid_PreparePropertyItem(object sender, PropertyItemEventArgs e)
        {
            var grid = (PropertyGrid)e.Source;
        }

        private void PropertyGrid_IsPropertyBrowseable(object sender, IsPropertyBrowsableArgs e)
        {
            if (_grid.SelectedObject != null)
            {
                var data = (Eeprom)_grid.SelectedObject;
                e.IsBrowsable = data.IsPropertyBrowsable(e.PropertyDescriptor);
            }
        }
    }
}