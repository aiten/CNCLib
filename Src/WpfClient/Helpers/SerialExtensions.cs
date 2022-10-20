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

using System;
using System.Linq;
using System.Media;
using System.Threading.Tasks;

using CNCLib.GCode.Serial;
using CNCLib.GCode.Tools;

using Framework.Arduino.SerialCommunication;
using Framework.Arduino.SerialCommunication.Abstraction;

using Machine = CNCLib.Logic.Abstraction.DTO.Machine;

public static class SerialExtension
{
    #region Probe

    public static async Task<bool> SendProbeCommandAsync(this ISerial serial, Machine machine, int axisIndex)
    {
        return await serial.SendProbeCommandAsync(machine.GetAxisName(axisIndex), machine.GetProbeSize(axisIndex), machine.ProbeDist, machine.ProbeDistUp, machine.ProbeFeed);
    }

    #endregion

    #region send/queue

    public static void PrepareAndQueueCommand(this ISerial serial, Machine machine, string commandString)
    {
        serial.QueueCommand(machine.PrepareCommand(commandString));
    }

    public static async Task SendMacroCommandAsync(this ISerial serial, Machine machine, string commandString)
    {
        string[] separators = { @"\n" };
        string[] cmds       = commandString.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in cmds)
        {
            string[] infos = s.Split(':');
            int      axis;

            if (infos.Length > 1 && string.Compare(infos[0], @";probe", true) == 0 && -1 != (axis = GCodeHelper.AxisNameToIndex(infos[1])))
            {
                if (false == await serial.SendProbeCommandAsync(machine, axis))
                {
                    break;
                }
            }
            else if (infos.Length == 1 && string.Compare(infos[0], @";beep", true) == 0)
            {
                SystemSounds.Beep.Play();
            }
            else
            {
                if (s.TrimEnd().EndsWith("?"))
                {
                    var result = await serial.SendCommandAsync(s.TrimEnd().TrimEnd('?'), GCodeSerial.DefaultTimeout);
                    if (result?.LastOrDefault()?.ReplyType.HasFlag(EReplyType.ReplyError) == false)
                    {
                        return;
                    }
                }
                else
                {
                    await serial.SendCommandAsync(s, GCodeSerial.DefaultTimeout);
                }
            }
        }
    }

    #endregion
}