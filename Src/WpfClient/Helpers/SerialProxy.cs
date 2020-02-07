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

using Framework.Arduino.SerialCommunication;
using Framework.Arduino.SerialCommunication.Abstraction;
using Framework.Dependency;
using Framework.Pattern;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CNCLib.WpfClient.Helpers
{
    public class SerialProxy
    {
        public SerialProxy()
        {
            Current = LocalCom;
        }

        public ISerial RemoteCom => Singleton<Serial.Client.SerialService>.Instance;

        private static Framework.Arduino.SerialCommunication.Serial _localSerial =
            new Framework.Arduino.SerialCommunication.Serial(
                new FactoryCreate<ISerialPort>(() => new SerialPort()),
                AppService.GetRequiredService<ILogger<Framework.Arduino.SerialCommunication.Serial>>());

        public ISerial LocalCom => _localSerial;

        public ISerial Current { get; private set; }

        public void SetCurrent(string serverName)
        {
            Current = string.IsNullOrEmpty(serverName) ? LocalCom : RemoteCom;
        }
    }
}