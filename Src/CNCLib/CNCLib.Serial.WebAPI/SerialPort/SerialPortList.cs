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

using System;
using System.Collections.Generic;
using System.Linq;

using Framework.Arduino.SerialCommunication.Abstraction;
using Framework.Dependency;

namespace CNCLib.Serial.WebAPI.SerialPort
{
    public class SerialPortList
    {
        #region INTERNAL List

        public static ISerialPort SerialPort { get; private set; }

        public static SerialPortWrapper[] Ports { get; private set; } = GetPortDefinitions().ToArray();

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
                SerialPort = Dependency.Container.Resolve<ISerialPort>();
            }

            var portNames = SerialPort.GetPortNames();

            if (Environment.MachineName == "AIT7" && !portNames.Any())
            {
                portNames = new[] { "com0", "com1", "com3", "com4", "com5", "com6", "com10" };
            }

            return portNames.Select(
                (port, index) => new SerialPortWrapper()
                {
                    Id       = GetIdFromPortName(port, index),
                    PortName = port
                });
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

            Ports = newList.ToArray();
        }

        #endregion
    }
}