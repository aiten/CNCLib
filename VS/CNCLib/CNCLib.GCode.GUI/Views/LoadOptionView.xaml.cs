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
using System.IO;
using System.Windows;
using CNCLib.GCode.GUI.ViewModels;
using Framework.Wpf.ViewModels;

namespace CNCLib.GCode.GUI.Views
{
    /// <summary>
    /// Interaction logic for EepromView.xaml
    /// </summary>
    public partial class LoadOptionView : Window
	{
		public LoadOptionView()
		{
			InitializeComponent();

			var vm = DataContext as LoadOptionViewModel;
			if (vm.CloseAction == null)
				vm.CloseAction = new Action(() => this.Close());

            if (vm.DialogOKAction == null)
                vm.DialogOKAction = new Action(() => { this.DialogResult = true; this.Close(); });

            if (vm.DialogCancelAction == null)
                vm.DialogCancelAction = new Action(() => { this.DialogResult = false; this.Close(); });

            if (vm.MessageBox == null)
			{
				vm.MessageBox = new Func<string, string, MessageBoxButton, MessageBoxImage, MessageBoxResult>((messageBoxText, caption, button, icon) =>
				{
					return MessageBox.Show(messageBoxText, caption, button, icon);
				});
			}

            Loaded += new RoutedEventHandler(async (object v, RoutedEventArgs e) =>
            {
                var vmm = DataContext as BaseViewModel;
                await vmm.Loaded();
            });

            if (vm.BrowseFileNameFunc == null)
            {
                vm.BrowseFileNameFunc = new Func<string, bool, string>((string filename, bool savefile) =>
               {
                   Microsoft.Win32.FileDialog dlg;
                   if (savefile)
                   {
                       dlg = new Microsoft.Win32.SaveFileDialog();
                   }
                   else
                   {
                       dlg = new Microsoft.Win32.OpenFileDialog();
                   }
                   dlg.FileName = filename;
                   string dir = Path.GetDirectoryName(filename);
                   if (!string.IsNullOrEmpty(dir))
                   {
                       dlg.InitialDirectory = dir;
                       dlg.FileName = Path.GetFileName(filename);
                   }

                    if ((dlg.ShowDialog()??false))
                    {
                        return filename = dlg.FileName;
                    }
                    return null;
               });
            }
        }
    }
}
