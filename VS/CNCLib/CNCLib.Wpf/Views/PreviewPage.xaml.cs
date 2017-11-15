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
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using AutoMapper;
using CNCLib.GCode.GUI.ViewModels;
using CNCLib.GCode.GUI.Views;
using CNCLib.Wpf.ViewModels;
using Framework.Tools.Dependency;
using Framework.Wpf.View;

namespace CNCLib.Wpf.Views
{
    /// <summary>
    /// Interaction logic for PreviewPage.xaml
    /// </summary>
    public partial class PreviewPage : Page
	{
		public PreviewPage()
		{
			InitializeComponent();

            this.DefaulInitForBaseViewModel();

            Global.Instance.PropertyChanged += (object sender, PropertyChangedEventArgs e) => 
            {
                if (e.PropertyName == nameof(Global.SizeX) || e.PropertyName == nameof(Global.SizeY))
                {
                    gcode.SizeX = (double) Global.Instance.SizeX;
                    gcode.SizeY = (double) Global.Instance.SizeY;
                };
            };

            var vm = DataContext as PreviewViewModel;

			if (vm.GetLoadInfo == null)
				vm.GetLoadInfo = new Func<PreviewViewModel.GetLoadInfoArg, bool>((arg) =>
				{
                    var dlg = new LoadOptionView();
                    var vmdlg = dlg.DataContext as LoadOptionViewModel;
                    vmdlg.LoadOptionsValue = Dependency.Resolve<IMapper>().Map<CNCLib.GCode.GUI.Models.LoadOptions>(arg.LoadOption);
                    vmdlg.UseAzure = arg.UseAzure;
                    if (!dlg.ShowDialog() ?? false)
                        return false;

                    arg.LoadOption = Dependency.Resolve<IMapper>().Map<CNCLib.Logic.Contracts.DTO.LoadOptions>(vmdlg.LoadOptionsValue);
                    arg.UseAzure = vmdlg.UseAzure;
                    return true;
                });


			if (vm.RefreshPreview == null)
			{
				vm.RefreshPreview = new Action(() =>
				{
					gcode.Dispatcher.Invoke(() => gcode.InvalidateVisual());
				});
			}
		}
	}
}
