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

using System.Linq;

using CNCLib.Wpf.Models;

using Framework.Arduino.SerialCommunication;

using CNCLib.Logic.Client;

using System.Threading.Tasks;
using System;

using Framework.Dependency;

namespace CNCLib.Wpf.Helpers
{
    public class JoystickHelper
    {
        internal static async Task<Tuple<Joystick, int>> Load()
        {
            using (var controller = Dependency.Resolve<IDynItemController>())
            {
                var joystick = await controller.GetAll(typeof(Joystick));
                if (joystick != null && joystick.Any())
                {
                    int id = joystick.First().ItemId;
                    return new Tuple<Joystick, int>((Joystick) await controller.Create(id), id);
                }
            }

            return new Tuple<Joystick, int>(new Joystick { BaudRate = 250000, ComPort = @"com7" }, -1);
        }

        internal static async Task<int> Save(Joystick joystick, int id)
        {
            using (var controller = Dependency.Resolve<IDynItemController>())
            {
                if (id >= 0)
                {
                    await controller.Save(id, "Joystick", joystick);
                    return id;
                }

                return await controller.Add("Joystick", joystick);
            }
        }

        public void JoystickReplyReceived(string trim)
        {
            // ;btn5		=> look for ;btn5
            // ;btn5:x		=> x is pressCount - always incremented, look for max x in setting => modulo 

            int idx;

            if ((idx = trim.IndexOf(':')) < 0)
            {
                var mc = Global.Instance.Machine.MachineCommands.FirstOrDefault(m => m.JoystickMessage == trim);
                if (mc != null)
                {
                    trim = mc.CommandString;
                }
            }
            else
            {
                string btn    = trim.Substring(0, idx + 1);
                var    machineCommands = Global.Instance.Machine.MachineCommands.Where(m => m.JoystickMessage?.Length > idx && m.JoystickMessage.Substring(0, idx + 1) == btn).ToList();

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

                var mc = Global.Instance.Machine.MachineCommands.FirstOrDefault(m => m.JoystickMessage == findCmd);
                if (mc == null)
                {
                    // try to find ;btn3 (without :)  
                    findCmd = trim.Substring(0, idx);
                    mc      = Global.Instance.Machine.MachineCommands.FirstOrDefault(m => m.JoystickMessage == findCmd);
                }

                if (mc != null)
                {
                    trim = mc.CommandString;
                }
            }

            new MachineGCodeHelper().SendCommandAsync(trim).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task SendInitCommands(string commandString)
        {
            string[] separators = { @"\n" };
            string[] cmds       = commandString.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in cmds)
            {
                await Global.Instance.ComJoystick.SendCommandAsync(s, int.MaxValue);
            }
        }
    }
}