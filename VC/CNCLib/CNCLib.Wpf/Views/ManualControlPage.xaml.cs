////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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
using System.Windows.Controls;
using CNCLib.Wpf.ViewModels;
using Framework.Wpf.ViewModels;

namespace CNCLib.Wpf
{
	/// <summary>
	/// Interaction logic for ManualControl.xaml
	/// </summary>
	public partial class ManualControlPage : Page
    {
        public ManualControlPage()
        {
            InitializeComponent();

			var vm = DataContext as BaseViewModel;
		}

		public bool IsConnected
        {
            get {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) return vm.Com.IsConnected;
                return false;
            }
        }

        #region Other

		private void BrowseFileOpenDialog_Click(object sender, RoutedEventArgs e)
		{
			// Configure open file dialog box
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			//dlg.FileName = "Document"; // Default file name
			//dlg.DefaultExt = ".txt"; // Default file extension
			//dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

			// Show open file dialog box
			Nullable<bool> result = dlg.ShowDialog();

			// Process open file dialog box results
			if (result == true)
			{
				// Open document
				string filename = dlg.FileName;
				var vm = DataContext as ManualControlViewModel;
				if (vm != null) vm.SD.FileName = filename;
			}
		}

        #endregion
  
    }
}
