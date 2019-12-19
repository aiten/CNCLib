﻿/*
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

using System.Threading.Tasks;

using Framework.Dependency;
using Framework.Wpf.ViewModels;

using MahApps.Metro.Controls;

using Microsoft.Extensions.DependencyInjection;

namespace CNCLib.WpfClient.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            var vm = AppService.GetRequiredService<ViewModels.MainWindowViewModel>();
            DataContext = vm;

            InitializeComponent();

            Loaded += async (v, e) =>
            {
                var vmm = DataContext as BaseViewModel;
                if (vmm != null)
                {
                    await vmm.Loaded();
                }
            };
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Windows_ClosingAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private async Task Windows_ClosingAsync()
        {
            var global = AppService.GetRequiredService<Global>();

            if (global.Com.LocalCom.IsConnected)
            {
                await global.Com.LocalCom.DisconnectAsync();
            }

            if (global.ComJoystick.IsConnected)
            {
                await global.ComJoystick.DisconnectAsync();
            }
        }
    }
}