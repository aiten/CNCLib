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

using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Arduino.SerialCommunication;
using Framework.Tools.Dependency;

namespace CNCLib.Serial.Server.SerialPort
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

        private static int GetIdFromPortName(string portname,int index)
        {
            string portNo = portname.Remove(0,3); // remove "com"
            if (int.TryParse(portNo, out int id))
                return id;
            return index;
        }

        private static IEnumerable<SerialPortWrapper> GetPortDefinitions()
        {
            if (SerialPort == null)
            {
                SerialPort = Dependency.Container.Resolve<ISerialPort>();
            }

            var portnames = SerialPort.GetPortNames();

            if (Environment.MachineName == "AIT7" && !portnames.Any())
                portnames = new string[] { "com0", "com1", "com3", "com4", "com5", "com6", "com10" };

            return portnames.Select((port, index) =>
            {
                return new SerialPortWrapper()
                {
                    Id = GetIdFromPortName(port,index),
                    PortName = port
                };
            } );
        }

        public static void Refresh()
        {
            var currentportdefinition = GetPortDefinitions().ToList();

            var newlist = new List<SerialPortWrapper>();

            // add same(existing) portname
            foreach (var port in Ports)
            {
                var existingport = currentportdefinition.Find((p) => port.PortName == p.PortName);

                if (existingport != null)
                {
                    newlist.Add(port);
                }
            }

            //addnew ports 
            foreach (var port in currentportdefinition)
            {
                var existingport = newlist.Find((p) => port.PortName == p.PortName);
                if (existingport == null)
                {
                    newlist.Add(port);
                }
            }

            Ports = newlist.ToArray();
        }

        #endregion
    }
}