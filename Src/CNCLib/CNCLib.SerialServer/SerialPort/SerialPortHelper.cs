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
using System.Runtime.InteropServices;
using CNCLib.SerialServer.Controllers;
using Framework.Arduino.SerialCommunication;

namespace CNCLib.SerialServer.SerialPort
{
    public class SerialPortHelper
    {
        public int Id { get; set; }

        public string PortName { get; set; }

        public Serial Serial { get; set; } = new Serial();


        #region INTERNAL List

        public static SerialPortHelper[] Ports { get; private set; } = GetPortDefinitions().ToArray();

        private static IEnumerable<SerialPortHelper> GetPortDefinitions()
        {
            var portnames = System.IO.Ports.SerialPort.GetPortNames();
            return portnames.Select((port, index) => new SerialPortHelper() {Id = index + 1, PortName = port});
        }

        public static void Refresh()
        {
            // keep same id
            var maxid = Ports.Max((e) => e.Id);

            var currentportdefinition = GetPortDefinitions().ToList();

            var newlist = new List<SerialPortHelper>();

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
            int nextid = 1;
            foreach (var port in currentportdefinition)
            {
                var existingport = newlist.Find((p) => port.PortName == p.PortName);
                if (existingport == null)
                {
                    while (newlist.Any((e) => e.Id == nextid))
                    {
                        nextid++;
                    }
                    newlist.Add(new SerialPortHelper() {Id = nextid, PortName = port.PortName});
                }
            }

            Ports = newlist.ToArray();


            #endregion

        }
    }
}