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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AutoMapper;
using CNCLib.GCode.GUI.ViewModels;
using CNCLib.GCode.GUI.Views;
using CNCLib.Wpf.ViewModels;
using Framework.Tools.Dependency;
using Framework.Wpf.Helpers;
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
            var vm = Dependency.Resolve<PreviewViewModel>();
            DataContext = vm;

            InitializeComponent();

            this.DefaulInitForBaseViewModel();

            ToggleSettings();

            Global.Instance.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Global.SizeX) || e.PropertyName == nameof(Global.SizeY))
                {
                    gcode.SizeX = (double) Global.Instance.SizeX;
                    gcode.SizeY = (double) Global.Instance.SizeY;
                }
            };

            if (vm.GetLoadInfo == null)
            {
                vm.GetLoadInfo = arg =>
                {
                    var dlg   = new LoadOptionView();
                    var vmdlg = dlg.DataContext as LoadOptionViewModel;
                    if (vmdlg != null)
                    {
                        vmdlg.LoadOptionsValue = Dependency.Resolve<IMapper>().Map<GCode.GUI.Models.LoadOptions>(arg.LoadOption);
                        vmdlg.UseAzure         = arg.UseAzure;
                        if (!dlg.ShowDialog() ?? false)
                        {
                            return false;
                        }

                        arg.LoadOption = Dependency.Resolve<IMapper>().Map<Logic.Contracts.DTO.LoadOptions>(vmdlg.LoadOptionsValue);
                        arg.UseAzure   = vmdlg.UseAzure;
                    }

                    return true;
                };
            }


            if (vm.RefreshPreview == null)
            {
                vm.RefreshPreview = () => { gcode.Dispatcher.Invoke(() => gcode.InvalidateVisual()); };
            }
        }

        private bool _isSettingsVisible = true;

        void ToggleSettings()
        {
            if (_isSettingsVisible)
            {
                _settings.Visibility = Visibility.Hidden;
                _settings.Width      = 0;
                _toggle.Content      = ">";
            }
            else
            {
                _settings.Visibility = Visibility.Visible;
                _settings.Width      = 100;
                _toggle.Content      = "<";
            }

            _isSettingsVisible = !_isSettingsVisible;
        }

        bool CanToggleSettings()
        {
            return true;
        }

        public ICommand ToggleSettingsCommand => new DelegateCommand(ToggleSettings, CanToggleSettings);
    }
}