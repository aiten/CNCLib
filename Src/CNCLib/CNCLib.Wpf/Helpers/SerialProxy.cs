////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Arduino.SerialCommunication;

namespace CNCLib.Wpf.Helpers
{
    public class SerialProxy
    {
        public SerialProxy()
        {
            Current = LocalCom;
        }
        private Framework.Arduino.SerialCommunication.ISerial RemoteCom => Framework.Tools.Pattern.Singleton<CNCLib.Serial.Client.SerialService>.Instance;
        public Framework.Arduino.SerialCommunication.ISerial LocalCom => Framework.Tools.Pattern.Singleton<Framework.Arduino.SerialCommunication.Serial>.Instance;
        public Framework.Arduino.SerialCommunication.ISerial Current { get; private set; }

        public void SetCurrent(string portname)
        {
            if (portname.StartsWith("com"))
            {
                Current = LocalCom;
            }
            else
            {
                Current = RemoteCom;
            }
        }
    }
}