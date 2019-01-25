/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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

using System.ComponentModel;

namespace CNCLib.Wpf.Models
{
    public class Joystick
    {
        const string CATEGORY_INTERNAL      = "Internal";
        const string CATEGORY_GENERAL       = "General";
        const string CATEGORY_COMMUNICATION = "Communication";

        [Category(CATEGORY_COMMUNICATION)]
        [DisplayName("ComPort")]
        [Description("Com of attached joystick")]
        public string ComPort { get; set; }

        [Category(CATEGORY_COMMUNICATION)]
        [DisplayName("BaudRate")]
        [Description("BaudRate")]
        public int BaudRate { get; set; }

        [Category(CATEGORY_GENERAL)]
        [DisplayName("InitCommands")]
        [Description(@"Commands sent to the joystick after connected. Seperate commands with \n")]
        public string InitCommands { get; set; }
    }
}