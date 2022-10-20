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

namespace CNCLib.WpfClient.Helpers;

using System.Linq;

using CNCLib.Logic.Abstraction.DTO;

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
        if (prefix == CommandSyntax.Hpgl)
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
            string btn             = trim.Substring(0, idx + 1);
            var    machineCommands = machine.MachineCommands.Where(m => m.JoystickMessage?.Length > idx && m.JoystickMessage.Substring(0, idx + 1) == btn).ToList();

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
                mc      = machine.MachineCommands.FirstOrDefault(m => m.JoystickMessage == findCmd);
            }

            if (mc != null)
            {
                trim = mc.CommandString;
            }
        }

        return trim;
    }
}