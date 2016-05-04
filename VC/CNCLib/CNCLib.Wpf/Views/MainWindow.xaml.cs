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

namespace CNCLib.Wpf.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
            DateTime now = DateTime.Now;
			Com.Trace.EnableTrace(string.Format(@"{0}CNCLibTrace_{1:D4}{2:D2}{3:D2}_{4:D2}{5:D2}{6:D2}.txt",
                    System.IO.Path.GetTempPath(),
                    now.Year,now.Month,now.Day,now.Hour,now.Minute,now.Second));
		}
		private Framework.Arduino.ArduinoSerialCommunication Com
		{
			get { return Framework.Tools.Pattern.Singleton<Framework.Arduino.ArduinoSerialCommunication>.Instance; }
		}
        private Framework.Arduino.ArduinoSerialCommunication ComJoystick
        {
            get { return Framework.Tools.Pattern.Singleton<Wpf.Helpers.JoystickArduinoSerialCommunication>.Instance; }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (Com.IsConnected)
				Com.Disconnect();

			Com.Trace.CloseTrace();

            if (ComJoystick.IsConnected)
                ComJoystick.Disconnect();

            ComJoystick.Trace.CloseTrace();
        }
    }
}
