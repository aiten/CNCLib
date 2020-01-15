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

using System;
using System.Collections.Generic;
using System.Linq;

using Framework.Arduino.SerialCommunication.Abstraction;
using Framework.Dependency;

using Microsoft.Extensions.DependencyInjection;

namespace CNCLib.Serial.WebAPI.SerialPort
{
    public class SerialPortList
    {
        #region INTERNAL List

        public static ISerialPort SerialPort { get; private set; }

        public static IEnumerable<SerialPortWrapper> Ports { get; private set; } = GetPortDefinitions();

        public static SerialPortWrapper GetPort(int id)
        {
            var port = Ports.FirstOrDefault((s) => s.Id == id);
            port?.InitPort();
            return port;
        }

        private static int GetIdFromPortName(string portName, int index)
        {
            string portNo = portName.Remove(0, 3); // remove "com"
            if (int.TryParse(portNo, out int id))
            {
                return id;
            }

            return index;
        }

        private static IEnumerable<SerialPortWrapper> GetPortDefinitions()
        {
            if (SerialPort == null)
            {
                SerialPort = AppService.GetRequiredService<ISerialPort>();
            }

            var portNames = SerialPort.GetPortNames();

            return portNames.Select(
                (port, index) => new SerialPortWrapper()
                {
                    Id       = GetIdFromPortName(port, index),
                    PortName = port
                }).ToList();
        }

        public static void Refresh()
        {
            var currentPortDefinition = GetPortDefinitions().ToList();

            var newList = new List<SerialPortWrapper>();

            // add same(existing) portName
            foreach (var port in Ports)
            {
                var existingPort = currentPortDefinition.Find((p) => port.PortName == p.PortName);

                if (existingPort != null)
                {
                    newList.Add(port);
                }
            }

            //add new ports 
            foreach (var port in currentPortDefinition)
            {
                var existingPort = newList.Find((p) => port.PortName == p.PortName);
                if (existingPort == null)
                {
                    newList.Add(port);
                }
            }

            Ports = newList;
        }

        #endregion
    }
}