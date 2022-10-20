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

namespace CNCLib.GCode.Serial;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Framework.Arduino.SerialCommunication;
using Framework.Arduino.SerialCommunication.Abstraction;

public static class GCodeSerialExtension
{
    public const int DefaultTimeout = 120 * 1000;

    public static async Task<decimal?> GetParameterValueAsync(this ISerial serial, int parameter, string commandPrefix)
    {
        string message = await serial.SendCommandAndReadOKReplyAsync($"{commandPrefix}(print, #{parameter})", 10 * 1000);

        if (!string.IsNullOrEmpty(message))
        {
            // expected response : 0\nok
            string pos = message.Split('\n').FirstOrDefault();
            if (decimal.TryParse(pos, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal val))
            {
                return val;
            }
        }

        return null;
    }

    #region GetPosition

    private static decimal[] Convert(string[] list)
    {
        var ret = new decimal[list.Length];
        for (int i = 0; i < list.Length; i++)
        {
            decimal val;
            if (decimal.TryParse(list[i], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out val))
            {
                ret[i] = val;
            }
        }

        return ret;
    }

    static string TrimMsg(string msg, string replace)
    {
        return msg.Replace("ok", "").Replace(" ", "").Replace(replace, "").Replace(">", "");
    }

    static decimal[] Convert(string msg, string replace)
    {
        return Convert(TrimMsg(msg, replace).Split(':', ','));
    }

    static decimal[] TryConvert(string[] tags, string txt)
    {
        string tag = tags.FirstOrDefault((s) => s.StartsWith(txt));
        if (tag != null)
        {
            return Convert(TrimMsg(tag, txt).Split(':', ','));
        }

        return null;
    }

    public static async Task<IEnumerable<IEnumerable<decimal>>> GetPosition(this ISerial serial, string commandPrefix)
    {
        string message = await serial.SendCommandAndReadOKReplyAsync($"{commandPrefix}?", 10 * 1000);
        var    ret     = new List<IEnumerable<decimal>>();

        if (!string.IsNullOrEmpty(message))
        {
            if (message.Contains("MPos:"))
            {
                // new or grbl format
                message = message.Replace("ok", "").Replace("<", "").Replace(">", "").Trim();

                string[] tags = message.Split('|');

                var mPos = TryConvert(tags, "MPos:");
                if (mPos != null)
                {
                    ret.Add(mPos.ToArray());

                    var wco = TryConvert(tags, "WCO:");
                    if (wco != null)
                    {
                        for (int i = 0; i < wco.Length; i++)
                        {
                            mPos[i] -= wco[i];
                        }
                    }

                    ret.Add(mPos);
                }
            }
            else
            {
                decimal[] mPos = Convert(message, "dummy");
                ret.Add(mPos);

                message = await serial.SendCommandAndReadOKReplyAsync($"{commandPrefix}m114 s1", 10 * 1000);

                if (!string.IsNullOrEmpty(message))
                {
                    decimal[] rPos = Convert(message, "dummy");
                    ret.Add(rPos);
                }
            }
        }

        return ret;
    }

    #endregion
}