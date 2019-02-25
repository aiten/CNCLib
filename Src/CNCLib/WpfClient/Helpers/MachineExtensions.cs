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

using System.Linq;

using CNCLib.Logic.Contract.DTO;

namespace CNCLib.WpfClient.Helpers
{
    public static class MachineExtension
    {
        public static string PrepareCommand(this Machine machine, string commandString)
        {
            string prefix = machine.GetCommandPrefix();

            if (string.IsNullOrEmpty(prefix))
            {
                return commandString;
            }

            return prefix + commandString;
        }

        public static string GetCommandPrefix(this Machine machine)
        {
            var prefix = machine.CommandSyntax;
            if (prefix == CommandSyntax.HPGL)
            {
                return ((char)27).ToString();
            }

            return null;
        }


        public static string JoystickReplyReceived(this Machine machine, string trim)
        {
            // ;btn5		=> look for ;btn5
            // ;btn5:x		=> x is pressCount - always incremented, look for max x in setting => modulo 

            int idx;

            if ((idx = trim.IndexOf(':')) < 0)
            {
                var mc = machine.MachineCommands.FirstOrDefault(m => m.JoystickMessage == trim);
                if (mc != null)
                {
                    trim = mc.CommandString;
                }
            }
            else
            {
                string btn = trim.Substring(0, idx + 1);
                var machineCommands = machine.MachineCommands.Where(m => m.JoystickMessage?.Length > idx && m.JoystickMessage.Substring(0, idx + 1) == btn).ToList();

                uint max = 0;
                foreach (var m in machineCommands)
                {
                    uint val;
                    if (uint.TryParse(m.JoystickMessage.Substring(idx + 1), out val))
                    {
                        if (val > max)
                        {
                            max = val;
                        }
                    }
                }

                string findCmd = $"{btn}{uint.Parse(trim.Substring(idx + 1)) % (max + 1)}";

                var mc = machine.MachineCommands.FirstOrDefault(m => m.JoystickMessage == findCmd);
                if (mc == null)
                {
                    // try to find ;btn3 (without :)  
                    findCmd = trim.Substring(0, idx);
                    mc = machine.MachineCommands.FirstOrDefault(m => m.JoystickMessage == findCmd);
                }

                if (mc != null)
                {
                    trim = mc.CommandString;
                }
            }

            return trim;
        }
    }
}