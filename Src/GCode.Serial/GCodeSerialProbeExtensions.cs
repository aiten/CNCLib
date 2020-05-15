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

using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using CNCLib.GCode.Tools;

using Framework.Arduino.SerialCommunication;
using Framework.Arduino.SerialCommunication.Abstraction;

namespace CNCLib.GCode.Serial
{
    public static class GCodeSerialProbeExtension
    {
        public static async Task<bool> SendProbeCommandAsync(this ISerial serial, string axisName, decimal probeSize, decimal probeDist, decimal probeDistUp, decimal probeFeed)
        {
            var result = await serial.SendCommandAsync($"g91 g31 {axisName}-{probeDist.ToString(CultureInfo.InvariantCulture)} F{probeFeed.ToString(CultureInfo.InvariantCulture)} g90", GCodeSerial.DefaultProbeTimeout);
            if (result?.LastOrDefault()?.ReplyType.HasFlag(EReplyType.ReplyError) == false)
            {
                serial.QueueCommand($"g92 {axisName}{(-probeSize).ToString(CultureInfo.InvariantCulture)}");
                serial.QueueCommand($"g91 g0 {axisName}{probeDistUp.ToString(CultureInfo.InvariantCulture)} g90");
                return true;
            }

            return false;
        }
    }
}