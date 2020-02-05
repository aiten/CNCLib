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

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using CNCLib.GCode.Serial;
using CNCLib.Serial.Shared;
using CNCLib.Serial.WebAPI.Hubs;
using CNCLib.Serial.WebAPI.SerialPort;

using Framework.Arduino.SerialCommunication;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CNCLib.Serial.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class GCodeController : Controller
    {
        private readonly IHubContext<CNCLibHub> _hubContext;
        private static   IHubContext<CNCLibHub> _myHubContext;

        public GCodeController(IHubContext<CNCLibHub> hubContext)
        {
            _hubContext   = hubContext;
            _myHubContext = _hubContext;
        }

        #region SpecialCommand

        [HttpPost("{id:int}/getParameter")]
        public async Task<ActionResult<decimal>> GetParameter(int id, int paramNo)
        {
            var port = await SerialPortList.GetPortAndRescan(id);

            if (port == null)
            {
                return NotFound();
            }

            var paramValue = await port.Serial.GetParameterValueAsync(paramNo, port.GCodeCommandPrefix);

            return Ok(paramValue);
        }

        [HttpPost("{id:int}/getPosition")]
        public async Task<ActionResult<IEnumerable<IEnumerable<decimal>>>> GetPosition(int id)
        {
            var port = await SerialPortList.GetPortAndRescan(id);

            if (port == null)
            {
                return NotFound();
            }

            var position = await port.Serial.GetPosition(port.GCodeCommandPrefix);

            return Ok(position);
        }

        #endregion
    }
}