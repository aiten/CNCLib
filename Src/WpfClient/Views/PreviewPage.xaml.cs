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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using CNCLib.GCode.GUI.ViewModels;
using CNCLib.GCode.GUI.Views;
using CNCLib.WpfClient.ViewModels;

using Framework.Dependency;
using Framework.Wpf.Helpers;
using Framework.Wpf.Views;

using Microsoft.Extensions.DependencyInjection;

namespace CNCLib.WpfClient.Views
{
    /// <summary>
    /// Interaction logic for PreviewPage.xaml
    /// </summary>
    public partial class PreviewPage : Page
    {
        public PreviewPage()
        {
            var vm = AppService.GetRequiredService<PreviewViewModel>();
            DataContext = vm;

            InitializeComponent();

            this.DefaultInitForBaseViewModel();

            ToggleSettings();

            vm.Global.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Global.SizeX) || e.PropertyName == nameof(Global.SizeY))
                {
                    gcode.SizeX = (double)vm.Global.SizeX;
                    gcode.SizeY = (double)vm.Global.SizeY;
                }
            };

            if (vm.GetLoadInfo == null)
            {
                vm.GetLoadInfo = arg =>
                {
                    var dlg       = new LoadOptionView();
                    var viewModel = dlg.DataContext as LoadOptionViewModel;
                    if (viewModel != null)
                    {
                        viewModel.LoadOptionsValue = viewModel.MapLoadOptions(arg.LoadOption);
                        viewModel.UseAzure         = arg.UseAzure;
                        if (!dlg.ShowDialog() ?? false)
                        {
                            return false;
                        }

                        arg.LoadOption = viewModel.MapLoadOptions(viewModel.LoadOptionsValue);
                        arg.UseAzure   = viewModel.UseAzure;
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