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

namespace CNCLib.WpfClient.Views;

using System.Windows;

using CNCLib.GCode.Machine;

using Framework.Dependency;
using Framework.Wpf.Views;

using Xceed.Wpf.Toolkit.PropertyGrid;

/// <summary>
/// Interaction logic for EepromView.xaml
/// </summary>
public partial class EepromView : Window
{
    public EepromView()
    {
        var vm = AppService.GetRequiredService<ViewModels.EepromViewModel>();
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