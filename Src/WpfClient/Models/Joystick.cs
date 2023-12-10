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

namespace CNCLib.WpfClient.Models;

using System.ComponentModel;

public class Joystick
{
    const string CATEGORY_INTERNAL      = "Internal";
    const string CATEGORY_GENERAL       = "General";
    const string CATEGORY_COMMUNICATION = "Communication";

    [ReadOnly(false)]
    [Browsable(false)]
    [Category(CATEGORY_INTERNAL)]
    [DisplayName("Joystick Id")]
    [Description("Internal Id of joystick")]
    public int Id { get; set; }

    [Category(CATEGORY_COMMUNICATION)]
    [DisplayName("SerialServer")]
    [Description("Name of the CNCLib.Serial.Server, e.g. IP-Address, localhost or url - if empty, local port is used, e.g: https://servername:5000/serial.server")]
    public string? SerialServer { get; set; }

    [Category(CATEGORY_COMMUNICATION)]
    [DisplayName("SerialServerUser")]
    [Description("User to be used for to connect to the CNCLib.Serial.Server, default is empty")]
    public string? SerialServerUser { get; set; }

    [Category(CATEGORY_COMMUNICATION)]
    [DisplayName("SerialServerPassword")]
    [Description("Password to be used for the CNCLib.Serial.Server, default is empty")]
    public string? SerialServerPassword { get; set; }

    [Category(CATEGORY_COMMUNICATION)]
    [DisplayName("ComPort")]
    [Description("Com of attached joystick")]
    public string? ComPort { get; set; }

    [Category(CATEGORY_COMMUNICATION)]
    [DisplayName("BaudRate")]
    [Description("BaudRate")]
    public int BaudRate { get; set; }

    [Category(CATEGORY_GENERAL)]
    [DisplayName("InitCommands")]
    [Description(@"Commands sent to the joystick after connected. Seperate commands with \n")]
    public string? InitCommands { get; set; }
}