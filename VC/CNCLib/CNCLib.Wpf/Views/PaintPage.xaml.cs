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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using CNCLib.GUI;

namespace CNCLib.Wpf.Views
{
	/// <summary>
	/// Interaction logic for PaintPage.xaml
	/// </summary>
	public partial class PaintPage : Page
	{
		public PaintPage()
		{
			InitializeComponent();
		}

		private void container_Loaded(object sender, RoutedEventArgs e)
		{
			if (_container.Child == null)
			{
				var f = new PaintForm();
				f.TopLevel = false;
				f.FormBorderStyle = FormBorderStyle.None;
				_container.Child = f;
			}
			((PaintForm)_container.Child).SetMachineSize();

		}
	}
}
