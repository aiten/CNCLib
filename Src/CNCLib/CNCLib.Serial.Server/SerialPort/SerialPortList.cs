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
using CNCLib.Serial.Server.Hubs;
using Framework.Tools.Dependency;
using Microsoft.AspNetCore.SignalR;

namespace CNCLib.Serial.Server.SerialPort
{
    public class SerialPortList
    {
        #region INTERNAL List

        public static SerialPortWrapper[] Ports { get; private set; } = GetPortDefinitions().ToArray();

        public static SerialPortWrapper GetPort(int id)
        {
            var port = Ports.FirstOrDefault((s) => s.Id == id);
            port?.InitPort();
            return port;
        }

        private static int GetIdFromPortName(string portname)
        {
            string portNo = portname.Remove(0,3); // remove "com"
            return (int) uint.Parse(portNo);
        }

        private static IEnumerable<SerialPortWrapper> GetPortDefinitions()
        {
            var portnames = System.IO.Ports.SerialPort.GetPortNames();

if (Environment.MachineName == "AIT7" && !portnames.Any())
    portnames = new string[] { "com1" };

            return portnames.Select((port, index) =>
            {
                var helper = Dependency.Resolve<SerialPortWrapper>();
                helper.Id = GetIdFromPortName(port);
                helper.PortName = port;
                return helper;
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
                    newlist.Add(existingport);
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