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

using Framework.Dependency;
using Framework.Wpf.ViewModels;
using MahApps.Metro.Controls;

namespace CNCLib.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            var vm = Dependency.Resolve<ViewModels.MainWindowViewModel>();
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

#warning TODO: trace 
//            DateTime now = DateTime.Now;
//		    Global.Instance.Com.LocalCom.Trace.EnableTrace($@"{System.IO.Path.GetTempPath()}CNCLibTrace_{now.Year:D4}{now.Month:D2}{now.Day:D2}_{now.Hour:D2}{now.Minute:D2}{now.Second:D2}.txt");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Global.Instance.Com.LocalCom.IsConnected)
            {
                Global.Instance.Com.LocalCom.DisconnectAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            }

#warning TODO: trace 
//            Global.Instance.Com.LocalCom.Trace.CloseTrace();

            if (Global.Instance.ComJoystick.IsConnected)
            {
                Global.Instance.ComJoystick.DisconnectAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            }

#warning TODO: trace 
//		    Global.Instance.ComJoystick.Trace.CloseTrace();
        }
    }
}