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

namespace CNCLib.Serial.WebAPI.Controllers;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CNCLib.GCode.Draw;
using CNCLib.GCode.Generate.Load;
using CNCLib.Serial.Shared;
using CNCLib.Serial.WebAPI.Hubs;
using CNCLib.Serial.WebAPI.Models;
using CNCLib.Serial.WebAPI.SerialPort;

using Framework.Arduino.SerialCommunication;
using Framework.Drawing;
using Framework.WebAPI.Controller;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

using SkiaSharp;

[Authorize]
[Route("api/[controller]")]
public class HpglController : Controller
{
    public HpglController()
    {
    }

    #region Convert

    [HttpPost("split")]
    public async Task<IEnumerable<string>> Get([FromBody] IEnumerable<string> commands)
    {
        return await Task.FromResult(SerialExtensions.SplitHpglCommands(commands));
    }

    #endregion

}